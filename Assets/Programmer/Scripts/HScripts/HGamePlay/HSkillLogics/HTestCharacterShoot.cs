using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HTestCharacterShoot : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera thirdAimCamera;
    bool thirdAimCameraActive = false;
    private Cinemachine.CinemachineVirtualCamera c_thirdPersonCam;
    public GameObject testCommonThirdPersonFollowCam;
    public Camera mainPlayerCamera;
    public Transform thirdPersonFollowPlace;
    //public Transform aimLocateSpace;
    private HPlayerStateMachine stateMachine;
    
    public Transform thirdPersonCommonFollowPlace;
    public Transform aimTargetReticle;
    public GameObject effectToSpawn;

    public Transform gunTrans;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // if(mainPlayerCamera==null)
        //     mainPlayerCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
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
            StopAllCoroutines();
        }
        RotateCharacterWithMouse2();
        
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
    
    private void ShootBulletFromMuzzle(bool needShootHelp,bool hitButNoNeedHelp, Vector3 hitPosition)
    {
        //由枪口位置向屏幕中心所指的位置发射子弹
        GameObject Effects;
        if (thirdPersonFollowPlace)
        {
            // Debug.Log("ShootBulletFromMuzzle");
            //
            float middleX = Screen.width * 0.5f;
            float middleY = Screen.height * 0.5f;
            Ray shotRay = mainPlayerCamera.ScreenPointToRay(new Vector3(middleX, middleY, 0));
            Vector3 shotDir = shotRay.direction;
            //rotation is from gunTrans.rotation to shotDir

            // Effects = Instantiate(effectToSpawn, gunTrans.position, Quaternion.identity, this.transform);
            if (needShootHelp || hitButNoNeedHelp)
            {
                //打到怪，近似处理，辅助瞄准
                Vector3 dir = hitPosition - gunTrans.position;
                //instantiate effectToSpawn at gunTrans.position, with rotation to shotDir
                Effects = Instantiate(effectToSpawn, gunTrans.position, Quaternion.LookRotation(dir), this.transform);
                Destroy(Effects, 10f);
            }
            else {
                //没有打到任何东西，进入这个逻辑
                Effects = Instantiate(effectToSpawn, gunTrans.position, thirdPersonFollowPlace.rotation);
                Destroy(Effects, 10f);
            }
            
        }
    }
    
    IEnumerator CalculateScreenRayForShoot()
    {
        aimTargetReticle.gameObject.SetActive(true);
        GameObject historyObject = gameObject;
        //中心位置射出9条射线，检测射中的物体
        while (true)
        {
            Vector3 hitPosition = new Vector3();
            float middleX = Screen.width * 0.5f;
            float middleY = Screen.height * 0.5f;
            float deltaX = Screen.width * 0.05f;
            float deltaY = Screen.height * 0.05f;
            bool needShootHelp = false;
            bool hitButNoNeedHelp = false;
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
                            hitPosition = hit.point;
                            //draw ray
                            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                            Debug.Log(hit.collider.gameObject.name);
                            if(hit.collider.gameObject != historyObject)
                            {
                                historyObject = hit.collider.gameObject;
                                //aimTargetReticle.position = mainPlayerCamera.WorldToScreenPoint(hit.transform.position);
                            }
                            needShootHelp = true;
                            continue;
                        }
                    }
                }
                if(needShootHelp)
                    continue;
            }
            //使用hitSphere 检测射中的物体
            // if(Physics.SphereCast(mainPlayerCamera.transform.position, 0.1f, mainPlayerCamera.transform.forward, out RaycastHit hit, 100))
            // {
            //     if (hit.collider.gameObject.CompareTag("Enemy"))
            //     {
            //         hitPosition = hit.point;
            //         //draw ray
            //         Debug.DrawRay(mainPlayerCamera.transform.position, mainPlayerCamera.transform.forward * 100, Color.red);
            //         Debug.Log(hit.collider.gameObject.name);
            //         if(hit.collider.gameObject != historyObject)
            //         {
            //             historyObject = hit.collider.gameObject;
            //             //aimTargetReticle.position = mainPlayerCamera.WorldToScreenPoint(hit.transform.position);
            //         }
            //         needShootHelp = true;
            //     }
            // }
            
            Ray middleRay = mainPlayerCamera.ScreenPointToRay(new Vector3(middleX, middleY, 0));
            RaycastHit hit2;
            if (Physics.Raycast(middleRay, out hit2, 100))
            {
                if (!needShootHelp)
                {
                    hitButNoNeedHelp = true;
                    hitPosition = hit2.point;
                }
            }
            ShootBulletFromMuzzle(needShootHelp,hitButNoNeedHelp,hitPosition);
            yield return new WaitForSeconds(0.1f);
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
