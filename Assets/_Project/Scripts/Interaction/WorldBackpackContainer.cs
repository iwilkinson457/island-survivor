using System.Collections.Generic;
using UnityEngine;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Interaction
{
    public class WorldBackpackContainer : MonoBehaviour, IInteractable
    {
        [System.Serializable]
        private class StoredEntry
        {
            public ItemDefinition item;
            public int quantity;

            public StoredEntry(ItemDefinition item, int quantity)
            {
                this.item = item;
                this.quantity = quantity;
            }
        }

        [SerializeField] private ItemDefinition backpackItem;
        [SerializeField] private List<StoredEntry> storedItems = new();

        public string InteractionPrompt
        {
            get
            {
                int stackCount = storedItems.Count;
                string bagName = backpackItem != null ? backpackItem.DisplayName : "Backpack";
                return $"Loot {bagName} cache ({stackCount} stack{(stackCount == 1 ? string.Empty : "s")})";
            }
        }

        public void Initialize(ItemDefinition sourceBackpack, IReadOnlyList<PlacedItem> placedItems)
        {
            backpackItem = sourceBackpack;
            storedItems.Clear();

            if (placedItems == null) return;
            foreach (var placed in placedItems)
            {
                if (placed?.item == null || placed.quantity <= 0) continue;
                storedItems.Add(new StoredEntry(placed.item, placed.quantity));
            }
        }

        public void Interact(GameObject interactor)
        {
            var inventory = interactor.GetComponent<PlayerInventory>()
                         ?? interactor.GetComponentInParent<PlayerInventory>();
            if (inventory == null) return;

            bool changed = false;

            for (int i = storedItems.Count - 1; i >= 0; i--)
            {
                var entry = storedItems[i];
                if (entry?.item == null || entry.quantity <= 0)
                {
                    storedItems.RemoveAt(i);
                    continue;
                }

                if (!inventory.CanAddItem(entry.item, entry.quantity))
                    continue;

                if (!inventory.TryAddItem(entry.item, entry.quantity))
                    continue;

                storedItems.RemoveAt(i);
                changed = true;
            }

            if (backpackItem != null)
            {
                if (inventory.TryReceiveWorldItem(backpackItem, 1, out _))
                {
                    backpackItem = null;
                    changed = true;
                }
            }

            if (!changed)
            {
                Debug.Log("[WorldBackpackContainer] No space to loot backpack cache.");
                return;
            }

            if (backpackItem == null && storedItems.Count == 0)
                Destroy(gameObject);
        }
    }
}
