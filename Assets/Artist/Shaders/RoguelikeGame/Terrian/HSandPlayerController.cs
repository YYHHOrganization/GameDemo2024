using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HSandPlayerController : MonoBehaviour
{
    public float speed = 1.5f;
    public float mulSpeed = 2;
    public float jumpH = 1;

    private CharacterController controller;
    private HRGPlayerRutBrush brush;
    private Animator animator;
    private void Start()
    {
        controller = this.GetComponent<CharacterController>();
        brush = this.GetComponent<HRGPlayerRutBrush>();
        animator = this.GetComponentInChildren<Animator>();
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
    }

    private const float g = 9.8f;

    void Update()
    {
        if (controller.isGrounded)
        {
            float currentSpeed = speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = mulSpeed * speed;
            }

            if (Input.GetKey(KeyCode.W))
            {
                controller.SimpleMove(transform.forward * currentSpeed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                controller.SimpleMove(-transform.forward * currentSpeed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                controller.SimpleMove(-transform.right * currentSpeed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                controller.SimpleMove(transform.right * currentSpeed);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.Move((controller.velocity + Mathf.Sqrt(2 * g * jumpH) * Vector3.up) * Time.deltaTime);
            }
        }
        else
        {
            controller.Move((controller.velocity + g * Time.deltaTime * Vector3.down) * Time.deltaTime);
        }

        //向下发射一根射线,检测距离为1
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.3f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                if (brush)
                {
                    brush.Paint();
                }
            }
        }
    }
}
