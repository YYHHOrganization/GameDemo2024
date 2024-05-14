using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class HRogueGrabberLogic : MonoBehaviour
{
    private GameObject selectedObject;
    private Camera currentCamera;
    private float transformY;
    private int rotationCnt;
    private int currentRotationCnt = 0;
    HRoguePuzzleGameLogic puzzleGameLogic;
    public CinemachineVirtualCamera virtualCamera;
    private void Start()
    {
        //get current camera
        if (HRogueCameraManager.Instance != null)
        {
            currentCamera = HRogueCameraManager.Instance.CurrentActiveCamera.GetComponent<Camera>();
        }
        else
        {
            currentCamera = Camera.main;
        }
        
        transformY = transform.position.y;
        puzzleGameLogic = GetComponent<HRoguePuzzleGameLogic>();
    }
    
    // Update is called once per frame
    void Update()
    {
        CheckMouseClick();
    }

    private bool isInRightSpace = false;
    private void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if(selectedObject == null)  
            {
                RaycastHit hit = CastRay();

                if(hit.collider != null) 
                {
                    Debug.Log("hit.collider.tag: " + hit.collider.name);
                    if (!hit.collider.CompareTag("DragThing")) 
                    {
                        return;
                    }

                    selectedObject = hit.collider.gameObject;
                    Cursor.visible = false;
                }
            } 
            else  //如果
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, currentCamera.WorldToScreenPoint(selectedObject.transform.position).z);
                Vector3 worldPosition = currentCamera.ScreenToWorldPoint(position);
                selectedObject.transform.position = new Vector3(worldPosition.x, transformY + 0.1f, worldPosition.z);

                selectedObject = null;
                Cursor.visible = true;
            }
        }

        if(selectedObject != null)  //如果选中了一个物体，右键可以让其旋转
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, currentCamera.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = currentCamera.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, transformY + 0.1f, worldPosition.z);

            if (Input.GetMouseButtonDown(1)) 
            {
                selectedObject.transform.rotation = Quaternion.Euler(new Vector3(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 90f,
                    selectedObject.transform.rotation.eulerAngles.z));
                currentRotationCnt = puzzleGameLogic.GetCurrentRotationCnt(selectedObject.name);
                currentRotationCnt++;
                currentRotationCnt %= 4;
                
                Debug.Log("currentRotationCnt: " + currentRotationCnt);
                puzzleGameLogic.SetRotationCnt(selectedObject.name, currentRotationCnt);
                puzzleGameLogic.DebugRotationCnt(selectedObject.name);
            }
        }

        if (puzzleGameLogic)
        {
            if (puzzleGameLogic.CheckIfAnswerIsRight(selectedObject)) //如果拼图正确，就显示绿色边框，否则显示黄色边框
            {
                isInRightSpace = true;
            }
            else
            {
                isInRightSpace = false;
            }
        }
        
        if (isInRightSpace)
        {
            if(selectedObject == null)
            {
                return;
            }
            //把碎片放到正确的位置
            puzzleGameLogic.SetPuzzleToCorrectPlace(selectedObject);
            selectedObject.tag = "Untagged";
            isInRightSpace = false;
            selectedObject = null;
            Cursor.visible = true;
        }
        
    }
    
    private RaycastHit CastRay() 
    {
        //这样写射线检测的原因在于，使用相机的裁剪面来确定射线的起点和终点，可以确保射线在屏幕上可见的范围内进行检测，而不会在屏幕外浪费性能(GPT写的)。
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            currentCamera.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            currentCamera.nearClipPlane);
        Vector3 worldMousePosFar = currentCamera.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = currentCamera.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
    
}
