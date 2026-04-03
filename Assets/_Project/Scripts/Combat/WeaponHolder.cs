using UnityEngine;
using UnityEngine.InputSystem;
using ExtractionDeadIsles.Combat;

namespace ExtractionDeadIsles.Combat
{
    public class WeaponHolder : MonoBehaviour
    {
        [SerializeField] private MeleeWeapon activeMeleeWeapon;

        private void Update()
        {
            if (activeMeleeWeapon == null) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
                activeMeleeWeapon.LightAttack();

            if (mouse.rightButton.wasPressedThisFrame)
                activeMeleeWeapon.Kickback();
        }
    }
}
