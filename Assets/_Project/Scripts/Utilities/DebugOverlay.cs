using UnityEngine;
using ExtractionDeadIsles.Player;
using ExtractionDeadIsles.AI;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.Utilities
{
    public class DebugOverlay : MonoBehaviour
    {
        private PlayerController _playerController;
        private PlayerStats _playerStats;
        private Vector3 _lastSoundPos;
        private float _lastSoundRadius;
        private int _zombieCount;

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerController = player.GetComponent<PlayerController>();
                _playerStats = player.GetComponent<PlayerStats>();
            }
            GameEvents.OnSoundEmitted += OnSoundEmitted;
            GameEvents.OnEnemyDied += OnEnemyDied;
            _zombieCount = FindObjectsByType<ZombieController>(FindObjectsSortMode.None).Length;
        }

        private void OnDestroy()
        {
            GameEvents.OnSoundEmitted -= OnSoundEmitted;
            GameEvents.OnEnemyDied -= OnEnemyDied;
        }

        private void OnSoundEmitted(Vector3 pos, float radius, SoundType type)
        {
            _lastSoundPos = pos;
            _lastSoundRadius = radius;
        }

        private void OnEnemyDied(GameObject enemy)
        {
            _zombieCount = Mathf.Max(0, _zombieCount - 1);
        }

        private void OnGUI()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            GUILayout.BeginArea(new Rect(10, 10, 300, 220), GUI.skin.box);
            GUILayout.Label($"<b>== DEBUG OVERLAY ==</b>");

            if (_playerStats != null)
                GUILayout.Label($"Health: {_playerStats.CurrentHealth:F0} / {_playerStats.MaxHealth:F0}");

            if (_playerController != null)
            {
                GUILayout.Label($"Stamina: {_playerController.CurrentStamina:F0} / {_playerController.MaxStamina:F0}");
                GUILayout.Label($"Speed: {_playerController.CurrentSpeed:F1} m/s");
                GUILayout.Label($"Crouching: {_playerController.IsCrouching}");
                GUILayout.Label($"Sprinting: {_playerController.IsSprinting}");
            }

            if (_playerStats != null)
            {
                GUILayout.Label($"Hunger: {_playerStats.CurrentHunger:F0}");
                GUILayout.Label($"Thirst: {_playerStats.CurrentThirst:F0}");
            }

            GUILayout.Label($"Zombies alive: {_zombieCount}");
            GUILayout.Label($"Last sound: {_lastSoundPos} r={_lastSoundRadius:F1}m");
            GUILayout.EndArea();
#endif
        }
    }
}
