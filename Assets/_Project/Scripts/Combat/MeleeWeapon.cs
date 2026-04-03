using System.Collections;
using UnityEngine;
using ExtractionDeadIsles.Core;
using ExtractionDeadIsles.AI;

namespace ExtractionDeadIsles.Combat
{
    public class MeleeWeapon : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField] private float lightAttackDamage = 25f;
        [SerializeField] private float kickbackDamage = 5f;
        [SerializeField] private float attackRange = 1.8f;
        [SerializeField] private float kickbackRange = 2.0f;
        [SerializeField] private float hitRadius = 0.3f;
        [SerializeField] private float attackCooldown = 0.4f;
        [SerializeField] private float hitDetectionDelay = 0.15f;

        [Header("Sound")]
        [SerializeField] private float swingSoundRadius = 8f;

        [SerializeField] private Transform attackOrigin;

        private float _cooldownTimer;
        private bool _canAttack = true;

        private void Update()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0f) _canAttack = true;
            }
        }

        public void LightAttack()
        {
            if (!_canAttack) return;
            _canAttack = false;
            _cooldownTimer = attackCooldown;
            StartCoroutine(PerformHitDetection(lightAttackDamage, attackRange, false));
            EmitSwingSound();
        }

        public void Kickback()
        {
            if (!_canAttack) return;
            _canAttack = false;
            _cooldownTimer = attackCooldown;
            StartCoroutine(PerformHitDetection(kickbackDamage, kickbackRange, true));
            EmitSwingSound();
        }

        private IEnumerator PerformHitDetection(float damage, float range, bool isKickback)
        {
            yield return new WaitForSeconds(hitDetectionDelay);

            Transform origin = attackOrigin != null ? attackOrigin : transform;
            Ray ray = new Ray(origin.position, origin.forward);

            RaycastHit[] hits = Physics.SphereCastAll(ray, hitRadius, range);

            foreach (var hit in hits)
            {
                // Determine hit zone
                HitZone zone = HitZone.Torso;
                var hitZoneCol = hit.collider.GetComponent<HitZoneCollider>();
                if (hitZoneCol != null) zone = hitZoneCol.Zone;

                // Try to get damageable (walk up parent chain)
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null && !damageable.IsDead)
                {
                    damageable.TakeDamage(damage, zone);
                    Debug.Log($"[MeleeWeapon] Hit {hit.collider.name} in zone {zone} for {damage} dmg");

                    if (isKickback)
                    {
                        var stateMachine = hit.collider.GetComponentInParent<ZombieStateMachine>();
                        if (stateMachine != null)
                        {
                            Vector3 knockDir = (hit.transform.position - origin.position).normalized;
                            knockDir.y = 0f;
                            stateMachine.ApplyKnockback(knockDir);
                        }
                    }

                    break; // Hit one target per swing
                }
            }
        }

        private void EmitSwingSound()
        {
            Transform origin = attackOrigin != null ? attackOrigin : transform;
            GameEvents.EmitSound(origin.position, swingSoundRadius, SoundType.Melee);
        }
    }
}
