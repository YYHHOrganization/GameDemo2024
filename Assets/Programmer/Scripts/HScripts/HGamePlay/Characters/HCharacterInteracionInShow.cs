using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

//这个脚本用于处理角色在展示场景中的交互
public class HCharacterInteracionInShow : MonoBehaviour
{
    private GameObject camera;
    private Camera cameraComponent;
    private Animator animator;
    private int isPickedUpHash;

    private bool isInteractMode;

    private bool isRotated = false;
    private bool isPickedUp = false;
    private Vector3 originPosition;
    

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("PlayerCamera");
        if (camera)
        {
            cameraComponent = camera.GetComponent<Camera>();
        }

        animator = gameObject.GetComponent<Animator>();
        isPickedUpHash = Animator.StringToHash("isPickedUp");
        //Debug.Log("now in start method");
        //note:两个角色都要用到这个脚本，所以这个脚本的Start方法里不能直接吧isInteractMode设置为true，否则切换人之后的模式不正确
        originPosition = transform.position;
    }

    //判断鼠标是否在角色上并且按下
    private void CheckMouseIsOnCharacterAndDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!cameraComponent)
            {
                camera = GameObject.FindGameObjectWithTag("PlayerCamera");
                if (camera)
                {
                    cameraComponent = camera.GetComponent<Camera>();
                }
            }
            
            Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                //Debug.DrawLine(ray.origin,hitInfo.point, Color.red);
                GameObject obj = hitInfo.collider.gameObject;
                if (obj == this.gameObject)
                {
                    //random 0 or 1
                    int animationIndex = UnityEngine.Random.Range(0, 2);
                    switch (animationIndex)
                    {
                        case 0:
                            animator.SetTrigger("StandBy1");
                            break;
                        case 1:
                            animator.SetTrigger("StandBy2");
                            break;
                    }
                    //animator.SetInteger("animationIndex", animationIndex);
                }
            }
        }
    }

    public void SetInteractMode(bool isInteract)
    {
        isInteractMode = isInteract;
        //Debug.Log("isInteractMode" + isInteractMode);
        //Debug.Log("THIS IS " + this.gameObject.name);
    }

    //鼠标拖动角色旋转
    private void CheckCharacterRotation()
    {
        if (Input.GetMouseButton(0))
        {
            isRotated = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isRotated = false;
        }

        if (isRotated)
        {
            float x = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, -x * 250f * Time.deltaTime);
        }
    }

    private void CheckPickCharacterUp()
    {
        if (Input.GetMouseButton(0))
        {
            isPickedUp = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPickedUp = false;
        }

        if (isPickedUp)
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            transform.position += new Vector3(x * 4f * Time.deltaTime, y * 6f * Time.deltaTime, 0);
            if (gameObject.transform.position.y > 0.2f) //被拽到了空中，做对应动作
            {
                animator.SetBool("isPickedUp",true);
            }
            
        }
        else
        {
            //角色回到原位
            //transform.localPosition = new Vector3(0, 0, 0);
            transform.position = originPosition;
            animator.SetBool("isPickedUp",false);
        }
    }
    
    private void Update()
    {
        if (isInteractMode)
        {
            CheckPickCharacterUp();
        }
        else
        {
            //Debug.Log("Update " + isInteractMode + this.gameObject.name);
            CheckMouseIsOnCharacterAndDown();
            CheckCharacterRotation();
        }
        
    }
    
}
