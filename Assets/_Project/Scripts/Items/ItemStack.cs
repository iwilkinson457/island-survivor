using System;

namespace ExtractionDeadIsles.Items
{
    [Serializable]
    public class ItemStack
    {
        public ItemDefinition item;
        public int quantity;

        public bool IsEmpty => item == null || quantity <= 0;
        public int SpaceRemaining => IsEmpty || item == null ? 0 : item.MaxStack - quantity;

        public bool CanStackWith(ItemDefinition other)
        {
            if (other == null) return false;
            if (IsEmpty) return true;
            return item == other && item.Stackable && quantity < item.MaxStack;
        }

        public int AddAmount(int amount)
        {
            if (amount <= 0) return 0;
            if (item == null) return amount;

            int add = Math.Min(SpaceRemaining, amount);
            quantity += add;
            return amount - add;
        }

        public int RemoveAmount(int amount)
        {
            if (amount <= 0 || IsEmpty) return 0;

            int removed = Math.Min(quantity, amount);
            quantity -= removed;
            if (quantity <= 0)
            {
                item = null;
                quantity = 0;
            }

            return removed;
        }
    }
}
