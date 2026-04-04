using UnityEngine;

namespace ExtractionDeadIsles.Building
{
    public class PlacementGhost : MonoBehaviour
    {
        [SerializeField] private LayerMask placementMask = ~0;
        [SerializeField] private float maxSlope = 35f;
        [SerializeField] private float overlapRadius = 0.75f;

        public bool IsValidPlacement { get; private set; }
        public Vector3 LastPlacementPosition { get; private set; }

        public bool UpdatePlacement(Ray ray, float maxDistance)
        {
            if (!Physics.Raycast(ray, out var hit, maxDistance, placementMask))
            {
                IsValidPlacement = false;
                return false;
            }

            LastPlacementPosition = hit.point;
            transform.position = hit.point;
            transform.rotation = Quaternion.identity;

            float slope = Vector3.Angle(hit.normal, Vector3.up);
            bool validSlope = slope <= maxSlope;
            bool blocked = Physics.CheckSphere(hit.point + Vector3.up * 0.2f, overlapRadius);
            IsValidPlacement = validSlope && !blocked;
            return true;
        }
    }
}
