using UnityEngine;
using ExtractionDeadIsles.Inventory;

namespace ExtractionDeadIsles.Items
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Extraction Dead Isles/Items/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [TextArea(2, 4)] [SerializeField] private string shortDescription;
        [SerializeField] private Sprite icon;
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

        [Header("Equipment")]
        [SerializeField] private EquipmentSlotType[] compatibleEquipmentSlots = {};

        [Header("Backpack Storage (if this item equips into the Backpack slot)")]
        [SerializeField] private int backpackStorageWidth;
        [SerializeField] private int backpackStorageHeight;

        [Header("BackpackGrid Footprint")]
        [SerializeField] private int gridWidth = 1;
        [SerializeField] private int gridHeight = 1;
        [SerializeField] private bool rotatable;

        public string ItemId => itemId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string ShortDescription => shortDescription;
        public Sprite Icon => icon;
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
        public EquipmentSlotType[] CompatibleEquipmentSlots => compatibleEquipmentSlots;
        public int BackpackStorageWidth => backpackStorageWidth;
        public int BackpackStorageHeight => backpackStorageHeight;
        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public bool Rotatable => rotatable;

        /// <summary>Returns true if this item can be placed in the given equipment slot type.</summary>
        public bool IsCompatibleWithEquipmentSlot(EquipmentSlotType slotType)
        {
            if (compatibleEquipmentSlots == null || compatibleEquipmentSlots.Length == 0) return false;
            foreach (var s in compatibleEquipmentSlots)
                if (s == slotType) return true;
            return false;
        }

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
