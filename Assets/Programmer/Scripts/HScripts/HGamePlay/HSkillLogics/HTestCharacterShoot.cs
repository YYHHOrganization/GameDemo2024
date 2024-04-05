using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTestCharacterShoot : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera thirdAimCamera;
    bool thirdAimCameraActive = false;
    private Cinemachine.CinemachineVirtualCamera c_thirdPersonCam;
    public GameObject testCommonThirdPersonFollowCam;
    private Camera mainPlayerCamera;
    public Transform thirdPersonFollowPlace;
    //public Transform aimLocateSpace;
    private HPlayerStateMachine stateMachine;
    
    public Transform thirdPersonCommonFollowPlace;
    public Transform aimTargetReticle;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //mainPlayerCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        SetCommonThirdPersonFollowCamera(testCommonThirdPersonFollowCam);
        stateMachine = gameObject.GetComponent<HPlayerStateMachine>();
        stateMachine.SetInThirdPersonCamera(true);
        aimTargetReticle.gameObject.SetActive(false);
    }

    public void SetCommonThirdPersonFollowCamera(GameObject virtualCamera)
    {
        c_thirdPersonCam = virtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }
    //set mainPlayerCamera
    public void SetMainPlayerCamera(Camera camera)
    {
        mainPlayerCamera = camera;
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //FaceToPlayerCameraDirection();
            thirdAimCamera.gameObject.SetActive(true);
            StartCoroutine(CalculateScreenRayForShoot());
        }
        else if (Input.GetMouseButton(0))
        {
            //PrepareForShoot();
        } else if(Input.GetMouseButtonUp(0))
        {
            thirdAimCamera.gameObject.SetActive(false);
            aimTargetReticle.gameObject.SetActive(false);
            //FaceToPlayerCameraDirection();
            StopCoroutine(CalculateScreenRayForShoot());
        }
        RotateCharacterWithMouse2();
        
    }
    
    private void PrepareForShoot()
    {
        RotateCharacterWithMouse2();
    }

    private void RotateCharacterWithMouse()
    {
        
        float fMouseX = Input.GetAxis("Mouse X");
        float fMouseY = Input.GetAxis("Mouse Y");
        //thirdPersonFollowPlace.Rotate(Vector3.up, fMouseX, Space.World);//左右旋转
        //thirdPersonFollowPlace.transform.position = aimLocateSpace.position;
        thirdPersonFollowPlace.Rotate(Vector3.right, -fMouseY * 2, Space.World);//上下旋转
        transform.Rotate(Vector3.up, fMouseX, Space.World);//左右旋转
    }

    float TurnSpeed = 3;
    float VerticalRotMin = -80;
    float VerticalRotMax = 80;
    
    void RotateCharacterWithMouse2()
    {
        var rotInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        var rot = transform.eulerAngles;
        var rot2 = transform.eulerAngles;
        rot.y += rotInput.x * TurnSpeed;
        rot2.y += rotInput.x * TurnSpeed * 0.8f;
        transform.rotation = Quaternion.Euler(rot);

        if (thirdPersonFollowPlace != null && thirdPersonCommonFollowPlace != null)
        {
            rot = thirdPersonFollowPlace.localRotation.eulerAngles;
            rot2 = thirdPersonCommonFollowPlace.localRotation.eulerAngles;
            rot.x -= rotInput.y * TurnSpeed;
            rot2.x -= rotInput.y * TurnSpeed * 0.8f;
            if (rot.x > 180)
                rot.x -= 360;
            if (rot2.x > 180)
                rot2.x -= 360;
            rot.x = Mathf.Clamp(rot.x, VerticalRotMin, VerticalRotMax);
            rot2.x = Mathf.Clamp(rot2.x, VerticalRotMin+20, VerticalRotMax-20);
            thirdPersonFollowPlace.localRotation = Quaternion.Euler(rot);
            thirdPersonCommonFollowPlace.localRotation = Quaternion.Euler(rot2);
        }
        
    }

    private void FaceToPlayerCameraDirection()
    {
        //直接朝向相机看向的位置，再射出射线
        Vector3 positionToLookAt;
        positionToLookAt.x = mainPlayerCamera.transform.forward.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = mainPlayerCamera.transform.forward.z;
        
        Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt,Vector3.up);
        transform.rotation = targetRotation;
    }
    
    IEnumerator CalculateScreenRayForShoot()
    {
        aimTargetReticle.gameObject.SetActive(true);
        GameObject historyObject = gameObject;
        //中心位置射出9条射线，检测射中的物体
        while (true)
        {
            float middleX = Screen.width * 0.5f;
            float middleY = Screen.height * 0.5f;
            float deltaX = Screen.width * 0.05f;
            float deltaY = Screen.height * 0.05f;
            bool isHit = false;
            //中心位置射出9条射线，检测射中的物体
            for(int i=-1; i<=1; i++)
            {
                for(int j=-1; j<=1; j++)
                {
                    Ray ray = mainPlayerCamera.ScreenPointToRay(new Vector3(middleX + i * deltaX, middleY + j * deltaY, 0));
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 100))
                    {
                        if (hit.collider.gameObject.CompareTag("Enemy"))
                        {
                            //draw ray
                            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                            Debug.Log(hit.collider.gameObject.name);
                            if(hit.collider.gameObject != historyObject)
                            {
                                historyObject = hit.collider.gameObject;
                                aimTargetReticle.position = mainPlayerCamera.WorldToScreenPoint(hit.transform.position);
                            }
                            isHit = true;
                            continue;
                        }
                    }
                }

                if (isHit)
                    continue;
            }

            yield return new WaitForSeconds(0.2f);
        }
        
    }
    
    
    private void CalculateScreenRayForShootManyRays()
    {
        float middleX = Screen.width * 0.5f;
        float middleY = Screen.height * 0.5f;
        float deltaX = Screen.width * 0.05f;
        float deltaY = Screen.height * 0.05f;
        //中心位置射出9条射线，检测射中的物体
        for(int i=-1; i<=1; i++)
        {
            for(int j=-1; j<=1; j++)
            {
                Ray ray = mainPlayerCamera.ScreenPointToRay(new Vector3(middleX + i * deltaX, middleY + j * deltaY, 0));
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 10))
                {
                    if (hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        //draw ray
                        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
                        Debug.Log(hit.collider.gameObject.name);
                    }
                }
            }
        }
    }
}
