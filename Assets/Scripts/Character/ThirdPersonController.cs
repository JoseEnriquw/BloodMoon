using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Assets.Scripts.Character
{

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Referencias")]
        public Transform CinemachineCameraTarget;  // pivot de cámara (hombros)
        public Transform CameraTransform;          // Main Camera transform
        public Transform BarrelEnd;                // salida de proyectil

        [Header("Movement")]
        public float WalkSpeed = 2f;
        public float RunSpeed = 5f;
        public float Gravity = -9.81f;
        public float RotationSpeed = 10f;

        [Header("Camera")]
        public float LookSpeed = 1.5f;
        public float CameraDistance = 4f;
        public float CameraHeight = 1.5f;
        public float MinPitch = -30f;
        public float MaxPitch = 70f;
        public LayerMask ObstacleLayers;
        public float CameraCollisionRadius = 0.3f;

        [Header("Disparo")]
        public GameObject BulletPrefab;
        public float BulletRange = 100f;
        public float BulletSpeed = 20f;

        // componentes
        CharacterController _controller;
        Animator _animator;
        StarterAssetsInputs _input;
#if ENABLE_INPUT_SYSTEM
        PlayerInput _playerInput;
#endif

        // estado cámara
        float _cinemachineYaw;
        float _cinemachinePitch;
        float _verticalVelocity;
        const float _threshold = 0.01f;

        bool IsMouse =>
#if ENABLE_INPUT_SYSTEM
            _playerInput.currentControlScheme == "KeyboardMouse";
#else
        true;
#endif

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif

            // inicializamos yaw/pitch
            _cinemachineYaw = CinemachineCameraTarget.rotation.eulerAngles.y;
            _cinemachinePitch = CinemachineCameraTarget.rotation.eulerAngles.x;
        }

        void Update()
        {
            HandleMovement();
            HandleAnimations();
            HandleShooting();
        }

        void LateUpdate()
        {
            HandleCamera();
        }

        void HandleMovement()
        {
            // 1) lectura input
            Vector2 moveInput = _input.move;         // asume StarterAssetsInputs.move
            bool isRunning = _input.sprint;       // asume Sprint viene de Shift

            float speed = isRunning ? RunSpeed : WalkSpeed;

            // 2) calculamos dirección relativa a cámara
            Vector3 camFwd = CinemachineCameraTarget.forward;
            camFwd.y = 0; camFwd.Normalize();
            Vector3 camRight = CinemachineCameraTarget.right;
            camRight.y = 0; camRight.Normalize();

            Vector3 desiredDir = camRight * moveInput.x + camFwd * moveInput.y;

            if (desiredDir.sqrMagnitude >= _threshold)
            {
                desiredDir.Normalize();

                // 3) rotamos personaje:
                bool aiming = _animator.GetBool("IsAiming");
                if (aiming)
                {
                    // al apuntar, giramos según yaw de la cámara
                    Quaternion targetRot = Quaternion.Euler(0, _cinemachineYaw, 0);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRot,
                        RotationSpeed * Time.deltaTime
                    );
                }
                else
                {
                    // en estado normal, giramos hacia la dirección de movimiento
                    Quaternion targetRot = Quaternion.LookRotation(desiredDir);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRot,
                        RotationSpeed * Time.deltaTime
                    );
                }

                // 4) desplazamiento horizontal
                Vector3 horizontalVel = desiredDir * speed;
                // 5) gravedad
                if (_controller.isGrounded)
                    _verticalVelocity = -0.5f;
                else
                    _verticalVelocity += Gravity * Time.deltaTime;

                Vector3 velocity = horizontalVel + Vector3.up * _verticalVelocity;
                _controller.Move(velocity * Time.deltaTime);
            }
            else
            {
                // sin input, solo aplicar gravedad
                if (_controller.isGrounded)
                    _verticalVelocity = -0.5f;
                else
                    _verticalVelocity += Gravity * Time.deltaTime;
                _controller.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
            }
        }

        void HandleAnimations()
        {
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 0.5f;
            Vector3 v = _controller.velocity;
            v.y = 0;
            float speedPct = v.magnitude / RunSpeed;
            _animator.SetFloat("Speed", speedPct, 0.1f, Time.deltaTime);

            bool aiming = Input.GetMouseButton(1);
            //_animator.SetBool("IsAiming", aiming);

            //if (aiming)
            //{
            //    _animator.SetFloat("AimX", _input.look.x, 0.1f, Time.deltaTime);
            //    _animator.SetFloat("AimY", _input.look.y, 0.1f, Time.deltaTime);
            //}
        }

        void HandleCamera()
        {
            // 1) rotación libre de cámara
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float dtMul = IsMouse ? 1f : Time.deltaTime;
                _cinemachineYaw += _input.look.x * LookSpeed * dtMul;
                _cinemachinePitch -= _input.look.y * LookSpeed * dtMul;
            }
            _cinemachinePitch = Mathf.Clamp(_cinemachinePitch, MinPitch, MaxPitch);
            CinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachinePitch, _cinemachineYaw, 0);

            // 2) calculamos posición deseada y evitamos clipping
            Vector3 desiredPos = CinemachineCameraTarget.position
                               + CinemachineCameraTarget.forward * -CameraDistance
                               + Vector3.up * CameraHeight;

            if (Physics.SphereCast(
                CinemachineCameraTarget.position,
                CameraCollisionRadius,
                (desiredPos - CinemachineCameraTarget.position).normalized,
                out RaycastHit hit,
                CameraDistance,
                ObstacleLayers))
            {
                CameraTransform.position = hit.point - CinemachineCameraTarget.forward * CameraCollisionRadius;
            }
            else
            {
                CameraTransform.position = desiredPos;
            }

            CameraTransform.LookAt(CinemachineCameraTarget.position);
        }

        void HandleShooting()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
                _animator.SetTrigger("Fire");
            }
        }

        void Shoot()
        {
            // raycast instantáneo
            Ray ray = new Ray(BarrelEnd.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, BulletRange))
            {
                // impacto: aplicar daño, efectos, etc.
            }
            // opcional: proyectil físico
            if (BulletPrefab)
            {
                GameObject proj = Instantiate(BulletPrefab, BarrelEnd.position,
                                              Quaternion.LookRotation(transform.forward));
                if (proj.TryGetComponent<Rigidbody>(out var rb))
                    rb.velocity = transform.forward * BulletSpeed;
            }
        }

        //void OnAnimatorIK(int layerIndex)
        //{
        //    bool aiming = _animator.GetBool("IsAiming");
        //    if (aiming)
        //    {
        //        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        //        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        //        Vector3 aimPoint = CameraTransform.position + CameraTransform.forward * BulletRange;
        //        _animator.SetIKPosition(AvatarIKGoal.RightHand, aimPoint);
        //        _animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(CameraTransform.forward));
        //    }
        //    else
        //    {
        //        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
        //        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        //    }
        //}
    }


}