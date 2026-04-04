using System;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    [Serializable]
    public class InventorySlot
    {
        public ItemStack Stack = new ItemStack();

        public bool HasItem => !Stack.IsEmpty;
        public ItemDefinition Item => Stack.item;
        public int Quantity => Stack.quantity;

        public bool CanAccept(ItemDefinition item)
        {
            if (item == null) return false;
            if (!HasItem) return true;
            return Stack.CanStackWith(item);
        }

        public int TryAdd(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return amount;

            if (!HasItem)
            {
                Stack.item = item;
                Stack.quantity = 0;
            }

            if (Stack.item != item)
                return amount;

            if (!item.Stackable && Stack.quantity > 0)
                return amount;

            return Stack.AddAmount(amount);
        }

        public int Remove(int amount) => Stack.RemoveAmount(amount);

        public void Clear()
        {
            Stack.item = null;
            Stack.quantity = 0;
        }
    }
}
