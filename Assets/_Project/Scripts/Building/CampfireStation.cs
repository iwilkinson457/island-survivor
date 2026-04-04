using UnityEngine;
using ExtractionDeadIsles.Interaction;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;
using ExtractionDeadIsles.Player;
using ExtractionDeadIsles.World;

namespace ExtractionDeadIsles.Building
{
    [RequireComponent(typeof(SphereCollider))]
    public class CampfireStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private string prompt = "Use Campfire";
        [SerializeField] private float useRadius = 3f;
        [SerializeField] private ItemDefinition rawItem;
        [SerializeField] private ItemDefinition cookedItem;

        public string InteractionPrompt => prompt;

        private void Reset()
        {
            var sphere = GetComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = useRadius;
        }

        public void Interact(GameObject interactor)
        {
            TryCook(interactor);
        }

        public void ConfigureCooking(ItemDefinition raw, ItemDefinition cooked)
        {
            rawItem = raw;
            cookedItem = cooked;
        }

        public bool TryCook(GameObject interactor)
        {
            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null || rawItem == null || cookedItem == null) return false;
            if (!inventory.HasItems(rawItem, 1))
            {
                Debug.Log("[CampfireStation] No raw food available to cook.");
                return false;
            }

            if (!inventory.CanAddItem(cookedItem, 1))
            {
                Debug.Log("[CampfireStation] Inventory full. Could not add cooked food.");
                return false;
            }

            inventory.RemoveItems(rawItem, 1);
            inventory.TryAddItem(cookedItem, 1);
            Debug.Log($"[CampfireStation] Cooked {cookedItem.DisplayName}.");
            return true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var tracker = other.GetComponent<CampfireProximityTracker>();
            if (tracker != null)
                tracker.NotifyEnteredCampfireRange();
        }

        private void OnTriggerExit(Collider other)
        {
            var tracker = other.GetComponent<CampfireProximityTracker>();
            if (tracker != null)
                tracker.NotifyExitedCampfireRange();
        }
    }
}
