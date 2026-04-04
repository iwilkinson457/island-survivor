using UnityEngine;
using UnityEngine.InputSystem;

namespace ExtractionDeadIsles.Combat
{
    public class WeaponHolder : MonoBehaviour
    {
        [SerializeField] private MeleeWeapon activeMeleeWeapon;
        [SerializeField] private string equippedItemId;

        public string EquippedItemId => equippedItemId;

        public void SetEquippedItem(string itemId)
        {
            equippedItemId = itemId;
        }

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
