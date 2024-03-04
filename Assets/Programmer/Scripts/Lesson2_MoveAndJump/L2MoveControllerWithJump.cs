using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class L2MoveControllerWithJump : MonoBehaviour
{
    L2PlayerInput playerInput;
    private CharacterController characterController;

    private Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    private Vector3 appliedMovement;
    private bool isMovementPressed;

    private Animator animator;
    private int isWalkingHash;
    private int isRunningHash;
    private int isJumpingHash;
    private float rotationFactorPerFrame = 15.0f;

    private bool isRunPressed;
    float runMultiplier = 3.0f;

    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight = 2f;
    private float maxJumpTime = 0.75f;
    private bool isJumping = false;
    private bool isJumpAnimating = false;
    
    float gravity = -9.8f;
    float groundedGravity = -0.05f;
    
    int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>(); 
    Dictionary<int, float> jumpGravities = new Dictionary<int, float>();

    private Coroutine currentJumpResetRoutine = null;
    private int jumpCountHash;
    
    private void Awake()
    {
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

    void SetupJumpVaraibles()
    {
        float timeToApex = maxJumpTime / 2.0f;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        
        float secondJumpGravity = (-2 * (maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * (maxJumpHeight + 2)) / (timeToApex * 1.25f);
        
        float thirdJumpGravity = (-2 * (maxJumpHeight + 3)) / Mathf.Pow((timeToApex * 1.5f), 2);
        float thirdJumpInitialVelocity = (2 * (maxJumpHeight + 3)) / (timeToApex * 1.5f);
        
        initialJumpVelocities.Add(1, initialJumpVelocity);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        initialJumpVelocities.Add(3, thirdJumpInitialVelocity);
        
        jumpGravities.Add(0, gravity); //设置一个0是为了处理当jumpCount reset到0的情况
        jumpGravities.Add(1, gravity);
        jumpGravities.Add(2, secondJumpGravity);
        jumpGravities.Add(3, thirdJumpGravity);
    }

    IEnumerator jumpResetRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        jumpCount = 0;
    }
    
    void HandleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            if (jumpCount <= 3 && currentJumpResetRoutine != null)
            {
                StopCoroutine(currentJumpResetRoutine);
            }
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            jumpCount += 1;
            animator.SetInteger(jumpCountHash, jumpCount);
            currentMovement.y = initialJumpVelocities[jumpCount];
            appliedMovement.y = initialJumpVelocities[jumpCount];
        } else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }
    
    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
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
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
                currentJumpResetRoutine = StartCoroutine(jumpResetRoutine());
                if (jumpCount == 3)
                {
                    jumpCount = 0;
                    animator.SetInteger(jumpCountHash, jumpCount);
                }
            }
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = previousYVelocity + (jumpGravities[jumpCount] * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -20.0f);
        }
        else  //起跳上升的过程中
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = previousYVelocity + (jumpGravities[jumpCount] * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * 0.5f;
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
    
    void Update()
    {
        HandleRotation();
        HandleAnimation();

        if (isRunPressed)
        {
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
        }
        else
        {
            appliedMovement.x = currentMovement.x;
            appliedMovement.z = currentMovement.z;
        }
        characterController.Move(appliedMovement * Time.deltaTime);
        HandleGravity(); //值得注意的是handleGravity要写在Move的下面，因为要先Move才能判断是否isGrounded
        HandleJump();
    }
}
