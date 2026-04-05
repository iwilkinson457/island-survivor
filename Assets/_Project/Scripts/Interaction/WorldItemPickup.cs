using UnityEngine;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Interaction
{
    public class WorldItemPickup : MonoBehaviour, IInteractable
    {
        public ItemDefinition item;
        public int quantity = 1;

        public string InteractionPrompt => item != null ? $"Pick up {item.DisplayName} x{quantity}" : "Pick up item";

        public void Interact(GameObject interactor)
        {
            var inv = interactor.GetComponent<PlayerInventory>()
                   ?? interactor.GetComponentInParent<PlayerInventory>();
            if (inv == null) return;

            if (!inv.CanAddItem(item, quantity))
            {
                Debug.Log($"[WorldItemPickup] Inventory full, cannot pick up {item?.DisplayName}.");
                return;
            }

            inv.TryAddItem(item, quantity);
            Destroy(gameObject);
        }
    }
}
