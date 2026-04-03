using UnityEngine;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.Combat
{
    public class HitZoneCollider : MonoBehaviour
    {
        [SerializeField] private HitZone zone = HitZone.Torso;
        public HitZone Zone => zone;
    }
}
