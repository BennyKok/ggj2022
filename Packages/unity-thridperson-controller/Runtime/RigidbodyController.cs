using System;
using BennyKok.Bootstrap;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Components;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RigidbodyController : MonoBehaviour
{
    [Header("Input")] public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference lookAction;
    public InputActionReference crouchAction;
    public InputActionReference runAction;


    [Header("Camera")] public bool hookToCinemachine;
    public CinemachineBrain cinemachineBrain;
    public Transform customAimTransform;
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;
    public float lookSensitivity = 0.05f;
    public float hLookSensitivity = 0.05f;
    public bool hideCursorOnStart;


    [Header("Movement")] public bool debug;
    public bool isControllable = true;
    public float acceleration = 4f;
    public float deceleration = 4f;
    public float speed = 8.0f;
    public bool instantRotate = false;
    public float rotateSpeed = 540;

    [DebugAction] public float jumpForce = 5f;
    public float jumpDuration = 1.5f;
    [DebugAction] public float stickToGroundForce = 0.5f;

    [Header("Event")] public UnityEvent onJump;


    [Header("Collision")] public LayerMask groundCheckMask;
    public float groupCheckOffset = 0.01f;


    [Header("Status")] public bool grounded;
    [SerializeField] private bool crouched;
    public bool running;

    public bool Crouched
    {
        get => crouched;
        set { crouched = value; }
    }


    private Camera targetCamera;
    [NonSerialized] public Rigidbody targetBody;
    [NonSerialized] public Vector2 moveInput;
    private float distToGround;
    private bool shouldJump;
    private bool previousIsJump;
    private bool previousIsJumpPadJump;
    private bool confirmInJump;
    private Vector2 movement;
    private Vector2 lookAxis;
    [NonSerialized] public Vector2 mobileLookAxis;
    private CapsuleCollider mCollider;
    private Vector3 mColliderCenter;
    private float mColliderHeight;
    private float jumpTime = -1;

    public bool IsAiming
    {
        get => isAiming;
        set
        {
            isAiming = value;
            // characterAnimator.OnAim(value);
        }
    }

    [NonSerialized] private bool isAiming;

    [System.NonSerialized] public CharacterAnimator characterAnimator;

    #region LIFECYCLE

    protected void Awake()
    {
        // base.Awake();

        originalScene = gameObject.scene;

        if (hookToCinemachine)
            CinemachineCore.GetInputAxis = GetInputLookAxis;

        targetCamera = Camera.main;
        targetBody = GetComponent<Rigidbody>();
        mCollider = GetComponent<CapsuleCollider>();
        characterAnimator = GetComponent<CharacterAnimator>();

        mColliderHeight = mCollider.height;
        mColliderCenter = mCollider.center;

        moveAction.action.performed += (context) =>
        {
            // Debug.Log(context.action.ReadValue<Vector2>().normalized);
            moveInput = context.action.ReadValue<Vector2>().normalized;
        };
        moveAction.action.canceled += (context) => moveInput = Vector2.zero;

        if (crouchAction)
            crouchAction.action.performed += (context) => { Crouched = !Crouched; };

        if (runAction)
        {
            runAction.action.performed += ctx => running = true;
            runAction.action.canceled += ctx => running = false;
        }

        jumpAction.action.performed += (context) =>
        {
            if (grounded)
                shouldJump = context.action.triggered ? true : shouldJump;
        };
    }

    public void Jump()
    {
        if (grounded)
            shouldJump = true;
    }

    private void Start()
    {
        distToGround = mCollider.bounds.extents.y;

        grounded = IsGrounded();

        if (hideCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        //isAiming && 
        if (customAimTransform)
        {
            xAxis.Update(Time.deltaTime);
            yAxis.Update(Time.deltaTime);

            customAimTransform.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();

        if (rotatingTowardsDirection)
        {
            if (RotateTowards(rotateDirection)) rotatingTowardsDirection = false;
        }
    }

    #endregion

    #region PLAYER_INPUT

    public float GetInputLookAxis(string axisName)
    {
        if (Time.timeScale == 0) return 0;

        // && !(cinemachineBrain.ActiveBlend.CamA is CinemachineFreeLook && cinemachineBrain.ActiveBlend.CamB is CinemachineFreeLook)
        if (cinemachineBrain.IsBlending)
            return 0;

        lookAxis = lookAction.action.ReadValue<Vector2>();

        // if (DisableOnNonMobilePlatform.IsMobilePlatform && EventSystem.current.IsPointerOverGameObject()) return 0;

        //https://forum.unity.com/threads/mouse-delta-input.646606/
        // Account for sensitivity setting on old Mouse X and Y axes.
        if (DisableOnNonMobilePlatform.IsMobilePlatform)
        {
            lookAxis = mobileLookAxis;
        }

        if (axisName == "Mouse X")
            return lookAxis.x * lookSensitivity;
        else if (axisName == "Mouse Y")
            return lookAxis.y * hLookSensitivity;

        return 0;
    }

    #endregion

    private bool IsGrounded(float extraHeight = 0)
    {
        //We do a capsule 0.1 below the character to see if the player is above something
        var start = mCollider.bounds.center;
        var end = start;
        end.y = mCollider.bounds.min.y - groupCheckOffset - extraHeight;
        Debug.DrawLine(start, end, Color.green, groupCheckOffset + extraHeight);
        return Physics.CheckCapsule(start, end, mCollider.radius, groundCheckMask, QueryTriggerInteraction.Ignore);
    }

    private bool IsStepBeneath() => IsGrounded(stairHeight);

    public float stairHeight = 0.3f;
    public float toeHeight = 0.1f;
    public float stairPushUpForce = 2f;

    private bool toeHit;
    private bool kneeHit;
    private bool isMovingForward;
    private bool isMovingApproxForward;
    private bool isMovingApproxForward2;
    private bool previousIsStairPush;

    private float stairPushCoolDownTime;

    private bool rotatingTowardsDirection;
    private float jumpPadJumpForce;
    private float jumpPadForwardForce;
    private Vector3 rotateDirection;

    private void OnGUI()
    {
        if (!debug) return;

        GUILayout.BeginVertical("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label(
            $"Toehit {toeHit}, kneeHit {kneeHit}, isMovingForward {isMovingForward}, isMovingApproxForward2 {isMovingApproxForward2}, isMovingApproxForward {isMovingApproxForward}");
        GUILayout.EndHorizontal();
    }

    public void FaceCameraDirection()
    {
        rotatingTowardsDirection = true;

        var forward = targetCamera.transform.forward;

        forward.y = 0;
        forward.Normalize();

        rotateDirection = forward;
    }

    private Scene originalScene;

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(other.transform);
        }
        else
        {
            transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(gameObject, originalScene);
        }
    }

    public void UpdateMovement()
    {
        //Calculating camera direction
        var forward = targetCamera.transform.forward;
        var right = targetCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        if (!crouched)
        {
            mCollider.height = mColliderHeight;
            mCollider.center = mColliderCenter;
        }
        else
        {
            mCollider.height = mColliderHeight / 2;

            var center = mColliderCenter;
            center.y /= 2f;
            mCollider.center = center;
        }

        //Get input
        movement = Vector2.Lerp(movement, moveInput,
            (moveInput.sqrMagnitude > 0 ? acceleration : deceleration) * Time.deltaTime);
        // Debug.Log(moveInput.sqrMagnitude > 0 ? "acceleration" : "deceleration");
        var nextGrounded = IsGrounded();
        if (Time.time - jumpTime > 0.05f || nextGrounded)
            grounded = nextGrounded;

        float v1 = Vector3.Dot(targetBody.transform.TransformDirection(new Vector3(movement.x, 0, movement.y)),
            targetBody.transform.forward);
        isMovingForward = v1 >= 0.9f;
        isMovingApproxForward = v1 >= 0f;
        isMovingApproxForward2 = v1 >= 0.5f;

        var start = mCollider.bounds.center + targetBody.transform.forward * (mCollider.radius - 0.1f);
        var forwardCheckDistance = 0.3f;

        start.y = mCollider.bounds.min.y + toeHeight;

        var second = start;
        second.y += stairHeight;

        Debug.DrawLine(start, start + targetBody.transform.forward * forwardCheckDistance, Color.green, 0.1f);
        Debug.DrawLine(second, second + targetBody.transform.forward * forwardCheckDistance, Color.green, 0.1f);

        toeHit = Physics.Raycast(start, targetBody.transform.forward, forwardCheckDistance, groundCheckMask);
        kneeHit = Physics.Raycast(second, targetBody.transform.forward, forwardCheckDistance, groundCheckMask);

        if (previousIsStairPush && Time.time > stairPushCoolDownTime && IsStepBeneath())
        {
            previousIsStairPush = false;
            jumpTime += 0.5f;
        }

        if (!previousIsJump && grounded && movement.magnitude > 0.08f && toeHit && !kneeHit && isMovingApproxForward)
        {
            //Hit stair
            jumpTime = Time.time;
            targetBody.AddForce(targetBody.transform.up * stairPushUpForce, ForceMode.VelocityChange);
            previousIsStairPush = true;
            stairPushCoolDownTime = Time.time + 0.1f;
            grounded = true;
        }

        if (characterAnimator)
            characterAnimator.OnGrounded(grounded);

        var currentVelocity = targetBody.velocity;
        var moveDirection = forward * movement.y + right * movement.x;

        var newVelVelocity = Vector3.zero;
        var newVelVelocityDelta = Vector3.zero;

        if (isControllable)
        {
            if (characterAnimator)
                characterAnimator.OnCrouched(Crouched);
            if (moveInput.magnitude > 0.05f)
            {
                if (characterAnimator)
                    characterAnimator.OnMove(Mathf.MoveTowards(characterAnimator.GetMoveSpeed(),
                        moveInput.magnitude * (running ? 1f : 0.5f), Time.deltaTime / 0.2f));

                // if (!IsAiming)
                RotateTowards(moveDirection);
                // else
                // RotateTowards(forward);

                rotatingTowardsDirection = false;
                newVelVelocity = moveDirection * (crouched ? speed / 2 : speed) * (running ? 1.5f : 1f);
                newVelVelocity.y = 0;

                var horizontalVelDelta = (newVelVelocity - currentVelocity);
                horizontalVelDelta.y = 0;
                newVelVelocityDelta = horizontalVelDelta;

                if (!((toeHit || kneeHit)))
                {
                    targetBody.AddForce(newVelVelocityDelta, ForceMode.VelocityChange);
                }
            }
            else
            {
                if (characterAnimator)
                    characterAnimator.OnMove(Mathf.MoveTowards(characterAnimator.GetMoveSpeed(), 0,
                        Time.deltaTime / 0.2f));

                targetBody.angularVelocity = Vector3.zero;
            }

            //Handle jumpping
            if (grounded && !previousIsStairPush)
            {
                if (shouldJump)
                {
                    if (characterAnimator)
                        characterAnimator.OnJump();
                    onJump.Invoke();
                    jumpTime = Time.time;
                    targetBody.AddForce(targetBody.transform.up * jumpForce, ForceMode.VelocityChange);
                    previousIsJump = true;
                }
                else if (confirmInJump)
                {
                    confirmInJump = false;
                    previousIsJump = false;
                    previousIsJumpPadJump = false;
                }
            }
            else if (previousIsJump)
            {
                confirmInJump = true;
            }

            shouldJump = false;
        }

        //Apply stick to ground force
        if ((targetBody.velocity.y < 0 || !previousIsJump) && !previousIsJumpPadJump)
        {
            targetBody.AddForce(Vector3.up * Physics.gravity.y * stickToGroundForce * Time.deltaTime,
                ForceMode.VelocityChange);
        }

        if (targetBody.velocity.y > 0)
        {
            // targetBody.AddForce(Vector3.up * Physics.gravity.y * stickToGroundForce * Time.deltaTime, ForceMode.VelocityChange);
        }

        if (previousIsJumpPadJump)
        {
            if (Time.time - jumpTime < jumpDuration)
            {
                var p = Camera.main.transform.forward;
                p.y = 0;
                targetBody.AddForce(
                    (targetBody.transform.up * jumpPadJumpForce + p * jumpPadForwardForce) * Time.deltaTime,
                    ForceMode.VelocityChange);
            }
            else
            {
                targetBody.AddForce(Vector3.up * Physics.gravity.y * stickToGroundForce * Time.deltaTime,
                    ForceMode.VelocityChange);
            }
        }

        var vel = targetBody.velocity;
        vel.y = Mathf.Min(vel.y, speed);
        targetBody.velocity = vel;
        // targetBody.velocity = Vector3.ClampMagnitude(targetBody.velocity, speed);
    }

    private bool RotateTowards(Vector3 moveDirection)
    {
        var degreesPerSecond = rotateSpeed;
        if (Mathf.Abs(Quaternion.LookRotation(moveDirection).eulerAngles.y - transform.eulerAngles.y) >= 90)
        {
            degreesPerSecond *= 1.5f;
        }

        Quaternion to = Quaternion.LookRotation(moveDirection);
        

        if (instantRotate)
        {
            targetBody.MoveRotation(to);
        }
        else
        {
            targetBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, to, degreesPerSecond * Time.deltaTime));
        }

        return transform.rotation == to;
    }

    public void JumpPad(float force, float forwardForce)
    {
        characterAnimator.OnJump();
        onJump.Invoke();
        jumpTime = Time.time;
        // targetBody.AddForce(targetBody.transform.up * force, ForceMode.VelocityChange);
        previousIsJump = true;
        jumpPadJumpForce = force;
        jumpPadForwardForce = forwardForce;
        previousIsJumpPadJump = true;
    }
}