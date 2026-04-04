using System.Collections.Generic;
using UnityEngine;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.World
{
    public class StarterLoadoutBootstrap : MonoBehaviour
    {
        [System.Serializable]
        private class StartingItem
        {
            public ItemDefinition item;
            public int amount = 1;
        }

        [SerializeField] private bool grantOnStart;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private List<StartingItem> startingItems = new();

        private void Start()
        {
            if (!grantOnStart || inventory == null) return;

            foreach (var entry in startingItems)
            {
                if (entry.item != null && entry.amount > 0)
                    inventory.TryAddItem(entry.item, entry.amount);
            }

            enabled = false;
        }

        private void Reset()
        {
            inventory = GetComponent<PlayerInventory>();
        }
    }
}
