using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemySenses))]
    public class ZombieStateMachine : MonoBehaviour
    {
        [Header("Speeds")]
        [SerializeField] private float wanderSpeed = 1.5f;
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float suspiciousSpeed = 2f;

        [Header("Behaviour")]
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float attackRange = 1.8f;
        [SerializeField] private float attackCooldown = 1.2f;
        [SerializeField] private float attackDamage = 12f;
        [SerializeField] private float losLostDuration = 3f;
        [SerializeField] private float suspiciousDuration = 8f;
        [SerializeField] private float knockbackDuration = 0.4f;

        private NavMeshAgent _agent;
        private EnemySenses _senses;
        private ZombieState _state = ZombieState.Idle;

        private Transform _player;
        private Vector3 _lastKnownPlayerPos;
        private float _losLostTimer;
        private float _suspiciousTimer;
        private float _attackCooldownTimer;
        private float _knockbackTimer;
        private bool _isKnockedBack;

        private ZombieController _controller;

        public ZombieState CurrentState => _state;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _senses = GetComponent<EnemySenses>();
            _controller = GetComponent<ZombieController>();
        }

        private void OnEnable()
        {
            _senses.OnSoundHeard += HandleSoundHeard;
            _senses.OnPlayerSpotted += HandlePlayerSpotted;
        }

        private void OnDisable()
        {
            _senses.OnSoundHeard -= HandleSoundHeard;
            _senses.OnPlayerSpotted -= HandlePlayerSpotted;
        }

        private void Start()
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null) _player = playerGo.transform;
            SetState(ZombieState.Idle);
        }

        private void Update()
        {
            if (_state == ZombieState.Dead) return;

            if (_isKnockedBack)
            {
                _knockbackTimer -= Time.deltaTime;
                if (_knockbackTimer <= 0f) _isKnockedBack = false;
                return;
            }

            _attackCooldownTimer -= Time.deltaTime;

            switch (_state)
            {
                case ZombieState.Idle:
                    UpdateIdle();
                    break;
                case ZombieState.Wander:
                    UpdateWander();
                    break;
                case ZombieState.Suspicious:
                    UpdateSuspicious();
                    break;
                case ZombieState.Chase:
                    UpdateChase();
                    break;
                case ZombieState.Attack:
                    UpdateAttack();
                    break;
            }
        }

        private void UpdateIdle()
        {
            // Occasionally transition to wander
            if (Random.value < 0.002f) SetState(ZombieState.Wander);
        }

        private void UpdateWander()
        {
            if (!_agent.hasPath || _agent.remainingDistance < 0.5f)
            {
                SetState(ZombieState.Idle);
            }
        }

        private void UpdateSuspicious()
        {
            _suspiciousTimer -= Time.deltaTime;

            if (!_agent.hasPath || _agent.remainingDistance < 0.5f)
            {
                // Arrived at sound pos — look around
            }

            if (_suspiciousTimer <= 0f)
                SetState(ZombieState.Idle);
        }

        private void UpdateChase()
        {
            if (_player == null) { SetState(ZombieState.Idle); return; }

            float distToPlayer = Vector3.Distance(transform.position, _player.position);

            // Check if still have LOS (senses handles this via events, but also poll for los loss)
            if (!HasLOS())
            {
                _losLostTimer -= Time.deltaTime;
                if (_losLostTimer <= 0f)
                {
                    _lastKnownPlayerPos = _player.position;
                    SetState(ZombieState.Suspicious);
                    return;
                }
            }
            else
            {
                _losLostTimer = losLostDuration;
                _lastKnownPlayerPos = _player.position;
            }

            if (distToPlayer <= attackRange)
            {
                SetState(ZombieState.Attack);
                return;
            }

            _agent.SetDestination(_player.position);
        }

        private void UpdateAttack()
        {
            if (_player == null) { SetState(ZombieState.Idle); return; }

            float distToPlayer = Vector3.Distance(transform.position, _player.position);

            if (distToPlayer > attackRange * 1.3f)
            {
                SetState(ZombieState.Chase);
                return;
            }

            if (_attackCooldownTimer <= 0f)
            {
                PerformAttack();
            }
        }

        private void PerformAttack()
        {
            _attackCooldownTimer = attackCooldown;

            var playerStats = _player?.GetComponent<Player.PlayerStats>();
            if (playerStats != null && !playerStats.IsDead)
            {
                playerStats.TakeDamage(attackDamage);
                Debug.Log($"[Zombie] {gameObject.name} attacked player for {attackDamage} damage.");
            }
        }

        private bool HasLOS()
        {
            if (_player == null) return false;
            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Vector3 toPlayer = _player.position - origin;
            return !Physics.Raycast(origin, toPlayer.normalized, toPlayer.magnitude, ~LayerMask.GetMask("Player", "Enemy"));
        }

        public void ApplyKnockback(Vector3 direction)
        {
            _isKnockedBack = true;
            _knockbackTimer = knockbackDuration;
            _agent.enabled = false;
            StartCoroutine(DoKnockback(direction));
        }

        private IEnumerator DoKnockback(Vector3 direction)
        {
            float elapsed = 0f;
            float duration = knockbackDuration;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + direction * 2.5f;

            // Clamp to NavMesh
            if (NavMesh.SamplePosition(endPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                endPos = hit.position;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _agent.enabled = true;
            _isKnockedBack = false;
        }

        public void TriggerDeath()
        {
            SetState(ZombieState.Dead);
        }

        private void SetState(ZombieState newState)
        {
            _state = newState;
            OnStateEnter(newState);
        }

        private void OnStateEnter(ZombieState state)
        {
            switch (state)
            {
                case ZombieState.Idle:
                    _agent.isStopped = true;
                    _agent.speed = wanderSpeed;
                    break;
                case ZombieState.Wander:
                    _agent.isStopped = false;
                    _agent.speed = wanderSpeed;
                    Vector3 wanderTarget = transform.position + Random.insideUnitSphere * wanderRadius;
                    wanderTarget.y = transform.position.y;
                    if (NavMesh.SamplePosition(wanderTarget, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                        _agent.SetDestination(hit.position);
                    break;
                case ZombieState.Suspicious:
                    _agent.isStopped = false;
                    _agent.speed = suspiciousSpeed;
                    _agent.SetDestination(_lastKnownPlayerPos);
                    _suspiciousTimer = suspiciousDuration;
                    break;
                case ZombieState.Chase:
                    _agent.isStopped = false;
                    _agent.speed = chaseSpeed;
                    _losLostTimer = losLostDuration;
                    break;
                case ZombieState.Attack:
                    _agent.isStopped = true;
                    break;
                case ZombieState.Dead:
                    _agent.isStopped = true;
                    _agent.enabled = false;
                    // Disable colliders
                    foreach (var col in GetComponentsInChildren<Collider>())
                        col.enabled = false;
                    break;
            }
        }

        private void HandleSoundHeard(Vector3 position)
        {
            if (_state == ZombieState.Chase || _state == ZombieState.Attack || _state == ZombieState.Dead) return;
            _lastKnownPlayerPos = position;
            SetState(ZombieState.Suspicious);
        }

        private void HandlePlayerSpotted(Transform player)
        {
            if (_state == ZombieState.Dead) return;
            _player = player;
            _lastKnownPlayerPos = player.position;
            SetState(ZombieState.Chase);
        }
    }
}
