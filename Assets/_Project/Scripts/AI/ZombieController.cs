using UnityEngine;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.AI
{
    [RequireComponent(typeof(ZombieStateMachine))]
    [RequireComponent(typeof(EnemySenses))]
    public class ZombieController : EnemyBase
    {
        [Header("Hit Zone Damage Multipliers")]
        [SerializeField] private float headMultiplier = 2.5f;
        [SerializeField] private float torsoMultiplier = 1.0f;
        [SerializeField] private float legMultiplier = 0.7f;

        [Header("Leg Damage")]
        [SerializeField] private float legSlowThreshold = 0.5f; // % of max health leg damage to trigger slow
        [SerializeField] private float legSlowSpeed = 1.8f;

        private ZombieStateMachine _stateMachine;
        private float _legDamageAccumulated = 0f;
        private float _legDamageForSlow;
        private bool _legDamaged = false;
        private UnityEngine.AI.NavMeshAgent _agent;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = GetComponent<ZombieStateMachine>();
            _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _legDamageForSlow = maxHealth * legSlowThreshold;
        }

        public override void TakeDamage(float amount, HitZone zone)
        {
            if (IsDead) return;

            float multiplier = zone switch
            {
                HitZone.Head => headMultiplier,
                HitZone.Torso => torsoMultiplier,
                HitZone.Leg => legMultiplier,
                _ => 1f
            };

            float finalDamage = amount * multiplier;

            if (zone == HitZone.Leg)
            {
                _legDamageAccumulated += finalDamage;
                if (!_legDamaged && _legDamageAccumulated >= _legDamageForSlow)
                {
                    _legDamaged = true;
                    if (_agent != null) _agent.speed = legSlowSpeed;
                    Debug.Log($"[ZombieController] {gameObject.name} leg damaged — slowed.");
                }
            }

            base.TakeDamage(finalDamage, zone);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            _stateMachine?.TriggerDeath();
        }
    }
}
