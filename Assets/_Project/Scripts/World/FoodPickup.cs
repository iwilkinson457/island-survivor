using UnityEngine;
using ExtractionDeadIsles.Interaction;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.World
{
    public class FoodPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemDefinition foodItem;
        [SerializeField] private int amount = 1;

        public string InteractionPrompt => $"Pick up {(foodItem != null ? foodItem.DisplayName : "Food")}";

        public void Interact(GameObject interactor)
        {
            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null || foodItem == null) return;

            if (!inventory.TryAddItem(foodItem, amount))
            {
                Debug.Log($"[FoodPickup] Inventory full. Could not pick up {foodItem.DisplayName}.");
                return;
            }

            Debug.Log($"[FoodPickup] Picked up {foodItem.DisplayName} x{amount}");
            Destroy(gameObject);
        }
    }
}
