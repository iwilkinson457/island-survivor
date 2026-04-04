using UnityEngine;
using ExtractionDeadIsles.Building;
using ExtractionDeadIsles.Player;
using ExtractionDeadIsles.World;

namespace ExtractionDeadIsles.UI
{
    public class HomeIslandDebugHUD : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private CampfireProximityTracker campfireTracker;
        [SerializeField] private SimplePlacementController placementController;

        private void Reset()
        {
            playerController = GetComponent<PlayerController>();
            playerStats = GetComponent<PlayerStats>();
            campfireTracker = GetComponent<CampfireProximityTracker>();
            placementController = GetComponent<SimplePlacementController>();
        }

        private void OnGUI()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            GUILayout.BeginArea(new Rect(10, 240, 320, 180), GUI.skin.box);
            GUILayout.Label("<b>HOME ISLAND LOOP</b>");
            if (playerStats != null)
            {
                GUILayout.Label($"Health: {playerStats.CurrentHealth:F0}");
                GUILayout.Label($"Hunger: {playerStats.CurrentHunger:F0}");
                GUILayout.Label($"Thirst: {playerStats.CurrentThirst:F0}");
            }

            GUILayout.Label($"Near campfire: {campfireTracker != null && campfireTracker.IsNearCampfire}");
            GUILayout.Label($"Selected hotbar slot: {(placementController != null ? placementController.SelectedHotbarIndex + 1 : 1)}");
            GUILayout.Label("Tab inventory | 1-6 hotbar | F place selected placeable");
            GUILayout.Label("Left click confirm placement | Right click cancel");
            GUILayout.Label("E interact | Left click attack | Right click kickback/use alt");
            GUILayout.EndArea();
#endif
        }
    }
}
