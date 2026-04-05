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

            _currentTarget = FindInteractableBySphereCast();
            if (_currentTarget != null)
                return;

            _currentTarget = FindInteractableByOverlap();
        }

        private IInteractable FindInteractableBySphereCast()
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            var hits = Physics.SphereCastAll(ray, 0.2f, interactRange, interactMask, QueryTriggerInteraction.Collide);
            if (hits == null || hits.Length == 0)
                return null;

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var hit in hits)
            {
                var interactable = hit.collider.GetComponent<IInteractable>()
                                 ?? hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                    return interactable;
            }

            return null;
        }

        private IInteractable FindInteractableByOverlap()
        {
            Vector3 probeCenter = cameraTransform.position + cameraTransform.forward * Mathf.Min(interactRange, 1.5f);
            var colliders = Physics.OverlapSphere(probeCenter, 0.75f, interactMask, QueryTriggerInteraction.Collide);
            if (colliders == null || colliders.Length == 0)
                return null;

            IInteractable best = null;
            float bestDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                var interactable = collider.GetComponent<IInteractable>()
                                 ?? collider.GetComponentInParent<IInteractable>();
                if (interactable == null)
                    continue;

                float distance = Vector3.Distance(cameraTransform.position, collider.ClosestPoint(cameraTransform.position));
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = interactable;
                }
            }

            return best;
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
