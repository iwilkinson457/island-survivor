using System;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    /// <summary>
    /// A single typed equipment slot. Accepts only items whose ItemDefinition declares
    /// compatibility with this slot's EquipmentSlotType.
    /// Equipment slots hold exactly one non-stacked item.
    /// </summary>
    [Serializable]
    public class EquipmentSlot
    {
        public EquipmentSlotType SlotType;
        public ItemStack Stack = new ItemStack();

        public bool HasItem => !Stack.IsEmpty;
        public ItemDefinition Item => Stack.item;

        public EquipmentSlot(EquipmentSlotType slotType)
        {
            SlotType = slotType;
        }

        /// <summary>Returns true if the item is compatible with this slot and the slot is empty.</summary>
        public bool CanAccept(ItemDefinition item)
        {
            if (item == null) return false;
            if (HasItem) return false;
            return item.IsCompatibleWithEquipmentSlot(SlotType);
        }

        /// <summary>Equips the item into this slot. Returns false if the slot rejects it.</summary>
        public bool TryEquip(ItemDefinition item)
        {
            if (!CanAccept(item)) return false;
            Stack.item = item;
            Stack.quantity = 1;
            return true;
        }

        /// <summary>Removes and returns the equipped item, or null if the slot was empty.</summary>
        public ItemDefinition Unequip()
        {
            if (!HasItem) return null;
            var item = Stack.item;
            Stack.item = null;
            Stack.quantity = 0;
            return item;
        }

        public void Clear()
        {
            Stack.item = null;
            Stack.quantity = 0;
        }
    }
}
