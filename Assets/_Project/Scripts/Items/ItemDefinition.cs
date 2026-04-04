using UnityEngine;

namespace ExtractionDeadIsles.Items
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Extraction Dead Isles/Items/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private ItemCategory category;
        [SerializeField] private bool stackable = true;
        [SerializeField] private int maxStack = 20;
        [SerializeField] private bool isCookable;
        [SerializeField] private string cookedResultItemId;
        [SerializeField] private bool isPlaceable;
        [SerializeField] private string placeablePrefabId;
        [SerializeField] private int hungerRestore;
        [SerializeField] private int thirstRestore;
        [SerializeField] private float gatherToolMultiplier = 1f;

        public string ItemId => itemId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public ItemCategory Category => category;
        public bool Stackable => stackable;
        public int MaxStack => maxStack;
        public bool IsCookable => isCookable;
        public string CookedResultItemId => cookedResultItemId;
        public bool IsPlaceable => isPlaceable;
        public string PlaceablePrefabId => placeablePrefabId;
        public int HungerRestore => hungerRestore;
        public int ThirstRestore => thirstRestore;
        public float GatherToolMultiplier => gatherToolMultiplier;

        private void OnValidate()
        {
            if (!stackable)
                maxStack = 1;

            maxStack = Mathf.Max(1, maxStack);
            gatherToolMultiplier = Mathf.Max(0.1f, gatherToolMultiplier);

            if (string.IsNullOrWhiteSpace(itemId))
                itemId = name.ToLowerInvariant().Replace(' ', '_');
        }
    }
}
