using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Player;

namespace ExtractionDeadIsles.Items
{
    public static class QuickConsumeHelper
    {
        public static bool TryConsumeHotbarSlot(PlayerInventory inventory, PlayerStats stats, int hotbarIndex)
        {
            if (inventory == null || stats == null) return false;
            if (!inventory.TryGetHotbarSlot(hotbarIndex, out var slot) || !slot.HasItem) return false;

            var item = slot.Item;
            if (item == null) return false;
            if (item.HungerRestore <= 0 && item.ThirstRestore <= 0) return false;

            stats.Consume(item);
            slot.Remove(1);
            inventory.NotifyChanged();
            return true;
        }
    }
}
