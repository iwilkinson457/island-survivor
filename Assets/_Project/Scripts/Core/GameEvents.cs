using System;
using UnityEngine;

namespace ExtractionDeadIsles.Core
{
    public static class GameEvents
    {
        // Sound system
        public static event Action<Vector3, float, SoundType> OnSoundEmitted;
        public static void EmitSound(Vector3 position, float radius, SoundType type)
        {
            OnSoundEmitted?.Invoke(position, radius, type);
        }

        // Player events
        public static event Action OnPlayerDied;
        public static void PlayerDied() => OnPlayerDied?.Invoke();

        // Enemy events
        public static event Action<UnityEngine.GameObject> OnEnemyDied;
        public static void EnemyDied(UnityEngine.GameObject enemy) => OnEnemyDied?.Invoke(enemy);

        // Item events
        public static event Action<string> OnItemPickedUp;
        public static void ItemPickedUp(string itemId) => OnItemPickedUp?.Invoke(itemId);
    }
}
