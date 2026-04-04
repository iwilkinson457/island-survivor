using UnityEngine;
using UnityEngine.InputSystem;
using ExtractionDeadIsles.Core;

namespace ExtractionDeadIsles.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float crouchSpeed = 2f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravity = -20f;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainSprint = 15f;
        [SerializeField] private float staminaDrainJump = 20f;
        [SerializeField] private float staminaRecoverRate = 10f;
        [SerializeField] private float staminaSprintMinimum = 0f;
        [SerializeField] private float staminaSprintResumeThreshold = 20f;
        [SerializeField] private float staminaJumpMinimum = 25f;

        [Header("Camera Look")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 0.15f;
        [SerializeField] private float verticalClampMin = -80f;
        [SerializeField] private float verticalClampMax = 80f;

        [Header("Crouch")]
        [SerializeField] private float normalHeight = 1.8f;
        [SerializeField] private float crouchHeight = 0.9f;
        [SerializeField] private float crouchCameraHeight = 0.8f;
        [SerializeField] private float normalCameraHeight = 1.6f;

        [Header("Sound Emission")]
        [SerializeField] private float footstepInterval = 0.45f;
        [SerializeField] private float walkSoundRadius = 5f;
        [SerializeField] private float sprintSoundRadius = 12f;
        [SerializeField] private float crouchSoundRadius = 2f;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundMask;

        private CharacterController _cc;
        private Vector3 _velocity;
        private float _verticalLook;
        private float _currentStamina;
        private float _footstepTimer;
        private bool _isCrouching;
        private bool _isSprinting;
        private bool _isGrounded;
        private bool _staminaExhausted;

        // Input
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _jumpPressed;
        private bool _sprintHeld;
        private bool _crouchHeld;

        public float CurrentStamina => _currentStamina;
        public float MaxStamina => maxStamina;
        public bool IsCrouching => _isCrouching;
        public bool IsSprinting => _isSprinting;
        public float CurrentSpeed => new Vector3(_velocity.x, 0f, _velocity.z).magnitude;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _currentStamina = maxStamina;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            GatherInput();
            CheckGrounded();
            HandleCrouch();
            HandleMovement();
            HandleLook();
            HandleFootstepSound();
        }

        private void GatherInput()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (keyboard == null || mouse == null) return;

            _moveInput = Vector2.zero;
            if (keyboard.wKey.isPressed) _moveInput.y += 1f;
            if (keyboard.sKey.isPressed) _moveInput.y -= 1f;
            if (keyboard.aKey.isPressed) _moveInput.x -= 1f;
            if (keyboard.dKey.isPressed) _moveInput.x += 1f;

            _lookInput = mouse.delta.ReadValue() * mouseSensitivity;
            _sprintHeld = keyboard.leftShiftKey.isPressed;
            _crouchHeld = keyboard.leftCtrlKey.isPressed;

            if (keyboard.spaceKey.wasPressedThisFrame)
                _jumpPressed = true;
        }

        private void CheckGrounded()
        {
            Vector3 spherePos = transform.position + Vector3.up * groundCheckRadius;
            _isGrounded = Physics.CheckSphere(spherePos, groundCheckRadius, groundMask);

            if (_isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;
        }

        private void HandleCrouch()
        {
            bool wantCrouch = _crouchHeld;

            if (wantCrouch && !_isCrouching)
            {
                _isCrouching = true;
                _cc.height = crouchHeight;
                _cc.center = new Vector3(0f, crouchHeight / 2f, 0f);
                if (cameraTransform != null)
                {
                    var lp = cameraTransform.localPosition;
                    cameraTransform.localPosition = new Vector3(lp.x, crouchCameraHeight, lp.z);
                }
            }
            else if (!wantCrouch && _isCrouching)
            {
                // Check headroom before standing
                if (!Physics.SphereCast(transform.position, _cc.radius, Vector3.up, out _, normalHeight - crouchHeight))
                {
                    _isCrouching = false;
                    _cc.height = normalHeight;
                    _cc.center = new Vector3(0f, normalHeight / 2f, 0f);
                    if (cameraTransform != null)
                    {
                        var lp = cameraTransform.localPosition;
                        cameraTransform.localPosition = new Vector3(lp.x, normalCameraHeight, lp.z);
                    }
                }
            }
        }

        private void HandleMovement()
        {
            // Stamina regen / drain — hysteresis prevents flicker at 0
            bool wantSprint = _sprintHeld && _moveInput.magnitude > 0.1f && !_isCrouching;
            if (_currentStamina <= staminaSprintMinimum) _staminaExhausted = true;
            if (_staminaExhausted && _currentStamina >= staminaSprintResumeThreshold) _staminaExhausted = false;
            _isSprinting = wantSprint && !_staminaExhausted;

            if (_isSprinting)
                _currentStamina = Mathf.Max(0f, _currentStamina - staminaDrainSprint * Time.deltaTime);
            else
                _currentStamina = Mathf.Min(maxStamina, _currentStamina + staminaRecoverRate * Time.deltaTime);

            float speed = _isCrouching ? crouchSpeed : (_isSprinting ? sprintSpeed : walkSpeed);

            Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            _cc.Move(move * speed * Time.deltaTime);

            // Jump
            if (_jumpPressed && _isGrounded && _currentStamina >= staminaJumpMinimum)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                _currentStamina = Mathf.Max(0f, _currentStamina - staminaDrainJump);
            }
            _jumpPressed = false;

            _velocity.y += gravity * Time.deltaTime;
            _cc.Move(_velocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            transform.Rotate(Vector3.up * _lookInput.x);
            _verticalLook -= _lookInput.y;
            _verticalLook = Mathf.Clamp(_verticalLook, verticalClampMin, verticalClampMax);

            if (cameraTransform != null)
            {
                Vector3 lp = cameraTransform.localEulerAngles;
                cameraTransform.localEulerAngles = new Vector3(_verticalLook, lp.y, lp.z);
            }
        }

        private void HandleFootstepSound()
        {
            if (_moveInput.magnitude < 0.1f || !_isGrounded) return;

            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer > 0f) return;

            _footstepTimer = _isSprinting ? footstepInterval * 0.6f : footstepInterval;
            float radius = _isCrouching ? crouchSoundRadius : (_isSprinting ? sprintSoundRadius : walkSoundRadius);
            GameEvents.EmitSound(transform.position, radius, SoundType.Footstep);
        }
    }
}
