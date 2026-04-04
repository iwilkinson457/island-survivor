using UnityEngine;
using ExtractionDeadIsles.Interaction;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.World
{
    public class GatherableResource : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemDefinition itemToGrant;
        [SerializeField] private int amount = 1;
        [SerializeField] private int usesRemaining = 1;
        [SerializeField] private string promptOverride;
        [SerializeField] private bool canRespawn;
        [SerializeField] private float respawnTime = 30f;

        public string InteractionPrompt => string.IsNullOrWhiteSpace(promptOverride)
            ? $"Gather {(itemToGrant != null ? itemToGrant.DisplayName : "Resource")}"
            : promptOverride;

        public void Interact(GameObject interactor)
        {
            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogWarning("[GatherableResource] Interactor has no PlayerInventory.");
                return;
            }

            if (itemToGrant == null)
            {
                Debug.LogWarning($"[GatherableResource] {name} has no item assigned.");
                return;
            }

            if (!inventory.TryAddItem(itemToGrant, amount))
            {
                Debug.Log($"[GatherableResource] Inventory full. Could not gather {itemToGrant.DisplayName}.");
                return;
            }

            usesRemaining--;
            Debug.Log($"[GatherableResource] Gathered {itemToGrant.DisplayName} x{amount}. Uses left: {usesRemaining}");

            if (usesRemaining <= 0)
            {
                if (canRespawn)
                    Debug.Log($"[GatherableResource] Respawn stub only. Expected respawn time: {respawnTime}s");
                Destroy(gameObject);
            }
        }
    }
}
