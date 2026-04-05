using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int backpackGridCapacity = 20;
        [SerializeField] private int hotbarSize = 6;

        // --- Domain: BackpackGrid ---
        private List<InventorySlot> _backpackGrid;

        // --- Domain: PocketsHotbar ---
        private List<InventorySlot> _pocketsHotbar;

        // --- Domain: Equipment ---
        private EquipmentSlot[] _equipmentSlots;

        public event Action OnInventoryChanged;

        // Domain-explicit properties
        public IReadOnlyList<InventorySlot> BackpackGrid => _backpackGrid;
        public IReadOnlyList<InventorySlot> PocketsHotbar => _pocketsHotbar;
        public IReadOnlyList<EquipmentSlot> EquipmentSlots => _equipmentSlots;
        public int BackpackGridCapacity => backpackGridCapacity;
        public int HotbarSize => hotbarSize;

        // Legacy-compatible aliases (used by InventoryDebugPanel and other callers)
        public IReadOnlyList<InventorySlot> Backpack => _backpackGrid;
        public IReadOnlyList<InventorySlot> Hotbar => _pocketsHotbar;
        public int BackpackSize => backpackGridCapacity;

        private void Awake()
        {
            _pocketsHotbar = CreateSlots(hotbarSize);
            _backpackGrid = CreateSlots(backpackGridCapacity);
            _equipmentSlots = CreateEquipmentSlots();
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

        /// <summary>
        /// Equips an item into the specified equipment slot.
        /// Returns false if the slot rejects the item (wrong type or already occupied).
        /// Does NOT remove the item from storage — caller is responsible for that.
        /// </summary>
        public bool TryEquipItem(ItemDefinition item, EquipmentSlotType slotType)
        {
            if (item == null) return false;
            var slot = GetEquipmentSlot(slotType);
            if (slot == null) return false;
            if (!slot.TryEquip(item)) return false;
            NotifyChanged();
            return true;
        }

        /// <summary>
        /// Removes and returns the item in the given equipment slot, or null if empty.
        /// Does NOT add the item back to storage — caller is responsible for that.
        /// </summary>
        public ItemDefinition UnequipItem(EquipmentSlotType slotType)
        {
            var slot = GetEquipmentSlot(slotType);
            if (slot == null) return null;
            var item = slot.Unequip();
            if (item != null) NotifyChanged();
            return item;
        }

        // -------------------------------------------------------------------------
        // Storage domains (PocketsHotbar + BackpackGrid)
        // -------------------------------------------------------------------------

        public bool TryAddItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            int remaining = amount;

            remaining = AddToCollection(_pocketsHotbar, item, remaining, onlyExistingStacks: true);
            remaining = AddToCollection(_backpackGrid, item, remaining, onlyExistingStacks: true);
            remaining = AddToCollection(_pocketsHotbar, item, remaining, onlyExistingStacks: false);
            remaining = AddToCollection(_backpackGrid, item, remaining, onlyExistingStacks: false);

            if (remaining != amount)
                NotifyChanged();

            return remaining == 0;
        }

        private int AddToCollection(List<InventorySlot> slots, ItemDefinition item, int amount, bool onlyExistingStacks)
        {
            int remaining = amount;
            foreach (var slot in slots)
            {
                if (remaining <= 0) break;

                if (onlyExistingStacks)
                {
                    if (!slot.HasItem) continue;
                    if (slot.Item != item) continue;
                }

                if (!slot.CanAccept(item)) continue;
                remaining = slot.TryAdd(item, remaining);
            }
            return remaining;
        }

        public bool HasItems(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            int found = CountItem(item);
            return found >= amount;
        }

        public int CountItem(ItemDefinition item)
        {
            if (item == null) return 0;
            int total = 0;
            CountInCollection(_pocketsHotbar, item, ref total);
            CountInCollection(_backpackGrid, item, ref total);
            return total;
        }

        private void CountInCollection(List<InventorySlot> slots, ItemDefinition item, ref int total)
        {
            foreach (var slot in slots)
                if (slot.Item == item)
                    total += slot.Quantity;
        }

        public bool RemoveItems(ItemDefinition item, int amount)
        {
            if (!HasItems(item, amount)) return false;

            int remaining = amount;
            remaining = RemoveFromCollection(_pocketsHotbar, item, remaining);
            remaining = RemoveFromCollection(_backpackGrid, item, remaining);
            NotifyChanged();
            return remaining <= 0;
        }

        private int RemoveFromCollection(List<InventorySlot> slots, ItemDefinition item, int amount)
        {
            int remaining = amount;
            foreach (var slot in slots)
            {
                if (remaining <= 0) break;
                if (slot.Item != item) continue;
                remaining -= slot.Remove(remaining);
            }
            return remaining;
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

        public bool TryMoveToHotbar(int backpackIndex, int hotbarIndex)
        {
            if (_backpackGrid == null || _pocketsHotbar == null) return false;
            if (backpackIndex < 0 || backpackIndex >= _backpackGrid.Count) return false;
            if (hotbarIndex < 0 || hotbarIndex >= _pocketsHotbar.Count) return false;

            var backpackSlot = _backpackGrid[backpackIndex];
            var hotbarSlot = _pocketsHotbar[hotbarIndex];
            var tempStack = hotbarSlot.Stack;
            hotbarSlot.Stack = backpackSlot.Stack;
            backpackSlot.Stack = tempStack;
            NotifyChanged();
            return true;
        }

        public string GetInventoryDebugSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Equipment:");
            foreach (var slot in _equipmentSlots)
                sb.AppendLine($"  [{slot.SlotType}] {(slot.HasItem ? slot.Item.DisplayName : "Empty")}");
            sb.AppendLine("Hotbar:");
            AppendCollection(sb, _pocketsHotbar);
            sb.AppendLine("Backpack Grid:");
            AppendCollection(sb, _backpackGrid);
            return sb.ToString();
        }

        private void AppendCollection(StringBuilder sb, List<InventorySlot> slots)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (!slot.HasItem)
                    sb.AppendLine($"[{i}] Empty");
                else
                    sb.AppendLine($"[{i}] {slot.Item.DisplayName} x{slot.Quantity}");
            }
        }

        public bool CanAddItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            int capacity = 0;
            CalculateCapacity(_pocketsHotbar, item, ref capacity);
            CalculateCapacity(_backpackGrid, item, ref capacity);
            return capacity >= amount;
        }

        private void CalculateCapacity(List<InventorySlot> slots, ItemDefinition item, ref int capacity)
        {
            foreach (var slot in slots)
            {
                if (!slot.HasItem)
                    capacity += item.MaxStack;
                else if (slot.Item == item && item.Stackable)
                    capacity += item.MaxStack - slot.Quantity;
            }
        }

        public void NotifyChanged() => OnInventoryChanged?.Invoke();
    }
}
