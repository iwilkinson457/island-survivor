using UnityEngine;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float maxThirst = 100f;

        private float _currentHealth;
        private float _currentHunger;
        private float _currentThirst;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercent => _currentHealth / maxHealth;
        public bool IsDead => _currentHealth <= 0f;

        // Hunger and Thirst are stubs for Milestone A - they exist but don't drain
        public float CurrentHunger => _currentHunger;
        public float CurrentThirst => _currentThirst;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _currentHunger = maxHunger;
            _currentThirst = maxThirst;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;
            _currentHealth = Mathf.Max(0f, _currentHealth - amount);
            if (_currentHealth <= 0f)
            {
                GameEvents.PlayerDied();
                Debug.Log("[PlayerStats] Player died.");
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            _currentHealth = Mathf.Min(maxHealth, _currentHealth + amount);
        }
    }
}
