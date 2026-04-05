using UnityEngine;
using UnityEngine.InputSystem;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Building
{
    public class SimplePlacementController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private float placeDistance = 5f;
        [SerializeField] private LayerMask placementMask = ~0;
        [SerializeField] private ItemDefinition campfireKitItem;
        [SerializeField] private ItemDefinition rawCookInput;
        [SerializeField] private ItemDefinition cookedFoodOutput;

        private PlacementGhost _ghost;
        private bool _placementMode;
        private int _selectedHotbarIndex;

        public bool IsInPlacementMode => _placementMode;
        public int SelectedHotbarIndex => _selectedHotbarIndex;

        private void Reset()
        {
            inventory = GetComponent<PlayerInventory>();
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            HandleHotbarSelection();
            HandlePlacementToggle();
            HandlePlacementUpdate();
        }

        private void HandleHotbarSelection()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.digit1Key.wasPressedThisFrame) _selectedHotbarIndex = 0;
            if (keyboard.digit2Key.wasPressedThisFrame) _selectedHotbarIndex = 1;
            if (keyboard.digit3Key.wasPressedThisFrame) _selectedHotbarIndex = 2;
            if (keyboard.digit4Key.wasPressedThisFrame) _selectedHotbarIndex = 3;
            if (keyboard.digit5Key.wasPressedThisFrame) _selectedHotbarIndex = 4;
            if (keyboard.digit6Key.wasPressedThisFrame) _selectedHotbarIndex = 5;

            if (keyboard.fKey.wasPressedThisFrame)
                TryBeginPlacement();
        }

        private void HandlePlacementToggle()
        {
            if (!_placementMode) return;
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.rightButton.wasPressedThisFrame)
                CancelPlacement();
        }

        private void HandlePlacementUpdate()
        {
            if (!_placementMode || _ghost == null || cameraTransform == null) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            _ghost.UpdatePlacement(ray, placeDistance);

            if (mouse.leftButton.wasPressedThisFrame && _ghost.IsValidPlacement)
                ConfirmPlacement();
        }

        public void TryBeginPlacement()
        {
            if (inventory == null || campfireKitItem == null) return;
            if (!inventory.TryGetHotbarSlot(_selectedHotbarIndex, out var slot) || slot.Item != campfireKitItem || slot.Quantity <= 0)
            {
                Debug.Log("[SimplePlacementController] Select Campfire Kit in hotbar before placing.");
                return;
            }

            if (_ghost == null)
                _ghost = CreateGhost();

            _placementMode = true;
            _ghost.gameObject.SetActive(true);
            Debug.Log("[SimplePlacementController] Placement mode started.");
        }

        private PlacementGhost CreateGhost()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "PlacementGhost_Campfire";
            Destroy(go.GetComponent<Collider>());
            go.transform.localScale = new Vector3(1.2f, 0.1f, 1.2f);
            var ghost = go.AddComponent<PlacementGhost>();
            return ghost;
        }

        private void ConfirmPlacement()
        {
            if (!inventory.TryGetHotbarSlot(_selectedHotbarIndex, out var slot) || slot.Item != campfireKitItem)
            {
                CancelPlacement();
                return;
            }

            slot.Remove(1);
            inventory.NotifyChanged();
            SpawnCampfire(_ghost.LastPlacementPosition);
            CancelPlacement();
        }

        private void SpawnCampfire(Vector3 position)
        {
            var root = new GameObject("CampfireStation_Runtime");
            root.transform.position = position;

            var baseMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseMesh.name = "CampfireVisual";
            baseMesh.transform.SetParent(root.transform, false);
            baseMesh.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);

            var collider = root.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 3f;
            var station = root.AddComponent<CampfireStation>();
            station.ConfigureCooking(rawCookInput, cookedFoodOutput);
            Debug.Log("[SimplePlacementController] Campfire placed.");
        }

        public void CancelPlacement()
        {
            _placementMode = false;
            if (_ghost != null)
                _ghost.gameObject.SetActive(false);
        }
    }
}
