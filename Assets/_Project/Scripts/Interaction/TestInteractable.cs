using UnityEngine;
using ExtractionDeadIsles.Interaction;

namespace ExtractionDeadIsles.Interaction
{
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Interact";
        [SerializeField] private string logMessage = "Test interactable triggered.";

        public string InteractionPrompt => promptText;

        public void Interact(GameObject interactor)
        {
            Debug.Log($"[TestInteractable] {logMessage}");
        }
    }
}
