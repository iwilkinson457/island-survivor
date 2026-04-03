using System;
using System.Collections;
using UnityEngine;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.AI
{
    public class EnemySenses : MonoBehaviour
    {
        [Header("Hearing")]
        [SerializeField] private float hearingRange = 18f;

        [Header("Vision")]
        [SerializeField] private float visionRange = 15f;
        [SerializeField] private float visionAngle = 110f;
        [SerializeField] private float crouchVisionRange = 7f;
        [SerializeField] private LayerMask visionBlockMask;

        [SerializeField] private Transform eyePosition;

        public event Action<Vector3> OnSoundHeard;
        public event Action<Transform> OnPlayerSpotted;

        private Transform _player;

        private void OnEnable()
        {
            GameEvents.OnSoundEmitted += HandleSoundEmitted;
        }

        private void OnDisable()
        {
            GameEvents.OnSoundEmitted -= HandleSoundEmitted;
        }

        private void Start()
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null) _player = playerGo.transform;
            StartCoroutine(VisionRoutine());
        }

        private void HandleSoundEmitted(Vector3 position, float radius, SoundType type)
        {
            float dist = Vector3.Distance(transform.position, position);
            if (dist <= radius && dist <= hearingRange)
            {
                OnSoundHeard?.Invoke(position);
            }
        }

        private IEnumerator VisionRoutine()
        {
            var wait = new WaitForSeconds(0.15f);
            while (true)
            {
                yield return wait;
                CheckVision();
            }
        }

        private void CheckVision()
        {
            if (_player == null || IsDead()) return;

            Vector3 origin = eyePosition != null ? eyePosition.position : transform.position + Vector3.up * 1.5f;
            Vector3 toPlayer = _player.position - origin;
            float dist = toPlayer.magnitude;

            // Check if player is crouching — reduce effective range
            var playerCtrl = _player.GetComponent<Player.PlayerController>();
            bool playerCrouching = playerCtrl != null && playerCtrl.IsCrouching;
            float effectiveRange = playerCrouching ? crouchVisionRange : visionRange;

            if (dist > effectiveRange) return;

            float angle = Vector3.Angle(transform.forward, toPlayer);
            if (angle > visionAngle * 0.5f) return;

            // LOS check
            if (Physics.Raycast(origin, toPlayer.normalized, dist, visionBlockMask))
                return;

            OnPlayerSpotted?.Invoke(_player);
        }

        private bool IsDead()
        {
            var enemy = GetComponent<EnemyBase>();
            return enemy != null && enemy.IsDead;
        }
    }
}
