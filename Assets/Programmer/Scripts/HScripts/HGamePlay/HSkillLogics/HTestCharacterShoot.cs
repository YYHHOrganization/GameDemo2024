using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

//
enum MyShootEnum
{
    ShootFromMuzzle,
    ShootLaserFromMuzzle
}
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
    public GameObject[] effectToSpawn;
    private LayerMask layerMask;

    public Transform gunTrans;

    public GameObject muzzleToSpawn;

    private ParticleSystem muzzleVFX;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // if (GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>())
        // {
        //     SetMainPlayerCamera(GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>());
        // }
        // if(mainPlayerCamera==null)
        //     mainPlayerCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        SetCommonThirdPersonFollowCamera(testCommonThirdPersonFollowCam);
        stateMachine = gameObject.GetComponent<HPlayerStateMachine>();
        stateMachine.SetInThirdPersonCamera(true);
        aimTargetReticle.gameObject.SetActive(false);
        layerMask = 1<<LayerMask.NameToLayer("Player");
        layerMask=~layerMask;
        GameObject muzzle = Instantiate(muzzleToSpawn, gunTrans.position, thirdPersonFollowPlace.rotation, gunTrans);
        muzzleVFX = muzzle.GetComponent<ParticleSystem>();
        muzzleVFX.Stop();

        animator = GetComponent<Animator>();
        animator.SetLayerWeight(1,0);
        
        ShowWeaponChangeMessage();
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
    int currentShootClass = 0;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //FaceToPlayerCameraDirection();
            thirdAimCamera.gameObject.SetActive(true);
            HandleOrdinaryShoot();
            SetOnWeap0n();
            // StartCoroutine(CalculateScreenRayForShoot());
            animator.SetLayerWeight(1,1);
        }
        else if (Input.GetMouseButton(0))
        {
            
            //PrepareForShoot();
        } 
        else if(Input.GetMouseButtonUp(0))
        {
            thirdAimCamera.gameObject.SetActive(false);
            aimTargetReticle.gameObject.SetActive(false);
            SetOffShoot();
            //FaceToPlayerCameraDirection();
            StopAllCoroutines();
            animator.SetLayerWeight(1,0);
        }
        RotateCharacterWithMouse2();
        HandleContinueShoot();

        if (Input.GetMouseButtonDown(1))
        {
            thirdAimCamera.m_Lens.FieldOfView = 20f;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            thirdAimCamera.m_Lens.FieldOfView = 60f;
        }
        
        //按下中键切换射击模式
        if (Input.GetMouseButtonDown(2))
        {
            currentShootClass = (currentShootClass + 1) % MyShootEnum.GetNames(typeof(MyShootEnum)).Length;
            ShowWeaponChangeMessage();
        }
        
    }

    bool shootContinueOn = false;
    void ShowWeaponChangeMessage()
    {
        //ui更改
        if (currentShootClass == MyShootEnum.ShootFromMuzzle.GetHashCode())
        {
            HMessageShowMgr.Instance.ShowMessage("ROGUE_CURRENT_WEAPON_GUN1");
        }
        else if (currentShootClass == MyShootEnum.ShootLaserFromMuzzle.GetHashCode())
        {
            HMessageShowMgr.Instance.ShowMessage("ROGUE_CURRENT_WEAPON_GUN2");
        }
    }
    void SetOnWeap0n()
    {
        
        if (currentShootClass == MyShootEnum.ShootLaserFromMuzzle.GetHashCode())
        {
            if (LaserEff == null)
            {
                LaserEff = Instantiate(effectToSpawn[MyShootEnum.ShootLaserFromMuzzle.GetHashCode()], gunTrans.position, thirdPersonFollowPlace.rotation);
                //Debug.Log("LaserEff出生: " + LaserEff);
            }
            shootContinueOn = true;
        }
    }
    void SetOffShoot()
    {
        shootContinueOn = false;
        if(LaserEff!=null)
            Destroy(LaserEff);
    }
    void HandleOrdinaryShoot()
    {
        //如果当前是ShootFromMuzzle，就调用ShootBulletFromMuzzle
        //如果当前是ShootLaserFromMuzzle，就调用YShootLaserFromMuzzle
        if(currentShootClass == MyShootEnum.ShootFromMuzzle.GetHashCode())
        {
            StartCoroutine(CalculateScreenRayForShoot());
        }
    }
    void HandleContinueShoot()
    {
        if (shootContinueOn == false)
        {
            return;
        }
        if(currentShootClass == MyShootEnum.ShootLaserFromMuzzle.GetHashCode())
        {
            YCalculateScreenRayForLaser();
        }
        
    }
    
    float TurnSpeed = 3;
    float VerticalRotMin = -60;
    float VerticalRotMax = 60;
    
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
        muzzleVFX.Play();
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
                Effects = Instantiate(effectToSpawn[MyShootEnum.ShootFromMuzzle.GetHashCode()], gunTrans.position, Quaternion.LookRotation(dir), gunTrans.transform);
                Destroy(Effects, 10f);
            }
            else {
                //没有打到任何东西，进入这个逻辑
                Effects = Instantiate(effectToSpawn[MyShootEnum.ShootFromMuzzle.GetHashCode()], gunTrans.position, thirdPersonFollowPlace.rotation,gunTrans.transform);
                Destroy(Effects, 10f);
            }
            
        }
    }
    
    IEnumerator CalculateScreenRayForShoot()
    {
        aimTargetReticle.gameObject.SetActive(true);
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
                    if(Physics.Raycast(ray, out hit, 100, layerMask))
                    {
                        //Debug.Log(hit.collider.gameObject.name);
                        if (hit.collider.gameObject.CompareTag("Enemy"))
                        {
                            hitPosition = hit.point;
                            //draw ray
                            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                            Debug.Log(hit.collider.gameObject.name);
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
            if (Physics.Raycast(middleRay, out hit2, 100, layerMask))
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
    
    void YCalculateScreenRayForLaser()
    {
        aimTargetReticle.gameObject.SetActive(true);
        if (LaserEff == null)
        {
            LaserEff = Instantiate(effectToSpawn[MyShootEnum.ShootLaserFromMuzzle.GetHashCode()], gunTrans.position, thirdPersonFollowPlace.rotation);
            //Debug.Log("LaserEff出生: " + LaserEff);
        }
        
        GameObject historyObject = gameObject;
        //中心位置射出1条射线，检测射中的物体
        Vector3 hitPosition = new Vector3();
        float middleX = Screen.width * 0.5f;
        float middleY = Screen.height * 0.5f;
        bool hitButNoNeedHelp = false;
            
        Ray middleRay = mainPlayerCamera.ScreenPointToRay(new Vector3(middleX, middleY, 0));
        RaycastHit hit2;
        if (Physics.Raycast(middleRay, out hit2, 100, layerMask))
        {
            hitButNoNeedHelp = true;
            // Debug.Log("hitButNoNeedHelp: " + hit2.collider.gameObject.name);
            hitPosition = hit2.point;
        }
        // Debug.Log("hitButNoNeedHelp: " + hitButNoNeedHelp);
        YShootLaserFromMuzzle(hitButNoNeedHelp,hitPosition);
        
    }
    private GameObject LaserEff;
    private void YShootLaserFromMuzzle(bool hitButNoNeedHelp, Vector3 hitPosition)
    {
        //由枪口位置向屏幕中心所指的位置发射子弹
        // Debug.Log("enterLLLLLLLLLLLLLL");
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
            if (hitButNoNeedHelp)
            {
                //打到怪，近似处理，辅助瞄准
                Vector3 dir = hitPosition - gunTrans.position;
                //instantiate effectToSpawn at gunTrans.position, with rotation to shotDir
                LaserEff.transform.position = gunTrans.position;
                LaserEff.transform.rotation = Quaternion.LookRotation(dir);
            }
            else 
            {
                LaserEff.transform.position = gunTrans.position;
                LaserEff.transform.rotation = thirdPersonFollowPlace.rotation;
                Debug.Log("LaserEffrotate: " + LaserEff.transform.rotation);
            }
            
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
