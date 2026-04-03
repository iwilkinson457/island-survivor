using UnityEngine;
using UnityEngine.InputSystem;
using ExtractionDeadIsles.Interaction;

namespace ExtractionDeadIsles.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float interactRange = 2.5f;
        [SerializeField] private LayerMask interactMask;

        private IInteractable _currentTarget;

        public string CurrentPrompt => _currentTarget?.InteractionPrompt ?? string.Empty;
        public bool HasTarget => _currentTarget != null;

        private void Update()
        {
            DetectTarget();
            HandleInteractInput();
        }

        private void DetectTarget()
        {
            if (cameraTransform == null) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
            {
                _currentTarget = hit.collider.GetComponent<IInteractable>()
                              ?? hit.collider.GetComponentInParent<IInteractable>();
            }
            else
            {
                _currentTarget = null;
            }
        }

        private void HandleInteractInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.eKey.wasPressedThisFrame && _currentTarget != null)
            {
                _currentTarget.Interact(gameObject);
            }
        }
    }
}
