using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int hotbarSize = 6;

        [Header("Spatial Backpack Grid")]
        [SerializeField] private int defaultGridWidth  = 3;
        [SerializeField] private int defaultGridHeight = 3;
        [SerializeField] private ItemDefinition starterBackpackItem;

        // --- Domain: BackpackGrid (spatial) ---
        private BackpackGrid _spatialGrid;

        // --- Domain: PocketsHotbar ---
        private List<InventorySlot> _pocketsHotbar;

        // --- Domain: Equipment ---
        private EquipmentSlot[] _equipmentSlots;

        public event Action OnInventoryChanged;

        // Domain properties
        public BackpackGrid SpatialGrid => _spatialGrid;
        public IReadOnlyList<InventorySlot> PocketsHotbar => _pocketsHotbar;
        public IReadOnlyList<EquipmentSlot> EquipmentSlots => _equipmentSlots;
        public int HotbarSize => hotbarSize;

        // Legacy alias (InventoryDebugPanel uses Hotbar)
        public IReadOnlyList<InventorySlot> Hotbar => _pocketsHotbar;

        // Legacy int property
        public int BackpackGridCapacity => _spatialGrid != null ? _spatialGrid.Width * _spatialGrid.Height : 0;

        private void Awake()
        {
            _pocketsHotbar  = CreateSlots(hotbarSize);
            _equipmentSlots = CreateEquipmentSlots();

            int gw = defaultGridWidth;
            int gh = defaultGridHeight;

            if (starterBackpackItem != null && starterBackpackItem.BackpackStorageWidth > 0)
            {
                var backpackSlot = GetEquipmentSlot(EquipmentSlotType.Backpack);
                if (backpackSlot != null && backpackSlot.CanAccept(starterBackpackItem))
                {
                    backpackSlot.TryEquip(starterBackpackItem);
                    gw = starterBackpackItem.BackpackStorageWidth;
                    gh = starterBackpackItem.BackpackStorageHeight;
                }
            }

            _spatialGrid = new BackpackGrid(Mathf.Max(1, gw), Mathf.Max(1, gh));
        }

        private List<InventorySlot> CreateSlots(int count)
        {
            count = Mathf.Max(1, count);
            var slots = new List<InventorySlot>(count);
            for (int i = 0; i < count; i++)
                slots.Add(new InventorySlot());
            return slots;
        }

        private EquipmentSlot[] CreateEquipmentSlots()
        {
            var types = (EquipmentSlotType[])Enum.GetValues(typeof(EquipmentSlotType));
            var slots = new EquipmentSlot[types.Length];
            for (int i = 0; i < types.Length; i++)
                slots[i] = new EquipmentSlot(types[i]);
            return slots;
        }

        // -------------------------------------------------------------------------
        // Equipment domain
        // -------------------------------------------------------------------------

        public EquipmentSlot GetEquipmentSlot(EquipmentSlotType slotType)
        {
            foreach (var slot in _equipmentSlots)
                if (slot.SlotType == slotType) return slot;
            return null;
        }

        public bool TryEquipItem(ItemDefinition item, EquipmentSlotType slotType)
        {
            if (item == null) return false;
            var slot = GetEquipmentSlot(slotType);
            if (slot == null) return false;
            if (!slot.TryEquip(item)) return false;
            NotifyChanged();
            return true;
        }

        public ItemDefinition UnequipItem(EquipmentSlotType slotType)
        {
            var slot = GetEquipmentSlot(slotType);
            if (slot == null) return null;
            var item = slot.Unequip();
            if (item != null) NotifyChanged();
            return item;
        }

        public bool EquipBackpack(ItemDefinition backpackItem)
        {
            if (backpackItem == null) return false;
            if (backpackItem.BackpackStorageWidth <= 0) return false;

            int newW = backpackItem.BackpackStorageWidth;
            int newH = backpackItem.BackpackStorageHeight > 0 ? backpackItem.BackpackStorageHeight : 1;

            var overflow = _spatialGrid.SimulateResize(newW, newH);
            if (overflow.Count > 0) return false;

            var backpackSlot = GetEquipmentSlot(EquipmentSlotType.Backpack);
            if (backpackSlot != null)
                backpackSlot.Clear();

            if (backpackSlot != null)
                backpackSlot.TryEquip(backpackItem);

            _spatialGrid.ResizeWithItems(newW, newH);
            NotifyChanged();
            return true;
        }

        // -------------------------------------------------------------------------
        // Storage domains (PocketsHotbar + SpatialGrid)
        // -------------------------------------------------------------------------

        public bool TryAddItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            int remaining = amount;

            // Step 1: merge into existing pocket stacks
            remaining = MergeToPockets(item, remaining);
            if (remaining <= 0) { NotifyChanged(); return true; }

            // Step 2: merge into existing grid stacks
            int merged = _spatialGrid.TryMergeAmount(item, remaining);
            remaining -= merged;
            if (remaining <= 0) { NotifyChanged(); return true; }

            // Step 3: place into empty pocket slots
            remaining = FillEmptyPockets(item, remaining);
            if (remaining <= 0) { NotifyChanged(); return true; }

            // Step 4: place new grid stacks
            if (_spatialGrid.TryPlaceFirstFit(item, remaining))
            {
                NotifyChanged();
                return true;
            }

            // Partial success: some went in, some didn't
            if (remaining < amount)
                NotifyChanged();

            return false;
        }

        private int MergeToPockets(ItemDefinition item, int amount)
        {
            int remaining = amount;
            foreach (var slot in _pocketsHotbar)
            {
                if (remaining <= 0) break;
                if (!slot.HasItem) continue;
                if (slot.Item != item) continue;
                remaining = slot.TryAdd(item, remaining);
            }
            return remaining;
        }

        private int FillEmptyPockets(ItemDefinition item, int amount)
        {
            int remaining = amount;
            foreach (var slot in _pocketsHotbar)
            {
                if (remaining <= 0) break;
                if (slot.HasItem) continue;
                if (!slot.CanAccept(item)) continue;
                remaining = slot.TryAdd(item, remaining);
            }
            return remaining;
        }

        public bool HasItems(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            return CountItem(item) >= amount;
        }

        public int CountItem(ItemDefinition item)
        {
            if (item == null) return 0;
            int total = 0;
            foreach (var slot in _pocketsHotbar)
                if (slot.Item == item) total += slot.Quantity;
            total += _spatialGrid.CountItem(item);
            return total;
        }

        public bool RemoveItems(ItemDefinition item, int amount)
        {
            if (!HasItems(item, amount)) return false;
            int remaining = amount;

            // Remove from pockets first
            foreach (var slot in _pocketsHotbar)
            {
                if (remaining <= 0) break;
                if (slot.Item != item) continue;
                remaining -= slot.Remove(remaining);
            }

            if (remaining > 0)
                _spatialGrid.RemoveAmount(item, remaining);

            NotifyChanged();
            return true;
        }

        public ItemDefinition GetHotbarItem(int index)
        {
            if (!TryGetHotbarSlot(index, out var slot) || !slot.HasItem) return null;
            return slot.Item;
        }

        public bool TryGetHotbarSlot(int index, out InventorySlot slot)
        {
            slot = null;
            if (_pocketsHotbar == null || index < 0 || index >= _pocketsHotbar.Count) return false;
            slot = _pocketsHotbar[index];
            return true;
        }

        public bool CanAddItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;

            int pocketCapacity = 0;
            foreach (var slot in _pocketsHotbar)
            {
                if (!slot.HasItem)
                    pocketCapacity += item.MaxStack;
                else if (slot.Item == item && item.Stackable)
                    pocketCapacity += item.MaxStack - slot.Quantity;
            }

            if (pocketCapacity >= amount) return true;

            int gridNeeds = amount - pocketCapacity;
            return _spatialGrid.CanFitAmount(item, gridNeeds);
        }

        public string GetInventoryDebugSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Equipment:");
            foreach (var slot in _equipmentSlots)
                sb.AppendLine($"  [{slot.SlotType}] {(slot.HasItem ? slot.Item.DisplayName : "Empty")}");
            sb.AppendLine("Hotbar:");
            for (int i = 0; i < _pocketsHotbar.Count; i++)
            {
                var slot = _pocketsHotbar[i];
                sb.AppendLine(slot.HasItem
                    ? $"  [{i}] {slot.Item.DisplayName} x{slot.Quantity}"
                    : $"  [{i}] Empty");
            }
            sb.AppendLine($"Backpack Grid ({_spatialGrid.Width}x{_spatialGrid.Height}):");
            foreach (var pi in _spatialGrid.GetAllPlaced())
                sb.AppendLine($"  ({pi.x},{pi.y}) {pi.item.DisplayName} x{pi.quantity}{(pi.rotated ? " [R]" : "")}");
            return sb.ToString();
        }

        public void NotifyChanged() => OnInventoryChanged?.Invoke();
    }
}
