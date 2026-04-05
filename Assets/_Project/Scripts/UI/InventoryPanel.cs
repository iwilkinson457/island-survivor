using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ExtractionDeadIsles.Building;
using ExtractionDeadIsles.Crafting;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;
using ExtractionDeadIsles.Player;
using ExtractionDeadIsles.World;
using ExtractionDeadIsles.Interaction;

namespace ExtractionDeadIsles.UI
{
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerCrafter crafter;
        [SerializeField] private PlayerStats stats;
        [SerializeField] private CampfireProximityTracker campfireTracker;
        [SerializeField] private SimplePlacementController placementController;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float worldDropDistance = 1.5f;
        [SerializeField] private LayerMask interactLayer;

        const float PANEL_W = 820f;
        const float PANEL_H = 600f;
        const float CELL = 52f;
        const float GAP = 2f;
        const float STEP = 54f;
        const float LEFT_W = 490f;
        const float RIGHT_X_OFFSET = 505f;
        const float RIGHT_W = 305f;

        bool _visible;
        int _tab;
        Rect _panelRect;

        bool _dragging;
        ItemDefinition _dragItem;
        int _dragQuantity;
        bool _dragRotated;
        InventoryDomain _dragSrcDomain;
        int _dragSrcPocket;
        int _dragSrcGx, _dragSrcGy;
        bool _dragSrcGrotated;
        EquipmentSlotType _dragSrcEquip;

        bool _ctxOpen;
        Rect _ctxRect;
        ItemDefinition _ctxItem;
        int _ctxQty;
        InventoryDomain _ctxDomain;
        int _ctxPocket;
        int _ctxGx, _ctxGy;
        EquipmentSlotType _ctxEquip;

        private void Reset()
        {
            inventory = GetComponent<PlayerInventory>();
            crafter = GetComponent<PlayerCrafter>();
            stats = GetComponent<PlayerStats>();
            campfireTracker = GetComponent<CampfireProximityTracker>();
            placementController = GetComponent<SimplePlacementController>();
            playerTransform = transform;
        }

        private void OnEnable()
        {
            if (crafter != null)
                crafter.OnCraftedPlaceable += OnCraftedPlaceable;
            SetVisible(false);
        }

        private void OnDisable()
        {
            if (crafter != null)
                crafter.OnCraftedPlaceable -= OnCraftedPlaceable;
            SetVisible(false);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.tabKey.wasPressedThisFrame)
                SetVisible(!_visible);
        }

        private void SetVisible(bool v)
        {
            _visible = v;
            Cursor.lockState = v ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = v;
            if (!v && _dragging)
                ReturnDragToOrigin();
            if (!v)
                _ctxOpen = false;
        }

        private void OnCraftedPlaceable(ItemDefinition item)
        {
            SetVisible(false);
            placementController?.BeginPlacementWithItem(item);
        }

        private void OnGUI()
        {
            if (!_visible || inventory == null) return;
            var e = Event.current;

            _panelRect = new Rect(
                (Screen.width  - PANEL_W) * 0.5f,
                (Screen.height - PANEL_H) * 0.5f,
                PANEL_W, PANEL_H);

            float px = _panelRect.x;
            float py = _panelRect.y;

            GUI.Box(_panelRect, "");
            GUI.Box(new Rect(px + 498, py + 5, 3, PANEL_H - 10), "");

            DrawTabs(px + 5, py + 5);
            if (_tab == 0)
                DrawStorageTab(px + 5, py + 38);
            else
                DrawCraftingTab(px + 5, py + 38);

            DrawEquipmentPanel(px + RIGHT_X_OFFSET, py + 5);

            if (_dragging && e.type == EventType.Repaint)
            {
                string rot = _dragRotated ? " [R]" : "";
                GUI.color = new Color(1f, 1f, 0.4f, 0.95f);
                GUI.Label(
                    new Rect(e.mousePosition.x + 12, e.mousePosition.y - 14, 200, 22),
                    $"[{_dragItem.DisplayName} x{_dragQuantity}{rot}]");
                GUI.color = Color.white;
            }

            if (_dragging && e.type == EventType.KeyDown && e.keyCode == KeyCode.R
                && _dragItem != null && _dragItem.Rotatable)
            {
                _dragRotated = !_dragRotated;
                e.Use();
            }

            if (_ctxOpen)
                DrawContextMenu(e);

            if (e.type == EventType.MouseUp && e.button == 0 && _dragging)
            {
                if (!_panelRect.Contains(e.mousePosition))
                    DropToWorld();
                else
                    ReturnDragToOrigin();
                e.Use();
            }

            if (e.type == EventType.MouseDown && _ctxOpen && !_ctxRect.Contains(e.mousePosition))
            {
                _ctxOpen = false;
                e.Use();
            }
        }

        private void DrawTabs(float x, float y)
        {
            GUI.color = _tab == 0 ? Color.cyan : Color.white;
            if (GUI.Button(new Rect(x, y, 120, 28), "Storage")) _tab = 0;
            GUI.color = _tab == 1 ? Color.cyan : Color.white;
            if (GUI.Button(new Rect(x + 125, y, 120, 28), "Crafting")) _tab = 1;
            GUI.color = Color.white;
        }

        private void DrawStorageTab(float x, float y)
        {
            var grid = inventory.SpatialGrid;
            GUI.Label(new Rect(x, y, LEFT_W, 20), $"<b>Backpack</b> ({grid.Width}x{grid.Height})");
            y += 22;
            DrawGrid(x, y, grid);
            y += grid.Height * STEP + 8;
            GUI.Label(new Rect(x, y, LEFT_W, 20), "<b>Pockets / Hotbar</b>");
            y += 22;
            DrawPocketSlots(x, y);
        }

        private void DrawGrid(float ox, float oy, BackpackGrid grid)
        {
            var e = Event.current;

            int hovCol = Mathf.FloorToInt((e.mousePosition.x - ox) / STEP);
            int hovRow = Mathf.FloorToInt((e.mousePosition.y - oy) / STEP);

            int dw = 1, dh = 1;
            if (_dragging && _dragItem != null)
            {
                dw = _dragRotated ? _dragItem.GridHeight : _dragItem.GridWidth;
                dh = _dragRotated ? _dragItem.GridWidth  : _dragItem.GridHeight;
            }

            // Pass 1 — empty cell backgrounds
            for (int row = 0; row < grid.Height; row++)
            {
                for (int col = 0; col < grid.Width; col++)
                {
                    var cellRect = new Rect(ox + col * STEP, oy + row * STEP, CELL, CELL);
                    var pi = grid.GetItemAt(col, row);
                    if (pi == null)
                    {
                        if (_dragging && _dragItem != null
                            && col >= hovCol && col < hovCol + dw
                            && row >= hovRow && row < hovRow + dh)
                        {
                            bool fits = grid.CanPlace(_dragItem, hovCol, hovRow, _dragRotated);
                            GUI.color = fits ? new Color(0.3f, 0.8f, 0.3f) : new Color(0.8f, 0.3f, 0.3f);
                        }
                        else
                        {
                            GUI.color = new Color(0.15f, 0.15f, 0.15f);
                        }
                        GUI.Box(cellRect, "");
                        GUI.color = Color.white;
                    }
                }
            }

            // Pass 2 — draw placed items at anchor positions
            var drawn = new HashSet<PlacedItem>();
            foreach (var pi in grid.GetAllPlaced())
            {
                if (drawn.Contains(pi)) continue;
                drawn.Add(pi);
                float iw = pi.EffectiveWidth  * STEP - GAP;
                float ih = pi.EffectiveHeight * STEP - GAP;
                var itemRect = new Rect(ox + pi.x * STEP, oy + pi.y * STEP, iw, ih);
                GUI.color = new Color(0.65f, 0.82f, 1f);
                GUI.Box(itemRect, $"{pi.item.DisplayName}\nx{pi.quantity}");
                GUI.color = Color.white;
            }

            // Pass 3 — input handling
            for (int row = 0; row < grid.Height; row++)
            {
                for (int col = 0; col < grid.Width; col++)
                {
                    var cellRect = new Rect(ox + col * STEP, oy + row * STEP, CELL, CELL);
                    if (!cellRect.Contains(e.mousePosition)) continue;
                    var pi = grid.GetItemAt(col, row);

                    if (e.type == EventType.MouseDown && e.button == 0 && !_dragging && pi != null)
                    {
                        StartDragFromGrid(pi);
                        e.Use();
                        return;
                    }

                    if (e.type == EventType.MouseDown && e.button == 1 && !_dragging && pi != null)
                    {
                        ShowCtxGrid(pi, col, row);
                        e.Use();
                        return;
                    }

                    if (e.type == EventType.MouseUp && e.button == 0 && _dragging)
                    {
                        int anchorCol = Mathf.Clamp(Mathf.FloorToInt((e.mousePosition.x - ox) / STEP), 0, grid.Width  - 1);
                        int anchorRow = Mathf.Clamp(Mathf.FloorToInt((e.mousePosition.y - oy) / STEP), 0, grid.Height - 1);
                        CompleteDragToGrid(anchorCol, anchorRow);
                        e.Use();
                        return;
                    }
                }
            }
        }

        private void DrawPocketSlots(float ox, float oy)
        {
            var e = Event.current;
            for (int i = 0; i < inventory.PocketsHotbar.Count; i++)
            {
                var slotRect = new Rect(ox + i * STEP, oy, CELL, CELL);
                var slot = inventory.PocketsHotbar[i];
                bool isHovered = slotRect.Contains(e.mousePosition);

                if (slot.HasItem)
                {
                    GUI.color = new Color(0.65f, 0.82f, 1f);
                    GUI.Box(slotRect, $"{slot.Item.DisplayName}\nx{slot.Quantity}");
                }
                else if (_dragging && isHovered)
                {
                    GUI.color = new Color(0.3f, 0.8f, 0.3f);
                    GUI.Box(slotRect, "");
                }
                else
                {
                    GUI.color = new Color(0.2f, 0.2f, 0.2f);
                    GUI.Box(slotRect, $"[{i + 1}]");
                }
                GUI.color = Color.white;

                if (!isHovered) continue;

                if (e.type == EventType.MouseDown && e.button == 0 && !_dragging && slot.HasItem)
                {
                    StartDragFromPocket(i);
                    e.Use();
                    return;
                }

                if (e.type == EventType.MouseDown && e.button == 1 && !_dragging && slot.HasItem)
                {
                    ShowCtxPocket(slot.Item, slot.Quantity, i);
                    e.Use();
                    return;
                }

                if (e.type == EventType.MouseUp && e.button == 0 && _dragging)
                {
                    CompleteDragToPocket(i);
                    e.Use();
                    return;
                }
            }
        }

        private void DrawEquipmentPanel(float ox, float oy)
        {
            var e = Event.current;
            GUI.Label(new Rect(ox, oy, RIGHT_W, 20), "<b>Equipment</b>");
            oy += 22;

            var slotTypes = (EquipmentSlotType[])System.Enum.GetValues(typeof(EquipmentSlotType));
            foreach (var slotType in slotTypes)
            {
                var equip = inventory.GetEquipmentSlot(slotType);
                if (equip == null) { oy += STEP; continue; }

                var slotRect = new Rect(ox, oy, RIGHT_W - 10, CELL);
                bool isHovered = slotRect.Contains(e.mousePosition);
                string content = equip.HasItem ? equip.Item.DisplayName : "Empty";
                string label   = $"{slotType}: {content}";

                if (equip.HasItem)
                    GUI.color = new Color(0.9f, 0.75f, 1f);
                else if (_dragging && isHovered && _dragItem != null)
                {
                    bool acceptable = _dragItem.IsCompatibleWithEquipmentSlot(slotType);
                    GUI.color = acceptable ? new Color(0.3f, 0.8f, 0.3f) : new Color(0.8f, 0.3f, 0.3f);
                }
                else
                    GUI.color = new Color(0.25f, 0.2f, 0.3f);

                GUI.Box(slotRect, label);
                GUI.color = Color.white;

                if (isHovered)
                {
                    if (e.type == EventType.MouseDown && e.button == 0 && !_dragging && equip.HasItem)
                    {
                        StartDragFromEquipment(slotType);
                        e.Use();
                        return;
                    }

                    if (e.type == EventType.MouseDown && e.button == 1 && !_dragging && equip.HasItem)
                    {
                        ShowCtxEquip(equip.Item, 1, slotType);
                        e.Use();
                        return;
                    }

                    if (e.type == EventType.MouseUp && e.button == 0 && _dragging)
                    {
                        CompleteDragToEquipment(slotType);
                        e.Use();
                        return;
                    }
                }

                oy += STEP;
            }
        }

        private void DrawCraftingTab(float x, float y)
        {
            if (crafter == null)
            {
                GUI.Label(new Rect(x, y, LEFT_W, 20), "No crafter");
                return;
            }

            bool nearFire = campfireTracker != null && campfireTracker.IsNearCampfire;
            GUI.Label(new Rect(x, y, LEFT_W, 20),
                $"<b>Crafting</b>{(nearFire ? " (near campfire)" : "")}");
            y += 24;

            foreach (var recipe in crafter.GetRecipes())
            {
                bool canCraft = crafter.CanCraft(recipe, nearFire);

                string ingList = "";
                if (recipe.Ingredients != null)
                {
                    foreach (var ing in recipe.Ingredients)
                    {
                        int have = inventory.CountItem(ing.item);
                        ingList += $"{(ing.item != null ? ing.item.DisplayName : "?")} {have}/{ing.amount}  ";
                    }
                }

                float rowH = 52f;
                var rowRect = new Rect(x, y, LEFT_W - 10, rowH);

                GUI.color = canCraft ? new Color(0.8f, 1f, 0.8f) : new Color(0.4f, 0.4f, 0.4f);
                GUI.Box(rowRect, "");
                GUI.color = Color.white;

                string placeTag = recipe.OutputItem != null && recipe.OutputItem.IsPlaceable ? " [Placeable]" : "";
                GUI.Label(new Rect(x + 4, y + 2,  260, 20), $"{recipe.DisplayName}{placeTag}");
                GUI.Label(new Rect(x + 4, y + 22, 280, 20), ingList);

                GUI.enabled = canCraft;
                if (GUI.Button(new Rect(x + LEFT_W - 95, y + 10, 85, 30), "Craft"))
                    crafter.TryCraft(recipe, nearFire);
                GUI.enabled = true;

                y += rowH + 4;
            }
        }

        // -------------------------------------------------------------------------
        // Drag: start
        // -------------------------------------------------------------------------

        private void StartDragFromPocket(int index)
        {
            var slot = inventory.PocketsHotbar[index];
            _dragItem       = slot.Item;
            _dragQuantity   = slot.Quantity;
            _dragRotated    = false;
            _dragSrcDomain  = InventoryDomain.PocketsHotbar;
            _dragSrcPocket  = index;
            slot.Clear();
            inventory.NotifyChanged();
            _dragging = true;
        }

        private void StartDragFromGrid(PlacedItem pi)
        {
            _dragItem        = pi.item;
            _dragQuantity    = pi.quantity;
            _dragRotated     = pi.rotated;
            _dragSrcDomain   = InventoryDomain.BackpackGrid;
            _dragSrcGx       = pi.x;
            _dragSrcGy       = pi.y;
            _dragSrcGrotated = pi.rotated;
            inventory.SpatialGrid.RemovePlaced(pi);
            inventory.NotifyChanged();
            _dragging = true;
        }

        private void StartDragFromEquipment(EquipmentSlotType slotType)
        {
            var slot      = inventory.GetEquipmentSlot(slotType);
            _dragItem     = slot.Item;
            _dragQuantity = 1;
            _dragRotated  = false;
            _dragSrcDomain = InventoryDomain.Equipment;
            _dragSrcEquip  = slotType;
            slot.Unequip();
            inventory.NotifyChanged();
            _dragging = true;
        }

        // -------------------------------------------------------------------------
        // Drag: return / cancel
        // -------------------------------------------------------------------------

        private void ReturnDragToOrigin()
        {
            if (!_dragging || _dragItem == null) { _dragging = false; return; }

            switch (_dragSrcDomain)
            {
                case InventoryDomain.PocketsHotbar:
                    inventory.PocketsHotbar[_dragSrcPocket].TryAdd(_dragItem, _dragQuantity);
                    break;
                case InventoryDomain.BackpackGrid:
                    inventory.SpatialGrid.TryPlace(_dragItem, _dragSrcGx, _dragSrcGy, _dragSrcGrotated, _dragQuantity);
                    break;
                case InventoryDomain.Equipment:
                    inventory.GetEquipmentSlot(_dragSrcEquip)?.TryEquip(_dragItem);
                    break;
            }
            inventory.NotifyChanged();
            _dragging = false;
        }

        // -------------------------------------------------------------------------
        // Drag: complete
        // -------------------------------------------------------------------------

        private void CompleteDragToPocket(int targetIndex)
        {
            var targetSlot = inventory.PocketsHotbar[targetIndex];

            if (!targetSlot.HasItem)
            {
                targetSlot.TryAdd(_dragItem, _dragQuantity);
                _dragging = false;
                inventory.NotifyChanged();
                return;
            }

            if (targetSlot.Item == _dragItem && _dragItem.Stackable)
            {
                int rem = targetSlot.TryAdd(_dragItem, _dragQuantity);
                if (rem > 0)
                {
                    _dragQuantity = rem;
                    ReturnDragToOrigin();
                    return;
                }
                _dragging = false;
                inventory.NotifyChanged();
                return;
            }

            // Swap
            var swapItem = targetSlot.Item;
            var swapQty  = targetSlot.Quantity;
            targetSlot.Clear();
            targetSlot.TryAdd(_dragItem, _dragQuantity);

            if (!PlaceItemAtSource(swapItem, swapQty))
            {
                targetSlot.Clear();
                targetSlot.TryAdd(swapItem, swapQty);
                ReturnDragToOrigin();
                return;
            }

            _dragging = false;
            inventory.NotifyChanged();
        }

        private void CompleteDragToGrid(int col, int row)
        {
            var grid = inventory.SpatialGrid;

            if (!grid.CanPlace(_dragItem, col, row, _dragRotated))
            {
                var pi = grid.GetItemAt(col, row);
                if (pi != null)
                {
                    var swapItem = pi.item;
                    var swapQty  = pi.quantity;
                    var swapRot  = pi.rotated;
                    var swapX    = pi.x;
                    var swapY    = pi.y;
                    grid.RemovePlaced(pi);

                    if (grid.CanPlace(_dragItem, col, row, _dragRotated))
                    {
                        grid.TryPlace(_dragItem, col, row, _dragRotated, _dragQuantity);
                        if (PlaceItemAtSource(swapItem, swapQty))
                        {
                            _dragging = false;
                            inventory.NotifyChanged();
                            return;
                        }
                        grid.RemoveItemAt(col, row);
                    }
                    grid.TryPlace(swapItem, swapX, swapY, swapRot, swapQty);
                }
                ReturnDragToOrigin();
                return;
            }

            grid.TryPlace(_dragItem, col, row, _dragRotated, _dragQuantity);
            _dragging = false;
            inventory.NotifyChanged();
        }

        private void CompleteDragToEquipment(EquipmentSlotType slotType)
        {
            var slot = inventory.GetEquipmentSlot(slotType);
            if (slot == null)
            {
                ReturnDragToOrigin();
                return;
            }

            if (!slot.CanAccept(_dragItem))
            {
                if (slot.HasItem)
                {
                    var swapItem = slot.Item;
                    slot.Clear();
                    if (slot.CanAccept(_dragItem))
                    {
                        slot.TryEquip(_dragItem);
                        if (PlaceItemAtSource(swapItem, 1))
                        {
                            _dragging = false;
                            if (slotType == EquipmentSlotType.Backpack && _dragItem.BackpackStorageWidth > 0)
                            {
                                var overflow = inventory.SpatialGrid.ResizeWithItems(
                                    _dragItem.BackpackStorageWidth,
                                    Mathf.Max(1, _dragItem.BackpackStorageHeight));
                                foreach (var ov in overflow)
                                    SpawnWorldDrop(ov.item, ov.quantity);
                            }
                            inventory.NotifyChanged();
                            return;
                        }
                        slot.Clear();
                    }
                    slot.TryEquip(swapItem);
                }
                ReturnDragToOrigin();
                return;
            }

            slot.TryEquip(_dragItem);
            _dragging = false;

            if (slotType == EquipmentSlotType.Backpack && _dragItem.BackpackStorageWidth > 0)
            {
                var overflow = inventory.SpatialGrid.ResizeWithItems(
                    _dragItem.BackpackStorageWidth,
                    Mathf.Max(1, _dragItem.BackpackStorageHeight));
                foreach (var ov in overflow)
                    SpawnWorldDrop(ov.item, ov.quantity);
            }

            inventory.NotifyChanged();
        }

        private bool PlaceItemAtSource(ItemDefinition item, int qty)
        {
            switch (_dragSrcDomain)
            {
                case InventoryDomain.PocketsHotbar:
                {
                    var sl = inventory.PocketsHotbar[_dragSrcPocket];
                    if (sl.HasItem) return false;
                    sl.TryAdd(item, qty);
                    return true;
                }
                case InventoryDomain.BackpackGrid:
                {
                    var g = inventory.SpatialGrid;
                    if (g.CanPlace(item, _dragSrcGx, _dragSrcGy, false))
                        return g.TryPlace(item, _dragSrcGx, _dragSrcGy, false, qty);
                    return g.TryPlaceFirstFit(item, qty);
                }
                case InventoryDomain.Equipment:
                {
                    var es = inventory.GetEquipmentSlot(_dragSrcEquip);
                    if (es != null && es.CanAccept(item)) return es.TryEquip(item);
                    return inventory.TryAddItem(item, qty);
                }
                default:
                    return false;
            }
        }

        // -------------------------------------------------------------------------
        // Context menu
        // -------------------------------------------------------------------------

        private void ShowCtxPocket(ItemDefinition item, int qty, int pocketIndex)
        {
            _ctxItem    = item;
            _ctxQty     = qty;
            _ctxDomain  = InventoryDomain.PocketsHotbar;
            _ctxPocket  = pocketIndex;
            _ctxOpen    = true;
            _ctxRect    = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 140, 0);
        }

        private void ShowCtxGrid(PlacedItem pi, int gx, int gy)
        {
            _ctxItem   = pi.item;
            _ctxQty    = pi.quantity;
            _ctxDomain = InventoryDomain.BackpackGrid;
            _ctxGx     = gx;
            _ctxGy     = gy;
            _ctxOpen   = true;
            _ctxRect   = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 140, 0);
        }

        private void ShowCtxEquip(ItemDefinition item, int qty, EquipmentSlotType slotType)
        {
            _ctxItem   = item;
            _ctxQty    = qty;
            _ctxDomain = InventoryDomain.Equipment;
            _ctxEquip  = slotType;
            _ctxOpen   = true;
            _ctxRect   = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 140, 0);
        }

        private struct CtxAction
        {
            public string label;
            public System.Action execute;
        }

        private void DrawContextMenu(Event e)
        {
            var actions = new List<CtxAction>();

            // Drop
            actions.Add(new CtxAction { label = "Drop", execute = ExecuteCtxDrop });

            // Consume
            if (_ctxDomain != InventoryDomain.Equipment
                && _ctxItem != null
                && (_ctxItem.HungerRestore > 0 || _ctxItem.ThirstRestore > 0))
                actions.Add(new CtxAction { label = "Consume", execute = ExecuteCtxConsume });

            // Equip (from non-equipment domain)
            if (_ctxDomain != InventoryDomain.Equipment
                && _ctxItem != null
                && _ctxItem.CompatibleEquipmentSlots != null
                && _ctxItem.CompatibleEquipmentSlots.Length > 0)
                actions.Add(new CtxAction { label = "Equip", execute = ExecuteCtxEquip });

            // Unequip (from equipment domain)
            if (_ctxDomain == InventoryDomain.Equipment)
                actions.Add(new CtxAction { label = "Unequip", execute = ExecuteCtxUnequip });

            float menuH = actions.Count * 26f + 8f;
            _ctxRect = new Rect(
                Mathf.Min(_ctxRect.x, Screen.width  - 144),
                Mathf.Min(_ctxRect.y, Screen.height - menuH - 4),
                140, menuH);

            GUI.Box(_ctxRect, "");
            float ay = _ctxRect.y + 4;
            foreach (var action in actions)
            {
                if (GUI.Button(new Rect(_ctxRect.x + 4, ay, 132, 22), action.label))
                {
                    action.execute();
                    _ctxOpen = false;
                }
                ay += 26;
            }
        }

        private void ExecuteCtxDrop()
        {
            switch (_ctxDomain)
            {
                case InventoryDomain.PocketsHotbar:
                    inventory.PocketsHotbar[_ctxPocket].Clear();
                    break;
                case InventoryDomain.BackpackGrid:
                    inventory.SpatialGrid.RemoveItemAt(_ctxGx, _ctxGy);
                    break;
                case InventoryDomain.Equipment:
                    inventory.GetEquipmentSlot(_ctxEquip)?.Clear();
                    break;
            }
            inventory.NotifyChanged();
            SpawnWorldDrop(_ctxItem, _ctxQty);
        }

        private void ExecuteCtxConsume()
        {
            if (stats != null) stats.Consume(_ctxItem);

            switch (_ctxDomain)
            {
                case InventoryDomain.PocketsHotbar:
                {
                    var sl = inventory.PocketsHotbar[_ctxPocket];
                    sl.Remove(1);
                    break;
                }
                case InventoryDomain.BackpackGrid:
                {
                    var pi = inventory.SpatialGrid.GetItemAt(_ctxGx, _ctxGy);
                    if (pi != null)
                    {
                        pi.quantity--;
                        if (pi.quantity <= 0)
                            inventory.SpatialGrid.RemoveItemAt(_ctxGx, _ctxGy);
                    }
                    break;
                }
            }
            inventory.NotifyChanged();
        }

        private void ExecuteCtxEquip()
        {
            if (_ctxItem == null || _ctxItem.CompatibleEquipmentSlots == null) return;
            bool equipped = false;
            foreach (var slotType in _ctxItem.CompatibleEquipmentSlots)
            {
                var slot = inventory.GetEquipmentSlot(slotType);
                if (slot == null || slot.HasItem) continue;
                RemoveFromCtxSource();
                slot.TryEquip(_ctxItem);
                if (slotType == EquipmentSlotType.Backpack && _ctxItem.BackpackStorageWidth > 0)
                {
                    var overflow = inventory.SpatialGrid.ResizeWithItems(
                        _ctxItem.BackpackStorageWidth,
                        Mathf.Max(1, _ctxItem.BackpackStorageHeight));
                    foreach (var ov in overflow)
                        SpawnWorldDrop(ov.item, ov.quantity);
                }
                inventory.NotifyChanged();
                equipped = true;
                break;
            }
            if (!equipped)
                Debug.Log("[InventoryPanel] No empty compatible equipment slot for equip.");
        }

        private void ExecuteCtxUnequip()
        {
            var slot = inventory.GetEquipmentSlot(_ctxEquip);
            if (slot == null || !slot.HasItem) return;
            var item = slot.Item;
            slot.Clear();
            if (!inventory.TryAddItem(item, 1))
            {
                slot.TryEquip(item);
                Debug.Log("[InventoryPanel] Can't unequip — no room in storage.");
            }
            else
            {
                inventory.NotifyChanged();
            }
        }

        private void RemoveFromCtxSource()
        {
            switch (_ctxDomain)
            {
                case InventoryDomain.PocketsHotbar:
                    inventory.PocketsHotbar[_ctxPocket].Clear();
                    break;
                case InventoryDomain.BackpackGrid:
                    inventory.SpatialGrid.RemoveItemAt(_ctxGx, _ctxGy);
                    break;
            }
        }

        // -------------------------------------------------------------------------
        // World drop
        // -------------------------------------------------------------------------

        private void DropToWorld()
        {
            if (_dragItem == null) { _dragging = false; return; }
            SpawnWorldDrop(_dragItem, _dragQuantity);
            _dragging = false;
            inventory.NotifyChanged();
        }

        private void SpawnWorldDrop(ItemDefinition item, int qty)
        {
            if (item == null || qty <= 0) return;
            Vector3 spawnPos = playerTransform != null
                ? playerTransform.position + playerTransform.forward * worldDropDistance + Vector3.up * 0.5f
                : Vector3.zero;

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = $"WorldDrop_{item.DisplayName}";
            go.transform.position = spawnPos;
            go.transform.localScale = Vector3.one * 0.3f;

            int layer = LayerMask.NameToLayer("Resource");
            if (layer >= 0) go.layer = layer;

            var pickup = go.AddComponent<WorldItemPickup>();
            pickup.item     = item;
            pickup.quantity = qty;

            Debug.Log($"[InventoryPanel] Dropped {item.DisplayName} x{qty} to world.");
        }
    }
}
