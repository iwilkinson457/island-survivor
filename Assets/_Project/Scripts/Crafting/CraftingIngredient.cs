using System;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Crafting
{
    [Serializable]
    public class CraftingIngredient
    {
        public ItemDefinition item;
        public int amount = 1;
    }
}
