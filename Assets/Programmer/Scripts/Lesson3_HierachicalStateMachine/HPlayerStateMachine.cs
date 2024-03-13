using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

public class HPlayerStateMachine : MonoBehaviour
{
    L2PlayerInput playerInput;
    private CharacterController characterController;

    private Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    private Vector3 appliedMovement;
    private Vector3 _cameraRelativeMovement;
    private bool isMovementPressed;

    private Animator animator;
    private int isWalkingHash;
    private int isRunningHash;
    private int isJumpingHash;
    private float rotationFactorPerFrame = 15.0f;

    private bool isRunPressed;
    float runMultiplier = 5.0f;

    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight = 2f;
    private float maxJumpTime = 0.75f;
    private bool isJumping = false;
    private bool _requireNewJumpPress = false;
    
    float gravity = -9.8f;
    float groundedGravity = -0.05f;
    
    int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>(); 
    Dictionary<int, float> jumpGravities = new Dictionary<int, float>();

    private Coroutine currentJumpResetRoutine = null;
    private int jumpCountHash;

    private HPlayerBaseState _currentState;
    private HPlayerStateFactory _states;
    
    public Camera playerCamera;

    #region Gets and Sets
    
    public bool IsJumpPressed
    {
        get { return isJumpPressed; }
    }
    
    public HPlayerBaseState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }
    
    public Animator Animator
    {
        get { return animator; }
    }
    
    public Coroutine CurrentJumpResetRoutine
    {
        get { return currentJumpResetRoutine; }
        set { currentJumpResetRoutine = value; }
    }
    
    public Dictionary<int, float> InitialJumpVelocities
    {
        get { return initialJumpVelocities; }
    }
    
    public int JumpCount
    {
        get { return jumpCount; }
        set { jumpCount = value; }
    }
    
    public int IsJumpingHash
    {
        get { return isJumpingHash; }
    }
    
    public int JumpCountHash
    {
        get { return jumpCountHash; }
    }
    
    public bool RequireNewJumpPress
    {
        get { return _requireNewJumpPress; }
        set { _requireNewJumpPress = value; }
    }
    
    public bool IsJumping
    {
        set { isJumping = value; }
    }
    
    public float CurrentMovementY
    {
        get { return currentMovement.y; }
        set { currentMovement.y = value; }
    }
    
    public float AppliedMovementY
    {
        get { return appliedMovement.y; }
        set { appliedMovement.y = value; }
    }
    
    public float GroundedGravity
    {
        get { return groundedGravity; }
    }
    
    public CharacterController CharacterController
    {
        get { return characterController; }
    }
    
    public Dictionary<int, float> JumpGravities
    {
        get { return jumpGravities; }
    }
    
    public bool IsMovementPressed
    {
        get { return isMovementPressed; }
    }
    
    public bool IsRunPressed
    {
        get { return isRunPressed; }
    }
    
    public int IsWalkingHash
    {
        get { return isWalkingHash; }
    }
    
    public int IsRunningHash
    {
        get { return isRunningHash; }
    }
    
    public float AppliedMovementX
    {
        get { return appliedMovement.x; }
        set { appliedMovement.x = value; }
    }
    
    public float AppliedMovementZ
    {
        get { return appliedMovement.z; }
        set { appliedMovement.z = value; }
    }
    
    public float RunMultiplier
    {
        get { return runMultiplier; }
    }
    
    public Vector2 CurrentMovementInput
    {
        get { return currentMovementInput; }
    }
    
    public float Gravity
    {
        get { return gravity; }
    }
    
    #endregion
    
    private void Awake()
    {
        _states = new HPlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        
        playerInput = new L2PlayerInput();
        characterController = GetComponent<CharacterController>();
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        
        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;

        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;
        animator = GetComponent<Animator>();
        
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        jumpCountHash = Animator.StringToHash("jumpCount");
        
        SetupJumpVaraibles();
    }

    private void Start()
    {
        characterController.Move(appliedMovement * Time.deltaTime);
    }

    //处理角色的旋转逻辑，使角色面向移动方向，todo：使用Cinemachine之后这种旋转逻辑可能要进行修改
    void HandleRotation() 
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _cameraRelativeMovement.z;
        
        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed)
        {
            //Debug.Log("dddddddddddddd");
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationFactorPerFrame);
        }
        
    }

    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;
        
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;
        //create direction-relative input vectors, 也就是说在相机的前方和右方的投影
        Vector3 forwardRelativeVerticalInput = vectorToRotate.z * forward;
        Vector3 rightRelativeHorizontalInput = vectorToRotate.x * right;
            
        //create camera-relative movement
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        cameraRelativeMovement.y = currentYValue;
        return cameraRelativeMovement;
    }
    
    void SetupJumpVaraibles()
    {
        float timeToApex = maxJumpTime / 2.0f;
        float initialGravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        
        float secondJumpGravity = (-2 * (maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * (maxJumpHeight + 2)) / (timeToApex * 1.25f);
        
        float thirdJumpGravity = (-2 * (maxJumpHeight + 3)) / Mathf.Pow((timeToApex * 1.5f), 2);
        float thirdJumpInitialVelocity = (2 * (maxJumpHeight + 3)) / (timeToApex * 1.5f);
        
        initialJumpVelocities.Add(1, initialJumpVelocity);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        initialJumpVelocities.Add(3, thirdJumpInitialVelocity);
        
        jumpGravities.Add(0, initialGravity); //设置一个0是为了处理当jumpCount reset到0的情况
        jumpGravities.Add(1, initialGravity);
        jumpGravities.Add(2, secondJumpGravity);
        jumpGravities.Add(3, thirdJumpGravity);
    }
    
    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }
    
    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    
    public void SetInputActionDisableOrEnable(bool shouldLock)
    {
        if (shouldLock)
        {
            playerInput.CharacterControls.Disable();
        }
        else
        {
            playerInput.CharacterControls.Enable();
        }
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
    
    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates(); //逻辑上，先Update自己，有substate的话再Update Substate
        _cameraRelativeMovement = ConvertToCameraSpace(appliedMovement);
        characterController.Move(_cameraRelativeMovement * Time.deltaTime);
    }
}
