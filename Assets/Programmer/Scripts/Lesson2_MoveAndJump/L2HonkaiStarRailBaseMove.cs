using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class L2HonkaiStarRailBaseMove : MonoBehaviour
{
    L2HonkaiStarRailBase playerInput;
    private CharacterController characterController;

    private Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    private bool isMovementPressed;

    private Animator animator;
    private int isWalkingHash;
    private int isRunningHash;
    private float rotationFactorPerFrame = 15.0f;

    private bool isRunPressed;
    float runMultiplier = 3.0f;

    public GameObject runIcon;
    
    private void Awake()
    {
        playerInput = new L2HonkaiStarRailBase();
        characterController = GetComponent<CharacterController>();
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        
        playerInput.CharacterControls.Run.started += OnRun;
        //playerInput.CharacterControls.Run.canceled += OnRun;
        
        animator = GetComponent<Animator>();
        
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        
        runIcon.gameObject.SetActive(isRunPressed);
    }

    void OnRun(InputAction.CallbackContext context)
    {
        //如果context是按下的状态
        if(context.phase == InputActionPhase.Started)
        {
            isRunPressed = !isRunPressed;
            runIcon.gameObject.SetActive(isRunPressed);
        }
        //isRunPressed = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -0.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y += gravity * Time.deltaTime;
            currentRunMovement.y += gravity * Time.deltaTime;
        }
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        
        if (isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash,true);
        } 
        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash,false);
        }
        
        if((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash,true);
        }
        else if((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash,false);
            isRunPressed = false;
            runIcon.gameObject.SetActive(isRunPressed);
        }
        
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = currentMovement.z;
        
        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationFactorPerFrame);
        }
        
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleAnimation();

        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
        HandleGravity(); //值得注意的是handleGravity要写在Move的下面，因为要先Move才能判断是否isGrounded
    }
}
