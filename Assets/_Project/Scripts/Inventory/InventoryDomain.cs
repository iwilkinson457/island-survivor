namespace ExtractionDeadIsles.Inventory
{
    /// <summary>
    /// The three explicit inventory domains that make up the player's item-carrying system.
    /// Equipment holds actively worn/wielded gear in typed slots.
    /// PocketsHotbar holds 6 fixed quick-access slots.
    /// BackpackGrid holds spatial 2D storage sized by the equipped backpack.
    /// </summary>
    public enum InventoryDomain
    {
        Equipment,
        PocketsHotbar,
        BackpackGrid
    }
}
