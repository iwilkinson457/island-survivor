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

        const float PANEL_MARGIN_X = 32f;
        const float PANEL_MARGIN_Y = 24f;
        const float PANEL_PADDING = 20f;
        const float CELL = 52f;
        const float GAP = 2f;
        const float STEP = 54f;
        const float EQUIP_MIN_W = 380f;
        const float EQUIP_MAX_W = 460f;
        const float PANEL_GAP = 18f;

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
                PANEL_MARGIN_X,
                PANEL_MARGIN_Y,
                Screen.width - (PANEL_MARGIN_X * 2f),
                Screen.height - (PANEL_MARGIN_Y * 2f));

            float px = _panelRect.x;
            float py = _panelRect.y;
            float innerX = px + PANEL_PADDING;
            float innerY = py + PANEL_PADDING;
            float innerW = _panelRect.width - PANEL_PADDING * 2f;
            float innerH = _panelRect.height - PANEL_PADDING * 2f;
            float gridMinW = STEP * 8f + 12f;
            float equipW = Mathf.Clamp(innerW * 0.34f, EQUIP_MIN_W, EQUIP_MAX_W);
            float contentW = innerW - equipW - PANEL_GAP;
            if (contentW < gridMinW)
            {
                contentW = gridMinW;
                equipW = innerW - contentW - PANEL_GAP;
            }
            equipW = Mathf.Max(320f, equipW);

            float equipX = innerX;
            float equipY = innerY;
            float equipH = innerH;
            float contentX = equipX + equipW + PANEL_GAP;
            float contentY = innerY;
            float contentH = innerH;

            GUI.Box(_panelRect, "");
            GUI.Box(new Rect(contentX - (PANEL_GAP * 0.5f), py + 8f, 3f, _panelRect.height - 16f), "");

            DrawEquipmentPanel(equipX, equipY, equipW, equipH);
            DrawTabs(contentX, contentY, contentW);
            if (_tab == 0)
                DrawStorageTab(contentX, contentY + 42f, contentW, contentH - 48f);
            else
                DrawCraftingTab(contentX, contentY + 42f, contentW, contentH - 48f);

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

        private void DrawTabs(float x, float y, float width)
        {
            float tabW = Mathf.Min(160f, (width - 8f) * 0.5f);
            GUI.color = _tab == 0 ? Color.cyan : Color.white;
            if (GUI.Button(new Rect(x, y, tabW, 32f), "Inventory")) _tab = 0;
            GUI.color = _tab == 1 ? Color.cyan : Color.white;
            if (GUI.Button(new Rect(x + tabW + 8f, y, tabW, 32f), "Crafting")) _tab = 1;
            GUI.color = Color.white;
        }

        private void DrawStorageTab(float x, float y, float width, float height)
        {
            var grid = inventory.SpatialGrid;
            float gridPixelW = grid.Width * STEP - GAP;
            float gridX = x + Mathf.Max(0f, (width - gridPixelW) * 0.5f);

            GUI.Label(new Rect(x, y, width, 22f), $"<b>Backpack Inventory</b> ({grid.Width}x{grid.Height})");
            y += 28f;

            float gridBgH = grid.Height * STEP + 12f;
            GUI.Box(new Rect(x, y - 4f, width - 8f, gridBgH), "");
            DrawGrid(gridX, y + 2f, grid);

            float hotbarY = y + gridBgH + 18f;
            GUI.Label(new Rect(x, hotbarY, width, 22f), "<b>Pockets / Hotbar</b> — keys 1 to 6");
            hotbarY += 24f;

            float hotbarWidth = inventory.PocketsHotbar.Count * STEP;
            float hotbarX = x + Mathf.Max(0f, (width - hotbarWidth) * 0.5f);
            DrawPocketSlots(hotbarX, hotbarY);
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
                var slotRect = new Rect(ox + i * STEP, oy, CELL, CELL + 18f);
                var itemRect = new Rect(slotRect.x, slotRect.y + 16f, CELL, CELL);
                var slot = inventory.PocketsHotbar[i];
                bool isHovered = slotRect.Contains(e.mousePosition);

                GUI.color = Color.white;
                GUI.Label(new Rect(slotRect.x, slotRect.y, CELL, 16f), $"[{i + 1}]");

                if (slot.HasItem)
                {
                    GUI.color = new Color(0.65f, 0.82f, 1f);
                    GUI.Box(itemRect, $"{slot.Item.DisplayName}\nx{slot.Quantity}");
                }
                else if (_dragging && isHovered)
                {
                    GUI.color = new Color(0.3f, 0.8f, 0.3f);
                    GUI.Box(itemRect, "");
                }
                else
                {
                    GUI.color = new Color(0.2f, 0.2f, 0.2f);
                    GUI.Box(itemRect, "");
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

        private void DrawEquipmentPanel(float ox, float oy, float width, float height)
        {
            var e = Event.current;
            GUI.Label(new Rect(ox, oy, width, 24f), "<b>Equipment</b>");

            var panelRect = new Rect(ox, oy + 28f, width, height - 28f);
            GUI.Box(panelRect, "");

            float centerX = ox + width * 0.5f;
            float topY = oy + 54f;
            float slotW = Mathf.Clamp(width * 0.33f, 124f, 156f);
            float slotH = 52f;
            float silhouetteW = Mathf.Clamp(width * 0.34f, 140f, 180f);
            var silhouetteRect = new Rect(centerX - silhouetteW * 0.5f, topY + 18f, silhouetteW, Mathf.Min(height - 130f, 360f));
            DrawBodySilhouette(silhouetteRect);

            DrawEquipmentSlot(EquipmentSlotType.Head,     new Rect(centerX - slotW * 0.5f, topY, slotW, slotH), "Head", e);
            DrawEquipmentSlot(EquipmentSlotType.Weapon1,  new Rect(ox + 14f, topY + 110f, slotW, slotH), "Weapon 1", e);
            DrawEquipmentSlot(EquipmentSlotType.Weapon2,  new Rect(ox + width - slotW - 14f, topY + 110f, slotW, slotH), "Weapon 2", e);
            DrawEquipmentSlot(EquipmentSlotType.Torso,    new Rect(centerX - slotW * 0.5f, topY + 150f, slotW, slotH), "Torso", e);
            DrawEquipmentSlot(EquipmentSlotType.Backpack, new Rect(centerX - slotW * 0.5f, topY + 214f, slotW, slotH), "Backpack", e);
            DrawEquipmentSlot(EquipmentSlotType.Legs,     new Rect(centerX - slotW * 0.5f, topY + 278f, slotW, slotH), "Legs", e);
        }

        private void DrawBodySilhouette(Rect rect)
        {
            float headW = rect.width * 0.34f;
            float headH = rect.height * 0.16f;
            float torsoW = rect.width * 0.52f;
            float torsoH = rect.height * 0.32f;
            float armW = rect.width * 0.14f;
            float armH = rect.height * 0.28f;
            float legW = rect.width * 0.17f;
            float legH = rect.height * 0.34f;
            float centerX = rect.x + rect.width * 0.5f;

            GUI.color = new Color(0.15f, 0.18f, 0.22f, 0.9f);
            GUI.Box(new Rect(centerX - headW * 0.5f, rect.y + rect.height * 0.03f, headW, headH), "");
            GUI.Box(new Rect(centerX - torsoW * 0.5f, rect.y + rect.height * 0.22f, torsoW, torsoH), "");
            GUI.Box(new Rect(centerX - torsoW * 0.5f - armW - 8f, rect.y + rect.height * 0.26f, armW, armH), "");
            GUI.Box(new Rect(centerX + torsoW * 0.5f + 8f, rect.y + rect.height * 0.26f, armW, armH), "");
            GUI.Box(new Rect(centerX - legW - 8f, rect.y + rect.height * 0.58f, legW, legH), "");
            GUI.Box(new Rect(centerX + 8f, rect.y + rect.height * 0.58f, legW, legH), "");
            GUI.color = Color.white;
        }

        private void DrawEquipmentSlot(EquipmentSlotType slotType, Rect slotRect, string label, Event e)
        {
            var equip = inventory.GetEquipmentSlot(slotType);
            if (equip == null) return;

            bool isHovered = slotRect.Contains(e.mousePosition);
            string content = equip.HasItem ? equip.Item.DisplayName : "Empty";
            string boxText = $"{label}\n{content}";

            if (equip.HasItem)
                GUI.color = new Color(0.9f, 0.75f, 1f);
            else if (_dragging && isHovered && _dragItem != null)
            {
                bool acceptable = _dragItem.IsCompatibleWithEquipmentSlot(slotType);
                GUI.color = acceptable ? new Color(0.3f, 0.8f, 0.3f) : new Color(0.8f, 0.3f, 0.3f);
            }
            else
                GUI.color = new Color(0.25f, 0.2f, 0.3f);

            GUI.Box(slotRect, boxText);
            GUI.color = Color.white;

            if (!isHovered) return;

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
            }
        }

        private void DrawCraftingTab(float x, float y, float width, float height)
        {
            if (crafter == null)
            {
                GUI.Label(new Rect(x, y, width, 20), "No crafter");
                return;
            }

            bool nearFire = campfireTracker != null && campfireTracker.IsNearCampfire;
            GUI.Label(new Rect(x, y, width, 20),
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

                float rowH = 56f;
                var rowRect = new Rect(x, y, width - 10f, rowH);

                GUI.color = canCraft ? new Color(0.8f, 1f, 0.8f) : new Color(0.4f, 0.4f, 0.4f);
                GUI.Box(rowRect, "");
                GUI.color = Color.white;

                string placeTag = recipe.OutputItem != null && recipe.OutputItem.IsPlaceable ? " [Placeable]" : "";
                GUI.Label(new Rect(x + 6, y + 4, width - 120f, 20), $"{recipe.DisplayName}{placeTag}");
                GUI.Label(new Rect(x + 6, y + 24, width - 120f, 24), ingList);

                GUI.enabled = canCraft;
                if (GUI.Button(new Rect(x + width - 102f, y + 12, 88, 30), "Craft"))
                    crafter.TryCraft(recipe, nearFire);
                GUI.enabled = true;

                y += rowH + 6f;
                if (y > _panelRect.yMax - 80f)
                    break;
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
            if (slotType == EquipmentSlotType.Backpack && inventory.HasBackpackContents)
            {
                Debug.Log("[InventoryPanel] Cannot drag equipped backpack out while it still contains items.");
                return;
            }

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

            if (slotType == EquipmentSlotType.Backpack)
            {
                if (!inventory.TryEquipItem(_dragItem, EquipmentSlotType.Backpack))
                {
                    ReturnDragToOrigin();
                    return;
                }

                _dragging = false;
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
            if (_ctxDomain == InventoryDomain.Equipment && _ctxEquip == EquipmentSlotType.Backpack && inventory.HasBackpackContents)
            {
                Debug.Log("[InventoryPanel] Cannot drop equipped backpack while it still contains items.");
                return;
            }

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
                if (slot == null) continue;
                if (slot.HasItem && slotType != EquipmentSlotType.Backpack) continue;

                RemoveFromCtxSource();
                if (inventory.TryEquipItem(_ctxItem, slotType))
                {
                    equipped = true;
                    break;
                }
                ReturnCtxItemToSource();
                break;
            }
            if (!equipped)
                Debug.Log("[InventoryPanel] No empty compatible equipment slot for equip.");
        }

        private void ExecuteCtxUnequip()
        {
            var slot = inventory.GetEquipmentSlot(_ctxEquip);
            if (slot == null || !slot.HasItem) return;

            if (_ctxEquip == EquipmentSlotType.Backpack && inventory.HasBackpackContents)
            {
                Debug.Log("[InventoryPanel] Cannot unequip backpack while it still contains items.");
                return;
            }

            var item = inventory.UnequipItem(_ctxEquip);
            if (item == null) return;

            if (!inventory.TryAddItem(item, 1))
            {
                inventory.TryEquipItem(item, _ctxEquip);
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

        private void ReturnCtxItemToSource()
        {
            switch (_ctxDomain)
            {
                case InventoryDomain.PocketsHotbar:
                    inventory.PocketsHotbar[_ctxPocket].TryAdd(_ctxItem, _ctxQty);
                    break;
                case InventoryDomain.BackpackGrid:
                    inventory.SpatialGrid.TryPlaceFirstFit(_ctxItem, _ctxQty);
                    break;
            }
            inventory.NotifyChanged();
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
