using System.Collections.Generic;
using UnityEngine;
using ExtractionDeadIsles.Inventory;

namespace ExtractionDeadIsles.Crafting
{
    public class PlayerCrafter : MonoBehaviour
    {
        [SerializeField] private List<CraftingRecipe> defaultRecipes = new();
        [SerializeField] private PlayerInventory inventory;

        public event System.Action<Items.ItemDefinition> OnCraftedPlaceable;

        public IReadOnlyList<CraftingRecipe> GetRecipes() => defaultRecipes;

        public bool CanCraft(CraftingRecipe recipe, bool nearCampfire)
        {
            if (recipe == null || inventory == null || recipe.OutputItem == null) return false;
            if (recipe.RequiresCampfire && !nearCampfire) return false;

            // Placeables go directly to placement, not inventory — skip storage/equip capacity check
            if (!recipe.OutputItem.IsPlaceable)
            {
                if (!inventory.CanReceiveCraftedItem(recipe.OutputItem, recipe.OutputAmount)) return false;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                if (ingredient.item == null || ingredient.amount <= 0) return false;
                if (!inventory.HasItems(ingredient.item, ingredient.amount)) return false;
            }

            return true;
        }

        public bool TryCraft(CraftingRecipe recipe, bool nearCampfire)
        {
            if (!CanCraft(recipe, nearCampfire)) return false;

            foreach (var ingredient in recipe.Ingredients)
                inventory.RemoveItems(ingredient.item, ingredient.amount);

            if (recipe.OutputItem.IsPlaceable)
            {
                Debug.Log($"[PlayerCrafter] Crafted placeable {recipe.OutputItem.DisplayName} — entering placement mode.");
                OnCraftedPlaceable?.Invoke(recipe.OutputItem);
                return true;
            }

            if (!inventory.TryReceiveCraftedItem(recipe.OutputItem, recipe.OutputAmount, out var result))
            {
                foreach (var ingredient in recipe.Ingredients)
                    inventory.TryAddItem(ingredient.item, ingredient.amount);
                return false;
            }

            Debug.Log($"[PlayerCrafter] {result} x{recipe.OutputAmount}");
            return true;
        }

        private void Reset()
        {
            inventory = GetComponent<PlayerInventory>();
        }
    }
}
