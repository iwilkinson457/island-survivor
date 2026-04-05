using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Inventory
{
    public class PlacedItem
    {
        public ItemDefinition item;
        public int quantity;
        public int x, y;
        public bool rotated;

        public int EffectiveWidth  => rotated ? item.GridHeight : item.GridWidth;
        public int EffectiveHeight => rotated ? item.GridWidth  : item.GridHeight;

        // Legacy property aliases used by BackpackGrid internals
        public ItemDefinition Item    => item;
        public int X                  { get { return x; } internal set { x = value; } }
        public int Y                  { get { return y; } internal set { y = value; } }
        public bool Rotated           { get { return rotated; } internal set { rotated = value; } }

        public PlacedItem(ItemDefinition itemDef, int col, int row, bool isRotated, int qty = 1)
        {
            item     = itemDef;
            x        = col;
            y        = row;
            rotated  = isRotated;
            quantity = qty;
        }
    }
}
