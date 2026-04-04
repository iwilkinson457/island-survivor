using UnityEngine;

namespace ExtractionDeadIsles.World
{
    public class CampfireProximityTracker : MonoBehaviour
    {
        private int _campfireCount;

        public bool IsNearCampfire => _campfireCount > 0;

        public void NotifyEnteredCampfireRange()
        {
            _campfireCount++;
        }

        public void NotifyExitedCampfireRange()
        {
            _campfireCount = Mathf.Max(0, _campfireCount - 1);
        }
    }
}
