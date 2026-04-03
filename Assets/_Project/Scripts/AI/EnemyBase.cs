using UnityEngine;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.AI
{
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float maxHealth = 60f;

        protected float currentHealth;

        public bool IsDead => currentHealth <= 0f;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float amount, HitZone zone)
        {
            if (IsDead) return;
            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                OnDeath();
            }
        }

        protected virtual void OnDeath()
        {
            GameEvents.EnemyDied(gameObject);
            Debug.Log($"[EnemyBase] {gameObject.name} died.");
        }
    }
}
