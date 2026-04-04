using UnityEngine;
using ExtractionDeadIsles.Core;
using ExtractionDeadIsles.Items;

namespace ExtractionDeadIsles.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float maxThirst = 100f;
        [SerializeField] private float hungerDrainPerSecond = 0.4f;
        [SerializeField] private float thirstDrainPerSecond = 0.7f;
        [SerializeField] private float starvationGraceSeconds = 5f;
        [SerializeField] private float starvationDamagePerSecond = 2f;

        private float _currentHealth;
        private float _currentHunger;
        private float _currentThirst;
        private float _zeroNeedsTimer;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercent => _currentHealth / maxHealth;
        public bool IsDead => _currentHealth <= 0f;
        public float CurrentHunger => _currentHunger;
        public float CurrentThirst => _currentThirst;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _currentHunger = maxHunger;
            _currentThirst = maxThirst;
        }

        private void Update()
        {
            if (IsDead) return;

            _currentHunger = Mathf.Clamp(_currentHunger - hungerDrainPerSecond * Time.deltaTime, 0f, maxHunger);
            _currentThirst = Mathf.Clamp(_currentThirst - thirstDrainPerSecond * Time.deltaTime, 0f, maxThirst);

            if (_currentHunger <= 0f || _currentThirst <= 0f)
            {
                _zeroNeedsTimer += Time.deltaTime;
                if (_zeroNeedsTimer >= starvationGraceSeconds)
                    TakeDamage(starvationDamagePerSecond * Time.deltaTime);
            }
            else
            {
                _zeroNeedsTimer = 0f;
            }
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

        public void RestoreHunger(int amount)
        {
            _currentHunger = Mathf.Clamp(_currentHunger + amount, 0f, maxHunger);
        }

        public void RestoreThirst(int amount)
        {
            _currentThirst = Mathf.Clamp(_currentThirst + amount, 0f, maxThirst);
        }

        public void Consume(ItemDefinition item)
        {
            if (item == null) return;
            RestoreHunger(item.HungerRestore);
            RestoreThirst(item.ThirstRestore);
            Debug.Log($"[PlayerStats] Consumed {item.DisplayName}. Hunger +{item.HungerRestore}, Thirst +{item.ThirstRestore}");
        }
    }
}
