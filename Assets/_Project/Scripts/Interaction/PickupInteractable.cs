using UnityEngine;
using ExtractionDeadIsles.Interaction;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.Interaction
{
    public class PickupInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string itemName = "Item";
        [SerializeField] private string itemId = "item_id";

        public string InteractionPrompt => $"Pick up {itemName}";

        public void Interact(GameObject interactor)
        {
            GameEvents.ItemPickedUp(itemId);
            Debug.Log($"[Pickup] Picked up {itemName}");
            Destroy(gameObject);
        }
    }
}
