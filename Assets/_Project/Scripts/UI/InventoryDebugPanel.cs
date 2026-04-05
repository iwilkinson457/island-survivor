using UnityEngine;
using UnityEngine.InputSystem;
using ExtractionDeadIsles.Building;
using ExtractionDeadIsles.Crafting;
using ExtractionDeadIsles.Inventory;
using ExtractionDeadIsles.World;

namespace ExtractionDeadIsles.UI
{
    public class InventoryDebugPanel : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerCrafter crafter;
        [SerializeField] private CampfireProximityTracker campfireTracker;
        [SerializeField] private SimplePlacementController placementController;

        private bool _visible;

        private void Reset()
        {
            inventory = GetComponent<PlayerInventory>();
            crafter = GetComponent<PlayerCrafter>();
            campfireTracker = GetComponent<CampfireProximityTracker>();
            placementController = GetComponent<SimplePlacementController>();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.tabKey.wasPressedThisFrame)
            {
                _visible = !_visible;
                ApplyCursorState();
            }
        }

        private void OnEnable()
        {
            ApplyCursorState();
        }

        private void OnDisable()
        {
            _visible = false;
            ApplyCursorState();
        }

        private void ApplyCursorState()
        {
            Cursor.lockState = _visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _visible;
        }

        private void OnGUI()
        {
            if (!_visible || inventory == null) return;

            GUILayout.BeginArea(new Rect(Screen.width - 360, 10, 350, Screen.height - 20), GUI.skin.box);
            GUILayout.Label("<b>Inventory Debug</b> — use InventoryPanel for full UI");
            GUILayout.Label($"Near campfire: {campfireTracker != null && campfireTracker.IsNearCampfire}");
            GUILayout.Label($"Placement mode: {placementController != null && placementController.IsInPlacementMode}");
            GUILayout.Space(8);
            GUILayout.Label("Hotbar");
            DrawSlots(inventory.Hotbar);

            if (crafter != null)
            {
                GUILayout.Space(8);
                GUILayout.Label("Recipes");
                foreach (var recipe in crafter.GetRecipes())
                {
                    bool canCraft = crafter.CanCraft(recipe, campfireTracker != null && campfireTracker.IsNearCampfire);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"- {recipe.DisplayName} ({(canCraft ? "craftable" : "missing reqs")})", GUILayout.Width(240));
                    GUI.enabled = canCraft;
                    if (GUILayout.Button("Craft", GUILayout.Width(80)))
                        crafter.TryCraft(recipe, campfireTracker != null && campfireTracker.IsNearCampfire);
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();
        }

        private void DrawSlots(System.Collections.Generic.IReadOnlyList<InventorySlot> slots)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                GUILayout.Label(slot.HasItem
                    ? $"[{i + 1}] {slot.Item.DisplayName} x{slot.Quantity}"
                    : $"[{i + 1}] Empty");
            }
        }
    }
}
