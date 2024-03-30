using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
//using UnityEditor.Build;
using UnityEngine;
using UnityEngine.LowLevel;

/// <summary>
/// 用来控制 游玩模式的控制器
/// </summary>
public class YPlayModeController : MonoBehaviour
{
    //单例
    private static YPlayModeController instance;
    public static YPlayModeController Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
        CanVasUI = GameObject.Find("CanvasShowUI");
        CanvasShowUINew = GameObject.Find("CanvasShowUINew");
        
        YTriggerEvents.OnShortcutKeyInteractionStateChanged += SetNameUILabel;
    }
    HCameraLayoutManager CameraLayoutManager;
    GameObject curCharacter;
    private GameObject FreeLookCamera;
    GameObject PlayerCamera; 
    
    GameObject PuppetCamera;
    
    public bool detectModeIsOn = false;
    
    HPlayerStateMachine playerStateMachine;
    public void SetCharacter(int characterIndex)
    {
        PlayerDieFlag = false;
        //设置角色
        int id = yPlanningTable.Instance.selectNames2Id["character"];
        string path = "Prefabs/YCharacter/"+yPlanningTable.Instance.SelectTable[id][characterIndex]+"Player";

        int curLevelID = YLevelManager.GetCurrentLevelIndex();
        Transform GeneratePlace = yPlanningTable.Instance.GetCharacterGeneratePlace(curLevelID);

        GameObject player =
            Instantiate(Resources.Load<GameObject>(path), GeneratePlace.position, GeneratePlace.rotation);
        curCharacter = player;
        // go.transform.parent = YChooseCharacterShowPlace.transform;
        // go.transform.localPosition = Vector3.zero;
        // curCharacter = go;
        //PlayerCameraPlayMode
        string pathPlayerCamera = "Prefabs/YPlayModePrefab/PlayerCameraPlayMode";
        PlayerCamera = Instantiate(Resources.Load<GameObject>(pathPlayerCamera));
        //player.GetComponent<HPlayerStateMachine>().playerCamera = PlayerCamera.GetComponent<Camera>();
        playerStateMachine = player.GetComponent<HPlayerStateMachine>();
        playerStateMachine.playerCamera = PlayerCamera.GetComponent<Camera>();
        
        //FreeLookCameraPlayMode
        string pathFreeLookCamera = "Prefabs/YPlayModePrefab/FreeLookCameraPlayMode";
        FreeLookCamera = Instantiate(Resources.Load<GameObject>(pathFreeLookCamera));
        //设置目标与跟随
        FreeLookCamera.GetComponent<CinemachineFreeLook>().Follow = player.transform;
        FreeLookCamera.GetComponent<CinemachineFreeLook>().LookAt = player.transform;
        
        string pathCameraLayout = "Prefabs/YPlayModePrefab/CameraLayoutPlayMode";
        GameObject CameraLayout = Instantiate(Resources.Load<GameObject>(pathCameraLayout));
        CameraLayoutManager = CameraLayout.GetComponent<HCameraLayoutManager>();
        CameraLayoutManager.playerCamera = PlayerCamera.GetComponent<Camera>();
        // CameraLayoutManager.puppetCamera = GameObject.Find("PuppetCamera").GetComponent<Camera>();
        if (PuppetCamera == null)
        {
            PuppetCamera = GameObject.Find("PuppetCamera");
        }
        CameraLayoutManager.puppetCamera = PuppetCamera.GetComponent<Camera>();
        CameraLayoutManager.SetPlayerCameraWholeScreen();
        // CameraLayoutManager.SetPuppetCameraLittle();
    }

    public void LockPlayerInput(bool shouldLock) 
    {
        //YTriggerEvents.RaiseOnMouseLockStateChanged(!shouldLock);//视角lock 鼠标应该出现
        
        //curCharacter.GetComponent<HPlayerStateMachine>().SetInputActionDisableOrEnable(shouldLock);
        playerStateMachine.SetInputActionDisableOrEnable(shouldLock);
        FreeLookCamera.GetComponent<CinemachineInputProvider>().enabled = !shouldLock;
    }
    
    public void SetCameraLayout(int layoutIndex)
    {
        if (CameraLayoutManager == null)
        {
            return;
        }
        switch (layoutIndex)
        {
            case 0:
                CameraLayoutManager.SetPlayerCameraWholeScreen();
                break;
            case 1:
                CameraLayoutManager.SetPuppetCameraLittle();
                break;
            case 2:
                CameraLayoutManager.SetTwoCamerasEachHalf();
                break;
        }
    }
    
    public List<YNameUI> PlaceUIToShowScriptsList = new List<YNameUI>();
    public GameObject CanVasUI;
    public void PlaceKeepNameUI()
    {
        
        //元素视野打开 将所有的地点显示出来并改名字
        List<GameObject> allPoints = yPlanningTable.Instance.DestinationGoList;
        List<string> PlaceUIToShowList = yPlanningTable.Instance.DestinationUINameList;
        int j = 0;
        foreach (var destination in allPoints)
        {
            GameObject ShowPlaceUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/YNameUI/ShowPlaceUI"));
            ShowPlaceUI.transform.parent = CanVasUI.transform;
            //PlaceUIToShowList.Add(ShowPlaceUI);
            YNameUI yNameUiSctipt = ShowPlaceUI.GetComponent<YNameUI>();
            PlaceUIToShowScriptsList.Add(yNameUiSctipt);
            yNameUiSctipt.SetAttribute(PlaceUIToShowList[j],destination.transform,PlayerCamera);
            j++;
        }
    }
    bool flagEnterDetectViewOnOrOff = false;
    public void DetectViewOnOrOff()
    {
        if (flagEnterDetectViewOnOrOff == false)
        {
            PlaceKeepNameUI();
            flagEnterDetectViewOnOrOff = true;
        }
        
        detectModeIsOn = !detectModeIsOn;
        CanVasUI.SetActive(detectModeIsOn);

        if (detectModeIsOn)
        {
            HPlayerSkillManager.instance.SkillScanningTerrian();
        }
        
    }
    
    public void DetectViewOn()
    {
        //只会进一次
        if (flagEnterDetectViewOnOrOff == false)
        {
            PlaceKeepNameUI();
            flagEnterDetectViewOnOrOff = true;
        }
        //HPlayerSkillManager中会进行计时 调用这个函数 按理说不应这样，，
        CanVasUI.SetActive(true);
        //HPlayerSkillManager.instance.SkillScanningTerrian();
        
    }
    
    public void DetectViewOff()
    {
        detectModeIsOn = false;
        CanVasUI.SetActive(false);
        
    }

    private GameObject CanvasShowUINew;
    private GameObject ShowPlaceUI = null;
    YNameUI yNameUiSctipt;
    public void SetNameUILabel(object sender, YTriggerGameObjectEventArgs e)
    {
        //GameObject destination = e.gameObject;
        Transform showUI = e.showUIPlace;
        if (e.activated)
        {
            //如果showpalceui==null
            if (ShowPlaceUI == null)//但是这样会导致同时只有一个牌子显示 也挺好 防止不知道点哪个
            {
                //出现提示面板
                //可以使用快捷键进行交互
                // ShowPlaceUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/YNameUI/ShowInteractUI"));
                // ShowPlaceUI.transform.parent = CanvasShowUINew.transform;
                
                //等待Instantiate之后，再ShowPlaceUI.transform.parent = CanvasShowUINew.transform;
                ShowPlaceUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/YNameUI/ShowInteractUI"),CanvasShowUINew.transform);
                
                //PlaceUIToShowList.Add(ShowPlaceUI);
                //yNameUiSctipt = ShowPlaceUI.GetComponent<YNameUI>();
                yNameUiSctipt = ShowPlaceUI.GetComponentInChildren<YNameUI>();
            
                
            }

            if (yNameUiSctipt != null)
            {
                yNameUiSctipt.SetAttribute("Q",showUI,PlayerCamera);
                ShowPlaceUI.SetActive(true);
            }
            
        }
        else
        {
            if (ShowPlaceUI != null)
            {
                ShowPlaceUI.SetActive(false);
            }
           
        }
       
    }

    public void EnterNewLevel()
    {
        if (curCharacter)
        {
            Destroy(curCharacter);
        }
        if (FreeLookCamera)
        {
            Destroy(FreeLookCamera);
        }
        if (PlayerCamera)
        {
            Destroy(PlayerCamera);
        }
        if (CameraLayoutManager)
        {
            Destroy(CameraLayoutManager.gameObject);
        }
        
    }

    public void GiveUpPanel()
    {
        yPlanningTable.Instance.ClearMoveOrNoMoveAnimationList();
        SetCameraLayout(0);//全 小 半
        //视角如果锁住了要回退！
        LockPlayerInput(false);
        //鼠标进入屏幕
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
    }

    bool PlayerDieFlag = false;
    public void PlayerDie()
    {
        //应该先转换为死亡状态 播放死亡动画 然后一会儿后弹出失败界面
        if (PlayerDieFlag)
        {
            return;
        }
        PlayerDieFlag = true;
        //curCharacter.GetComponent<HPlayerStateMachine>().SetPlayerDie();
        StartCoroutine(PlayerDieCoroutine());
        playerStateMachine.IsDie = true;
    }
    IEnumerator  PlayerDieCoroutine()
    {
        yield return new WaitForSeconds(3f);
        YGameRoot.Instance.Pop();
        YGameRoot.Instance.Push(new YLossAndNextLevelPanel());
        
    }
   
}
