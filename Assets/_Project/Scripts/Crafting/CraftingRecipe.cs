using System.Collections.Generic;
using UnityEngine;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Crafting
{
    [CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Extraction Dead Isles/Crafting/Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [SerializeField] private string recipeId;
        [SerializeField] private string displayName;
        [SerializeField] private ItemDefinition outputItem;
        [SerializeField] private int outputAmount = 1;
        [SerializeField] private List<CraftingIngredient> ingredients = new();
        [SerializeField] private bool requiresCampfire;
        [SerializeField] private bool unlockedByDefault = true;

        public string RecipeId => string.IsNullOrWhiteSpace(recipeId) ? name.ToLowerInvariant().Replace(' ', '_') : recipeId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public ItemDefinition OutputItem => outputItem;
        public int OutputAmount => Mathf.Max(1, outputAmount);
        public IReadOnlyList<CraftingIngredient> Ingredients => ingredients;
        public bool RequiresCampfire => requiresCampfire;
        public bool UnlockedByDefault => unlockedByDefault;
    }
}
