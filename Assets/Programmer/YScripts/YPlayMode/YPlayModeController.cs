using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Build;
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
    }
    HCameraLayoutManager CameraLayoutManager;
    GameObject curCharacter;
    private GameObject FreeLookCamera;
    GameObject PlayerCamera; 
    
    public bool detectModeIsOn = false;
    public void SetCharacter(int characterIndex)
    {
        //设置角色
        int id = yPlanningTable.Instance.selectNames2Id["character"];
        string path = "Prefabs/YCharacter/"+yPlanningTable.Instance.SelectTable[id][characterIndex]+"Player";
        GameObject player = Instantiate(Resources.Load<GameObject>(path));
        curCharacter = player;
        // go.transform.parent = YChooseCharacterShowPlace.transform;
        // go.transform.localPosition = Vector3.zero;
        // curCharacter = go;
        //PlayerCameraPlayMode
        string pathPlayerCamera = "Prefabs/YPlayModePrefab/PlayerCameraPlayMode";
        PlayerCamera = Instantiate(Resources.Load<GameObject>(pathPlayerCamera));
        player.GetComponent<HPlayerStateMachine>().playerCamera = PlayerCamera.GetComponent<Camera>();
        
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
        CameraLayoutManager.puppetCamera = GameObject.Find("PuppetCamera").GetComponent<Camera>();
        CameraLayoutManager.SetPlayerCameraWholeScreen();
        // CameraLayoutManager.SetPuppetCameraLittle();
    }

    public void LockPlayerInput(bool shouldLock) 
    {
        //YTriggerEvents.RaiseOnMouseLockStateChanged(!shouldLock);//视角lock 鼠标应该出现
        curCharacter.GetComponent<HPlayerStateMachine>().SetInputActionDisableOrEnable(shouldLock);
        FreeLookCamera.GetComponent<CinemachineInputProvider>().enabled = !shouldLock;
    }
    
    public void SetCameraLayout(int layoutIndex)
    {
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
        List<GameObject> allPoints = yPlanningTable.Instance.DestinationList;
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
    
}
