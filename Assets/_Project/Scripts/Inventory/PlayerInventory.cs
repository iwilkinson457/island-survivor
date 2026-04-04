using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int backpackSize = 20;
        [SerializeField] private int hotbarSize = 6;

        private List<InventorySlot> _backpack;
        private List<InventorySlot> _hotbar;

        public event Action OnInventoryChanged;

        public IReadOnlyList<InventorySlot> Backpack => _backpack;
        public IReadOnlyList<InventorySlot> Hotbar => _hotbar;
        public int BackpackSize => backpackSize;
        public int HotbarSize => hotbarSize;

        private void Awake()
        {
            _backpack = CreateSlots(backpackSize);
            _hotbar = CreateSlots(hotbarSize);
        }

        private List<InventorySlot> CreateSlots(int count)
        {
            count = Mathf.Max(1, count);
            var slots = new List<InventorySlot>(count);
            for (int i = 0; i < count; i++)
                slots.Add(new InventorySlot());
            return slots;
        }

        public bool TryAddItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return false;
            int remaining = amount;

            remaining = AddToCollection(_hotbar, item, remaining, onlyExistingStacks: true);
            remaining = AddToCollection(_backpack, item, remaining, onlyExistingStacks: true);
            remaining = AddToCollection(_hotbar, item, remaining, onlyExistingStacks: false);
            remaining = AddToCollection(_backpack, item, remaining, onlyExistingStacks: false);

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
                if (onlyExistingStacks && !slot.HasItem) continue;
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
            CountInCollection(_hotbar, item, ref total);
            CountInCollection(_backpack, item, ref total);
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
            remaining = RemoveFromCollection(_hotbar, item, remaining);
            remaining = RemoveFromCollection(_backpack, item, remaining);
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
            if (_hotbar == null || index < 0 || index >= _hotbar.Count) return false;
            slot = _hotbar[index];
            return true;
        }

        public bool TryMoveToHotbar(int backpackIndex, int hotbarIndex)
        {
            if (_backpack == null || _hotbar == null) return false;
            if (backpackIndex < 0 || backpackIndex >= _backpack.Count) return false;
            if (hotbarIndex < 0 || hotbarIndex >= _hotbar.Count) return false;

            var backpackSlot = _backpack[backpackIndex];
            var hotbarSlot = _hotbar[hotbarIndex];
            var tempStack = hotbarSlot.Stack;
            hotbarSlot.Stack = backpackSlot.Stack;
            backpackSlot.Stack = tempStack;
            NotifyChanged();
            return true;
        }

        public string GetInventoryDebugSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Hotbar:");
            AppendCollection(sb, _hotbar);
            sb.AppendLine("Backpack:");
            AppendCollection(sb, _backpack);
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
            CalculateCapacity(_hotbar, item, ref capacity);
            CalculateCapacity(_backpack, item, ref capacity);
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
