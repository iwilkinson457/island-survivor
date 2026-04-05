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

        private struct PlayerViewEntry
        {
            public ItemDefinition item;
            public int quantity;
            public bool fromPocket;
            public int pocketIndex;
            public int gridX;
            public int gridY;
            public bool gridRotated;
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

        private const float PANEL_MARGIN_X = 48f;
        private const float PANEL_MARGIN_Y = 36f;
        private const float PANEL_PADDING = 20f;
        private const float SLOT_SIZE = 92f;
        private const float SLOT_GAP = 8f;

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
            float leftW = Mathf.Max(420f, innerW * 0.58f);
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
                DrawItemCard(dragRect, _dragItem, _dragQuantity, new Color(1f, 1f, 1f, 0.85f));
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
            var entries = BuildPlayerEntries();
            int columns = Mathf.Max(4, Mathf.FloorToInt((rect.width - 16f) / (SLOT_SIZE + SLOT_GAP)));
            float startX = rect.x + 8f;
            float startY = rect.y + 28f;

            GUI.Label(new Rect(rect.x + 8f, rect.y + 6f, rect.width - 16f, 20f), "Drag raw fish to Input, fuel items to Fuel, or drag finished output back here.");

            for (int i = 0; i < entries.Count; i++)
            {
                int col = i % columns;
                int row = i / columns;
                Rect slotRect = new Rect(startX + col * (SLOT_SIZE + SLOT_GAP), startY + row * (SLOT_SIZE + SLOT_GAP), SLOT_SIZE, SLOT_SIZE);
                var entry = entries[i];
                DrawItemCard(slotRect, entry.item, entry.quantity, new Color(0.65f, 0.82f, 1f));
                if (slotRect.Contains(e.mousePosition))
                    SetTooltip(entry.item, slotRect);

                if (e.type == EventType.MouseDown && e.button == 0 && _dragItem == null && slotRect.Contains(e.mousePosition))
                {
                    if (entry.fromPocket)
                        StartDragFromPocket(entry.pocketIndex);
                    else
                        StartDragFromGrid(entry.gridX, entry.gridY, entry.gridRotated);
                    e.Use();
                    return;
                }

                if (e.type == EventType.MouseUp && e.button == 0 && _dragItem != null && slotRect.Contains(e.mousePosition))
                {
                    if (TryDropToPlayerInventory())
                    {
                        e.Use();
                        return;
                    }
                }
            }

            if (entries.Count == 0)
                GUI.Label(new Rect(rect.x + 10f, rect.y + 34f, rect.width - 20f, 24f), "Inventory empty.");
        }

        private void DrawStation(Rect rect, Event e)
        {
            GUI.Box(rect, "");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, 20f), _station.IsBurning ? "State: Burning" : "State: Unlit");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 28f, rect.width - 20f, 20f), $"Fuel Time: {_station.CurrentFuelSecondsRemaining:0.0}s");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 48f, rect.width - 20f, 20f), $"Cook Progress: {_station.CurrentCookProgress:0.0}/{_station.CookDurationSeconds:0.0}s");

            float slotX = rect.x + 10f;
            float slotY = rect.y + 86f;
            float labelW = rect.width - 20f;
            GUI.Label(new Rect(slotX, slotY, labelW, 18f), "Input");
            Rect inputRect = new Rect(slotX, slotY + 20f, SLOT_SIZE, SLOT_SIZE);
            DrawStationSlot(inputRect, _station.InputItem, _station.InputQuantity, new Color(0.66f, 0.84f, 1f));

            GUI.Label(new Rect(slotX + 120f, slotY, labelW, 18f), "Fuel");
            Rect fuelRect = new Rect(slotX + 120f, slotY + 20f, SLOT_SIZE, SLOT_SIZE);
            DrawStationSlot(fuelRect, _station.FuelItem, _station.FuelQuantity, new Color(1f, 0.86f, 0.55f));

            GUI.Label(new Rect(slotX + 240f, slotY, labelW, 18f), "Output");
            Rect outputRect = new Rect(slotX + 240f, slotY + 20f, SLOT_SIZE, SLOT_SIZE);
            DrawStationSlot(outputRect, _station.OutputItem, _station.OutputQuantity, new Color(0.7f, 1f, 0.7f));

            HandleStationSlotEvents(e, inputRect, DragSource.CampfireInput, _station.InputItem, () => StartDragFromStation(DragSource.CampfireInput), () => TryDropToStationInput());
            HandleStationSlotEvents(e, fuelRect, DragSource.CampfireFuel, _station.FuelItem, () => StartDragFromStation(DragSource.CampfireFuel), () => TryDropToStationFuel());
            HandleStationSlotEvents(e, outputRect, DragSource.CampfireOutput, _station.OutputItem, () => StartDragFromStation(DragSource.CampfireOutput), () => false);

            string lightLabel = _station.IsBurning ? "Burning" : "Light Fire";
            GUI.enabled = !_station.IsBurning && _station.CanLight;
            if (GUI.Button(new Rect(slotX, slotY + 140f, 140f, 34f), lightLabel))
                _station.TryLight();
            GUI.enabled = true;

            GUI.Label(new Rect(slotX, slotY + 182f, rect.width - 20f, 60f),
                "Use the same drag/drop idea as inventory: drag raw fish into Input, drag sticks into Fuel, then drag cooked fish out of Output.");
        }

        private void HandleStationSlotEvents(Event e, Rect rect, DragSource slotSource, ItemDefinition item, System.Action startDrag, System.Func<bool> tryDrop)
        {
            if (item != null && rect.Contains(e.mousePosition))
                SetTooltip(item, rect);

            if (e.type == EventType.MouseDown && e.button == 0 && _dragItem == null && item != null && rect.Contains(e.mousePosition))
            {
                startDrag?.Invoke();
                e.Use();
                return;
            }

            if (e.type == EventType.MouseUp && e.button == 0 && _dragItem != null && rect.Contains(e.mousePosition))
            {
                if (tryDrop())
                {
                    e.Use();
                }
            }
        }

        private List<PlayerViewEntry> BuildPlayerEntries()
        {
            var entries = new List<PlayerViewEntry>();

            for (int i = 0; i < inventory.PocketsHotbar.Count; i++)
            {
                var slot = inventory.PocketsHotbar[i];
                if (!slot.HasItem) continue;
                entries.Add(new PlayerViewEntry
                {
                    item = slot.Item,
                    quantity = slot.Quantity,
                    fromPocket = true,
                    pocketIndex = i
                });
            }

            foreach (var placed in inventory.SpatialGrid.GetAllPlaced())
            {
                if (placed == null || placed.item == null || placed.quantity <= 0) continue;
                entries.Add(new PlayerViewEntry
                {
                    item = placed.item,
                    quantity = placed.quantity,
                    fromPocket = false,
                    gridX = placed.x,
                    gridY = placed.y,
                    gridRotated = placed.rotated
                });
            }

            return entries;
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

        private void StartDragFromGrid(int x, int y, bool rotated)
        {
            var placed = inventory.SpatialGrid.GetItemAt(x, y);
            if (placed == null) return;
            _dragSource = DragSource.PlayerGrid;
            _dragItem = placed.item;
            _dragQuantity = placed.quantity;
            _dragGridX = placed.x;
            _dragGridY = placed.y;
            _dragGridRotated = rotated;
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

        private void DrawStationSlot(Rect rect, ItemDefinition item, int quantity, Color tint)
        {
            GUI.color = tint;
            GUI.Box(rect, "");
            GUI.color = Color.white;

            if (item != null)
            {
                DrawSlotIcon(rect, item.Icon);
                DrawQuantityBadge(rect, quantity);
                if (item.Icon == null)
                    GUI.Label(new Rect(rect.x + 4f, rect.y + 4f, rect.width - 8f, rect.height - 8f), item.DisplayName);
            }
        }

        private void DrawItemCard(Rect rect, ItemDefinition item, int quantity, Color tint)
        {
            GUI.color = tint;
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
