using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ExtractionDeadIsles.Building;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.UI
{
    public class CampfirePanel : MonoBehaviour
    {
        private enum DragSource
        {
            None,
            PlayerPocket,
            PlayerGrid,
            CampfireInput,
            CampfireFuel,
            CampfireOutput
        }

        [SerializeField] private PlayerInventory inventory;

        private CampfireStation _station;
        private bool _visible;
        private DragSource _dragSource;
        private ItemDefinition _dragItem;
        private int _dragQuantity;
        private int _dragPocketIndex;
        private int _dragGridX;
        private int _dragGridY;
        private bool _dragGridRotated;
        private ItemDefinition _tooltipItem;
        private Rect _tooltipAnchor;
        private Vector2 _inventoryScroll;

        private const float PANEL_MARGIN_X = 40f;
        private const float PANEL_MARGIN_Y = 28f;
        private const float PANEL_PADDING = 20f;
        private const float SLOT_SIZE = 76f;
        private const float SLOT_GAP = 6f;
        private const float HOTBAR_LABEL_H = 18f;

        private void Reset()
        {
            inventory = GetComponent<PlayerInventory>();
        }

        private void Awake()
        {
            if (inventory == null)
                inventory = GetComponent<PlayerInventory>();
        }

        private void Update()
        {
            if (!_visible) return;

            var keyboard = Keyboard.current;
            if (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame || keyboard.eKey.wasPressedThisFrame))
                Close();
        }

        public void Open(CampfireStation station)
        {
            if (station == null || inventory == null) return;
            _station = station;
            _visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Close()
        {
            if (_dragItem != null)
                ReturnDragToSource();

            _visible = false;
            _station = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnGUI()
        {
            if (!_visible || _station == null || inventory == null) return;

            Event e = Event.current;
            _tooltipItem = null;
            _tooltipAnchor = default;

            var panelRect = new Rect(PANEL_MARGIN_X, PANEL_MARGIN_Y, Screen.width - PANEL_MARGIN_X * 2f, Screen.height - PANEL_MARGIN_Y * 2f);
            GUI.Box(panelRect, "Campfire");

            float innerX = panelRect.x + PANEL_PADDING;
            float innerY = panelRect.y + PANEL_PADDING;
            float innerW = panelRect.width - PANEL_PADDING * 2f;
            float innerH = panelRect.height - PANEL_PADDING * 2f;
            float leftW = Mathf.Max(520f, innerW * 0.62f);
            float rightW = innerW - leftW - 18f;
            Rect playerRect = new Rect(innerX, innerY + 28f, leftW, innerH - 28f);
            Rect stationRect = new Rect(playerRect.xMax + 18f, innerY + 28f, rightW, innerH - 28f);

            GUI.Label(new Rect(innerX, innerY, leftW, 24f), "<b>Player Inventory</b>");
            GUI.Label(new Rect(stationRect.x, innerY, rightW, 24f), "<b>Campfire Station</b>");

            DrawPlayerInventory(playerRect, e);
            DrawStation(stationRect, e);

            if (GUI.Button(new Rect(panelRect.xMax - 120f, panelRect.y + 8f, 96f, 28f), "Close"))
                Close();

            if (_dragItem != null && e.type == EventType.Repaint)
            {
                var dragRect = new Rect(e.mousePosition.x - (SLOT_SIZE * 0.5f), e.mousePosition.y - (SLOT_SIZE * 0.5f), SLOT_SIZE, SLOT_SIZE);
                DrawItemCard(dragRect, _dragItem, _dragQuantity, new Color(1f, 1f, 1f, 0.88f), true);
            }

            if (_tooltipItem != null && _dragItem == null)
                DrawTooltip(e.mousePosition);

            if (e.type == EventType.MouseUp && e.button == 0 && _dragItem != null)
            {
                ReturnDragToSource();
                e.Use();
            }
        }

        private void DrawPlayerInventory(Rect rect, Event e)
        {
            GUI.Box(rect, "");
            GUI.Label(new Rect(rect.x + 8f, rect.y + 6f, rect.width - 16f, 20f), "Full hotbar + backpack view. Drop cooked output anywhere in this panel to return it to inventory.");

            Rect contentRect = new Rect(rect.x + 8f, rect.y + 30f, rect.width - 16f, rect.height - 38f);
            float gridPixelW = inventory.SpatialGrid.Width * (SLOT_SIZE + SLOT_GAP) - SLOT_GAP;
            float gridPixelH = inventory.SpatialGrid.Height * (SLOT_SIZE + SLOT_GAP) - SLOT_GAP;
            float hotbarPixelW = inventory.PocketsHotbar.Count * (SLOT_SIZE + SLOT_GAP) - SLOT_GAP;
            float contentWidth = Mathf.Max(contentRect.width - 20f, gridPixelW + 12f, hotbarPixelW + 12f);
            float contentHeight = 26f + SLOT_SIZE + 20f + HOTBAR_LABEL_H + 8f + gridPixelH + 12f;
            Rect viewRect = new Rect(0f, 0f, contentWidth, contentHeight);

            _inventoryScroll = GUI.BeginScrollView(contentRect, _inventoryScroll, viewRect);

            float hotbarX = 6f + Mathf.Max(0f, (contentWidth - hotbarPixelW) * 0.5f);
            float hotbarY = 6f + HOTBAR_LABEL_H;
            GUI.Label(new Rect(6f, 4f, contentWidth - 12f, HOTBAR_LABEL_H), "<b>Pockets / Hotbar</b>");
            DrawPocketSlots(hotbarX, hotbarY, e);

            float gridLabelY = hotbarY + SLOT_SIZE + 20f;
            GUI.Label(new Rect(6f, gridLabelY, contentWidth - 12f, HOTBAR_LABEL_H), $"<b>Backpack Grid</b> ({inventory.SpatialGrid.Width}x{inventory.SpatialGrid.Height})");
            float gridX = 6f + Mathf.Max(0f, (contentWidth - gridPixelW) * 0.5f);
            float gridY = gridLabelY + HOTBAR_LABEL_H + 8f;
            DrawBackpackGrid(gridX, gridY, e);

            GUI.EndScrollView();

            if (_dragItem != null && e.type == EventType.MouseUp && e.button == 0 && contentRect.Contains(e.mousePosition))
            {
                if (TryDropToPlayerInventory())
                {
                    e.Use();
                    return;
                }
            }
        }

        private void DrawPocketSlots(float ox, float oy, Event e)
        {
            for (int i = 0; i < inventory.PocketsHotbar.Count; i++)
            {
                var slot = inventory.PocketsHotbar[i];
                Rect slotRect = new Rect(ox + i * (SLOT_SIZE + SLOT_GAP), oy, SLOT_SIZE, SLOT_SIZE);
                DrawEmptySlot(slotRect);

                if (slot.HasItem)
                {
                    DrawItemCard(slotRect, slot.Item, slot.Quantity, new Color(0.65f, 0.82f, 1f), false);
                    if (slotRect.Contains(e.mousePosition))
                        SetTooltip(slot.Item, slotRect);
                }

                if (!slotRect.Contains(e.mousePosition))
                    continue;

                if (e.type == EventType.MouseDown && e.button == 0 && _dragItem == null && slot.HasItem)
                {
                    StartDragFromPocket(i);
                    e.Use();
                    return;
                }

                if (e.type == EventType.MouseUp && e.button == 0 && _dragItem != null)
                {
                    if (TryDropToPlayerInventory())
                    {
                        e.Use();
                        return;
                    }
                }
            }
        }

        private void DrawBackpackGrid(float ox, float oy, Event e)
        {
            var grid = inventory.SpatialGrid;

            for (int row = 0; row < grid.Height; row++)
            {
                for (int col = 0; col < grid.Width; col++)
                {
                    Rect cellRect = new Rect(ox + col * (SLOT_SIZE + SLOT_GAP), oy + row * (SLOT_SIZE + SLOT_GAP), SLOT_SIZE, SLOT_SIZE);
                    DrawEmptySlot(cellRect);
                }
            }

            var drawn = new HashSet<PlacedItem>();
            foreach (var placed in grid.GetAllPlaced())
            {
                if (placed == null || drawn.Contains(placed))
                    continue;

                drawn.Add(placed);
                float iw = placed.EffectiveWidth * (SLOT_SIZE + SLOT_GAP) - SLOT_GAP;
                float ih = placed.EffectiveHeight * (SLOT_SIZE + SLOT_GAP) - SLOT_GAP;
                Rect itemRect = new Rect(ox + placed.x * (SLOT_SIZE + SLOT_GAP), oy + placed.y * (SLOT_SIZE + SLOT_GAP), iw, ih);
                DrawItemCard(itemRect, placed.item, placed.quantity, new Color(0.65f, 0.82f, 1f), false);

                if (itemRect.Contains(e.mousePosition))
                {
                    SetTooltip(placed.item, itemRect);

                    if (e.type == EventType.MouseDown && e.button == 0 && _dragItem == null)
                    {
                        StartDragFromGrid(placed);
                        e.Use();
                        return;
                    }
                }
            }

            if (_dragItem != null && e.type == EventType.MouseUp && e.button == 0)
            {
                Rect fullGridRect = new Rect(ox, oy, grid.Width * (SLOT_SIZE + SLOT_GAP) - SLOT_GAP, grid.Height * (SLOT_SIZE + SLOT_GAP) - SLOT_GAP);
                if (fullGridRect.Contains(e.mousePosition) && TryDropToPlayerInventory())
                {
                    e.Use();
                }
            }
        }

        private void DrawStation(Rect rect, Event e)
        {
            GUI.Box(rect, "");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, 20f), _station.IsBurning ? "State: Burning" : "State: Unlit");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 28f, rect.width - 20f, 20f), $"Fuel Time: {_station.CurrentFuelSecondsRemaining:0.0}s");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 48f, rect.width - 20f, 20f), $"Cook Progress: {_station.CurrentCookProgress:0.0}/{_station.CookDurationSeconds:0.0}s");

            float slotX = rect.x + 10f;
            float slotY = rect.y + 96f;
            DrawLabeledStationSlot(new Rect(slotX, slotY, SLOT_SIZE, SLOT_SIZE), "Input", _station.InputItem, _station.InputQuantity, new Color(0.66f, 0.84f, 1f), e, DragSource.CampfireInput, () => TryDropToStationInput());
            DrawLabeledStationSlot(new Rect(slotX + SLOT_SIZE + 22f, slotY, SLOT_SIZE, SLOT_SIZE), "Fuel", _station.FuelItem, _station.FuelQuantity, new Color(1f, 0.86f, 0.55f), e, DragSource.CampfireFuel, () => TryDropToStationFuel());
            DrawLabeledStationSlot(new Rect(slotX + (SLOT_SIZE + 22f) * 2f, slotY, SLOT_SIZE, SLOT_SIZE), "Output", _station.OutputItem, _station.OutputQuantity, new Color(0.7f, 1f, 0.7f), e, DragSource.CampfireOutput, null);

            string lightLabel = _station.IsBurning ? "Burning" : "Light Fire";
            GUI.enabled = !_station.IsBurning && _station.CanLight;
            if (GUI.Button(new Rect(slotX, slotY + SLOT_SIZE + 26f, 140f, 34f), lightLabel))
                _station.TryLight();
            GUI.enabled = true;

            if (_station.OutputItem != null)
            {
                if (GUI.Button(new Rect(slotX + 156f, slotY + SLOT_SIZE + 26f, 180f, 34f), "Take Output To Inventory"))
                {
                    _station.TryTakeOutputToPlayer(inventory);
                    inventory.NotifyChanged();
                }
            }

            GUI.Label(new Rect(slotX, slotY + SLOT_SIZE + 70f, rect.width - 20f, 80f),
                "Drag raw fish into Input, drag sticks into Fuel, and drag cooked fish from Output back into the inventory panel. The quick take button is also there as a safety net.");
        }

        private void DrawLabeledStationSlot(Rect rect, string label, ItemDefinition item, int quantity, Color tint, Event e, DragSource source, System.Func<bool> tryDrop)
        {
            GUI.Label(new Rect(rect.x, rect.y - 18f, rect.width + 40f, 16f), label);
            DrawEmptySlot(rect);
            if (item != null)
            {
                DrawItemCard(rect, item, quantity, tint, false);
                if (rect.Contains(e.mousePosition))
                    SetTooltip(item, rect);
            }

            if (!rect.Contains(e.mousePosition))
                return;

            if (e.type == EventType.MouseDown && e.button == 0 && _dragItem == null && item != null)
            {
                StartDragFromStation(source);
                e.Use();
                return;
            }

            if (e.type == EventType.MouseUp && e.button == 0 && _dragItem != null && tryDrop != null)
            {
                if (tryDrop())
                {
                    e.Use();
                    return;
                }
            }
        }

        private void StartDragFromPocket(int index)
        {
            var slot = inventory.PocketsHotbar[index];
            if (!slot.HasItem) return;
            _dragSource = DragSource.PlayerPocket;
            _dragItem = slot.Item;
            _dragQuantity = slot.Quantity;
            _dragPocketIndex = index;
            slot.Clear();
            inventory.NotifyChanged();
        }

        private void StartDragFromGrid(PlacedItem placed)
        {
            if (placed == null) return;
            _dragSource = DragSource.PlayerGrid;
            _dragItem = placed.item;
            _dragQuantity = placed.quantity;
            _dragGridX = placed.x;
            _dragGridY = placed.y;
            _dragGridRotated = placed.rotated;
            inventory.SpatialGrid.RemovePlaced(placed);
            inventory.NotifyChanged();
        }

        private void StartDragFromStation(DragSource source)
        {
            ItemDefinition item = null;
            int quantity = 0;
            bool ok;

            switch (source)
            {
                case DragSource.CampfireInput:
                    ok = _station.TryTakeInputStack(out item, out quantity);
                    break;
                case DragSource.CampfireFuel:
                    ok = _station.TryTakeFuelStack(out item, out quantity);
                    break;
                case DragSource.CampfireOutput:
                    ok = _station.TryTakeOutputStack(out item, out quantity);
                    break;
                default:
                    ok = false;
                    break;
            }

            if (!ok)
                return;

            _dragSource = source;
            _dragItem = item;
            _dragQuantity = quantity;
        }

        private bool TryDropToStationInput()
        {
            if (_dragItem == null) return false;
            if (!_station.TryPlaceInputItem(_dragItem, _dragQuantity)) return false;
            ClearDrag();
            return true;
        }

        private bool TryDropToStationFuel()
        {
            if (_dragItem == null) return false;
            if (!_station.TryPlaceFuelItem(_dragItem, _dragQuantity)) return false;
            ClearDrag();
            return true;
        }

        private bool TryDropToPlayerInventory()
        {
            if (_dragItem == null) return false;
            if (!inventory.TryAddItem(_dragItem, _dragQuantity)) return false;
            inventory.NotifyChanged();
            ClearDrag();
            return true;
        }

        private void ReturnDragToSource()
        {
            if (_dragItem == null) return;

            switch (_dragSource)
            {
                case DragSource.PlayerPocket:
                    inventory.PocketsHotbar[_dragPocketIndex].TryAdd(_dragItem, _dragQuantity);
                    inventory.NotifyChanged();
                    break;
                case DragSource.PlayerGrid:
                    inventory.SpatialGrid.TryPlace(_dragItem, _dragGridX, _dragGridY, _dragGridRotated, _dragQuantity);
                    inventory.NotifyChanged();
                    break;
                case DragSource.CampfireInput:
                    _station.TryPlaceInputItem(_dragItem, _dragQuantity);
                    break;
                case DragSource.CampfireFuel:
                    _station.TryPlaceFuelItem(_dragItem, _dragQuantity);
                    break;
                case DragSource.CampfireOutput:
                    _station.TryPlaceOutputItem(_dragItem, _dragQuantity);
                    break;
            }

            ClearDrag();
        }

        private void ClearDrag()
        {
            _dragSource = DragSource.None;
            _dragItem = null;
            _dragQuantity = 0;
            _dragPocketIndex = -1;
            _dragGridX = 0;
            _dragGridY = 0;
            _dragGridRotated = false;
        }

        private void DrawEmptySlot(Rect rect)
        {
            GUI.color = new Color(0.15f, 0.15f, 0.16f, 1f);
            GUI.Box(rect, "");
            GUI.color = Color.white;
        }

        private void DrawItemCard(Rect rect, ItemDefinition item, int quantity, Color tint, bool translucent)
        {
            GUI.color = translucent ? tint : new Color(tint.r, tint.g, tint.b, 1f);
            GUI.Box(rect, "");
            GUI.color = Color.white;

            DrawSlotIcon(rect, item != null ? item.Icon : null);
            DrawQuantityBadge(rect, quantity);
            if (item != null && item.Icon == null)
                GUI.Label(new Rect(rect.x + 4f, rect.y + 4f, rect.width - 8f, rect.height - 8f), item.DisplayName);
        }

        private void DrawSlotIcon(Rect rect, Sprite icon)
        {
            if (icon == null) return;
            var texture = icon.texture;
            if (texture == null) return;
            Rect uv = icon.textureRect;
            uv = new Rect(uv.x / texture.width, uv.y / texture.height, uv.width / texture.width, uv.height / texture.height);
            var iconRect = new Rect(rect.x + 6f, rect.y + 6f, rect.width - 12f, rect.height - 12f);
            GUI.DrawTextureWithTexCoords(iconRect, texture, uv, true);
        }

        private void DrawQuantityBadge(Rect rect, int quantity)
        {
            if (quantity <= 1) return;
            GUI.Label(new Rect(rect.x + 4f, rect.yMax - 18f, rect.width - 8f, 16f), $"x{quantity}");
        }

        private void SetTooltip(ItemDefinition item, Rect anchor)
        {
            if (item == null) return;
            _tooltipItem = item;
            _tooltipAnchor = anchor;
        }

        private void DrawTooltip(Vector2 mousePosition)
        {
            if (_tooltipItem == null) return;

            string description = !string.IsNullOrWhiteSpace(_tooltipItem.ShortDescription)
                ? _tooltipItem.ShortDescription
                : $"Category: {_tooltipItem.Category}";
            string details = _tooltipItem.Stackable ? $"Stack: 1-{_tooltipItem.MaxStack}" : "Non-stackable";
            string tooltipText = $"<b>{_tooltipItem.DisplayName}</b>\n{description}\n{details}";

            float width = 260f;
            float height = 74f;
            float x = Mathf.Min(mousePosition.x + 18f, Screen.width - width - 8f);
            float y = mousePosition.y - height - 8f;
            if (y < 8f)
                y = Mathf.Min(_tooltipAnchor.yMax + 8f, Screen.height - height - 8f);

            var rect = new Rect(x, y, width, height);
            GUI.color = new Color(0.08f, 0.08f, 0.1f, 0.96f);
            GUI.Box(rect, "");
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, rect.height - 16f), tooltipText);
        }
    }
}
