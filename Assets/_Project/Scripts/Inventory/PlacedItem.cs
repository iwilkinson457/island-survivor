using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    /// <summary>
    /// Represents an item occupying one or more cells in a <see cref="BackpackGrid"/>.
    /// Instances are created and owned by BackpackGrid; do not create directly.
    /// </summary>
    public class PlacedItem
    {
        /// <summary>The item definition placed in the grid.</summary>
        public ItemDefinition Item { get; }

        /// <summary>Top-left column of the item's footprint.</summary>
        public int X { get; internal set; }

        /// <summary>Top-left row of the item's footprint.</summary>
        public int Y { get; internal set; }

        /// <summary>Whether the item's footprint is rotated 90 degrees (width/height swapped).</summary>
        public bool Rotated { get; internal set; }

        /// <summary>Effective width in grid cells (accounts for rotation).</summary>
        public int EffectiveWidth  => Rotated ? Item.GridHeight : Item.GridWidth;

        /// <summary>Effective height in grid cells (accounts for rotation).</summary>
        public int EffectiveHeight => Rotated ? Item.GridWidth  : Item.GridHeight;

        internal PlacedItem(ItemDefinition item, int x, int y, bool rotated)
        {
            Item    = item;
            X       = x;
            Y       = y;
            Rotated = rotated;
        }
    }
}
