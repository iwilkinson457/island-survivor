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
        [SerializeField] private PlayerInventory inventory;

        private CampfireStation _station;
        private bool _visible;
        private Vector2 _inventoryScroll;

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
            _visible = false;
            _station = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnGUI()
        {
            if (!_visible || _station == null || inventory == null) return;

            var panelRect = new Rect(48f, 36f, Screen.width - 96f, Screen.height - 72f);
            GUI.Box(panelRect, "Campfire");

            float leftW = Mathf.Max(360f, panelRect.width * 0.52f);
            float rightW = panelRect.width - leftW - 24f;
            var leftRect = new Rect(panelRect.x + 16f, panelRect.y + 40f, leftW - 16f, panelRect.height - 56f);
            var rightRect = new Rect(leftRect.xMax + 24f, panelRect.y + 40f, rightW - 16f, panelRect.height - 56f);

            DrawPlayerInventory(leftRect);
            DrawStation(rightRect);

            if (GUI.Button(new Rect(panelRect.xMax - 120f, panelRect.y + 8f, 96f, 28f), "Close"))
                Close();
        }

        private void DrawPlayerInventory(Rect rect)
        {
            GUI.Box(rect, "");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, 24f), "<b>Player Inventory</b>");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 28f, rect.width - 20f, 20f), "Click raw fish to move to Input. Click fuel items to move to Fuel.");

            var items = BuildInventorySummary();
            float viewHeight = Mathf.Max(rect.height - 62f, items.Count * 34f + 12f);
            var viewRect = new Rect(0f, 0f, rect.width - 36f, viewHeight);
            var contentRect = new Rect(rect.x + 8f, rect.y + 56f, rect.width - 16f, rect.height - 64f);
            _inventoryScroll = GUI.BeginScrollView(contentRect, _inventoryScroll, viewRect);

            float y = 4f;
            foreach (var entry in items)
            {
                var item = entry.Key;
                int count = entry.Value;
                string tag = _station.CanAcceptInput(item) ? "Cook" : (_station.CanAcceptFuel(item) ? "Fuel" : "Stored");
                if (GUI.Button(new Rect(4f, y, viewRect.width - 8f, 28f), $"{item.DisplayName} x{count} [{tag}]"))
                {
                    if (_station.CanAcceptInput(item))
                        _station.TryMoveInputFromPlayer(inventory, item, 1);
                    else if (_station.CanAcceptFuel(item))
                        _station.TryMoveFuelFromPlayer(inventory, item, 1);
                }
                y += 32f;
            }

            if (items.Count == 0)
                GUI.Label(new Rect(8f, 8f, viewRect.width - 16f, 24f), "Inventory empty.");

            GUI.EndScrollView();
        }

        private Dictionary<ItemDefinition, int> BuildInventorySummary()
        {
            var map = new Dictionary<ItemDefinition, int>();

            foreach (var slot in inventory.PocketsHotbar)
            {
                if (!slot.HasItem) continue;
                map.TryGetValue(slot.Item, out int count);
                map[slot.Item] = count + slot.Quantity;
            }

            foreach (var placed in inventory.SpatialGrid.GetAllPlaced())
            {
                if (placed == null || placed.item == null || placed.quantity <= 0) continue;
                map.TryGetValue(placed.item, out int count);
                map[placed.item] = count + placed.quantity;
            }

            return map;
        }

        private void DrawStation(Rect rect)
        {
            GUI.Box(rect, "");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, 24f), "<b>Campfire Station</b>");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 30f, rect.width - 20f, 20f), _station.IsBurning ? "State: Burning" : "State: Unlit");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 50f, rect.width - 20f, 20f), $"Fuel Time: {_station.CurrentFuelSecondsRemaining:0.0}s");
            GUI.Label(new Rect(rect.x + 10f, rect.y + 70f, rect.width - 20f, 20f), $"Cook Progress: {_station.CurrentCookProgress:0.0}/{_station.CookDurationSeconds:0.0}s");

            float boxW = rect.width - 20f;
            float slotY = rect.y + 110f;
            DrawStationSlot(new Rect(rect.x + 10f, slotY, boxW, 70f), "Input", _station.InputItem, _station.InputQuantity, () => _station.TryReturnInputToPlayer(inventory));
            DrawStationSlot(new Rect(rect.x + 10f, slotY + 82f, boxW, 70f), "Fuel", _station.FuelItem, _station.FuelQuantity, () => _station.TryReturnFuelToPlayer(inventory));
            DrawStationSlot(new Rect(rect.x + 10f, slotY + 164f, boxW, 70f), "Output", _station.OutputItem, _station.OutputQuantity, () => _station.TryTakeOutputToPlayer(inventory));

            string lightLabel = _station.IsBurning ? "Burning" : "Light Fire";
            GUI.enabled = !_station.IsBurning && _station.CanLight;
            if (GUI.Button(new Rect(rect.x + 10f, slotY + 250f, 140f, 34f), lightLabel))
                _station.TryLight();
            GUI.enabled = true;

            GUI.Label(new Rect(rect.x + 10f, slotY + 292f, rect.width - 20f, 44f),
                "Cooking only progresses while fuel is burning. Output must have room for cooked fish.");
        }

        private void DrawStationSlot(Rect rect, string label, ItemDefinition item, int quantity, System.Func<bool> onClick)
        {
            string itemText = item != null ? $"{item.DisplayName} x{quantity}" : "Empty";
            if (GUI.Button(rect, $"{label}\n{itemText}"))
                onClick?.Invoke();
        }
    }
}
