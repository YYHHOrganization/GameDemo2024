using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class YChooseCharacterPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YChooseCharacterPanelNew";
    static readonly string pathPlace = "Prefabs/YPlace/YChooseCharacterShowPlace";
    public YChooseCharacterPanel() : base(new UIType(path)){}
    public GameObject YChooseCharacterShowPlace;
    public GameObject curCharacter;
    private GameObject curCatcake;
    public int curChooseCharacterIndex=-1;
    private int curChooseCatcakeIndex = -1;
    private string curChooseCatcakeIndexStr = "";

    private bool isInteractMode = true;

    private TMP_Text chatAnswerText;
    private TMP_InputField inputField;
    private Button sendMessageButton;
    public TMP_Text ChatAnswerText
    {
        get
        {
            if (chatAnswerText == null)
            {
                chatAnswerText = uiTool.GetOrAddComponentInChilden<TMP_Text>("AnswerText");
            }
            return chatAnswerText;
        }
        set => chatAnswerText = value;
    }
    GameObject CharacterScrollView;
    GameObject CatcakeScrollView;

    private void ShowCharactersWithMihoyoOptions()
    {
        if (yPlanningTable.Instance.isMihoyo)
        {
            Transform content = CharacterScrollView.transform.Find("Viewport/Content");
            //把CharacterScrollView下面所有的子物体都SetActiveTrue
            for (int i = 0; i < content.transform.childCount; i++)
            {
                if(content.transform.GetChild(i).gameObject.activeSelf==false && content.transform.GetChild(i).gameObject.name.Contains("CharacterButton"))
                    content.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    
    public override void OnEnter()
    {
        HAudioManager.instance.Play("SelectCharacterMusic", HAudioManager.instance.gameObject);
        //YChooseCharacterShowPlace = GameObject.Find("YChooseCharacterShowPlace");
        YChooseCharacterShowPlace = GameObject.Instantiate(Resources.Load<GameObject>(pathPlace));
        
        CharacterScrollView = uiTool.GetOrAddComponentInChilden<Transform>("CharacterScrollView").gameObject;
        CatcakeScrollView = uiTool.GetOrAddComponentInChilden<Transform>("CatcakeScrollView").gameObject;

        ShowCharactersWithMihoyoOptions();
        
        uiTool.GetOrAddComponentInChilden<Button>("SelectCharacterButton").onClick.AddListener(() =>
        {
            CharacterScrollView.SetActive(true);
            CatcakeScrollView.SetActive(false);
        });
        uiTool.GetOrAddComponentInChilden<Button>("SelectCatcakeButton").onClick.AddListener(() =>
        {
            CharacterScrollView.SetActive(false);
            CatcakeScrollView.SetActive(true);
        });
        
        uiTool.GetOrAddComponentInChilden<Button>("BeginButton").onClick.AddListener(()=>
        {
            Pop();
            //Push(new YChooseScreenplayPanel());//其实并不是 而是应该直接让角色在场景中活动，但是先这样吧
            if (YLevelManager.GetCurrentLevelIndex() != 2&&YLevelManager.GetCurrentLevelIndex() != 3)
            {
                Push(new YMainPlayModeOriginPanel());
                HAudioManager.instance.Play("StartPlayerModeMusic", HAudioManager.instance.gameObject);
            }
            else
            {
                HRoguePlayerAttributeAndItemManager.Instance.PushAttributePanel();
                //出现正在前往第几层的提示
                Push(new YRogueWinAndNextLevelPanel());
                HAudioManager.Instance.Play("StartRogueAudio", HAudioManager.instance.gameObject);
            }
                
            //销毁YChooseCharacterShowPlace
            GameObject.Destroy(YChooseCharacterShowPlace);
            //然后应该把那些加载出来 
            YPlayModeController.Instance.StartGameAndSet(curChooseCharacterIndex,curChooseCatcakeIndexStr);
            // YPlayModeController.Instance.SetCharacter(curChooseCharacterIndex);
            
        });
        
        //todo:后面有需求写一下换猫猫糕和加载拥有的猫猫糕的逻辑
        // uiTool.GetOrAddComponentInChilden<Button>("ChooseCatcakeButton").onClick.AddListener(()=>
        // {
        //     //Push(new SelectCatcakeForRogueGamePanel());
        //     
        // });
        
        
        Debug.Log("角色数量"+yPlanningTable.Instance.GetCharacterNum());
        //循环遍历角色列表
        #if BUILD_MODE
        // uiTool.GetOrAddComponentInChilden<Button>("CharacterButton0").gameObject.SetActive(false);
        // uiTool.GetOrAddComponentInChilden<Button>("CharacterButton1").onClick.AddListener(() =>
        // {
        //     //第二个变成了随机选择角色
        //     int randomIndex = Random.Range(2, yPlanningTable.Instance.GetCharacterNum());
        //     SetCharacter(randomIndex);
        // });
        for (int i = 2; i < yPlanningTable.Instance.GetCharacterNum(); i++)
        {
            //不可以直接传i 因为i会变，因为是引用类型
            int index = i;
            //给每个角色按钮添加点击事件
            //如果这个Button是隐藏的，那么就不添加点击事件:
            Button button = uiTool.GetOrAddComponentInChilden<Button>("CharacterButton" + index);
            if (button!=null)
            {
                button.onClick.AddListener(() =>
                {
                    SetCharacter(index);
                });
            }
            
            // uiTool.GetOrAddComponentInChilden<Button>("CharacterButton"+index).onClick.AddListener(()=>
            // {
            //     SetCharacter(index);
            // });
        }
        #else
        for (int i = 0; i < yPlanningTable.Instance.GetCharacterNum(); i++)
        {
            //不可以直接传i 因为i会变，因为是引用类型
            int index = i;
            //给每个角色按钮添加点击事件
            uiTool.GetOrAddComponentInChilden<Button>("CharacterButton"+index).onClick.AddListener(()=>
            {
                SetCharacter(index);
            });
        }
        #endif
        //遍历SD_RoguePetCSVFile.Class_Dic，将其id取出
        foreach (var item in SD_RoguePetCSVFile.Class_Dic)
        {
            string id = item.Key;
            uiTool.GetOrAddComponentInChilden<Button>("CatcakeButton_"+id).onClick.AddListener(()=>
            {
                SetCatcake(id);
            });
            
        }
        // for (int i = 0; i < SD_RoguePetCSVFile.Class_Dic.Count; i++)
        // {
        //     int index = i;
        //     uiTool.GetOrAddComponentInChilden<Button>("CatcakeButton"+index).onClick.AddListener(()=>
        //     {
        //         SetCatcake(index);
        //     });
        // }
        
        SetCharacter(10);
        // SetCatcake(0);
        SetCatcake("XingCatcake");
        
        inputField = uiTool.GetOrAddComponentInChilden<TMP_InputField>("InputField");
        inputField.onEndEdit.AddListener((string value) =>
        {
            if (curChooseCharacterIndex >= 0)
            {
                HChatGPTManager.Instance.SetPeopleIndex(curChooseCharacterIndex);
                HChatGPTManager.Instance.AskChatGPT(this, value);
            }
        });
        
        chatAnswerText = uiTool.GetOrAddComponentInChilden<TMP_Text>("AnswerText");
        
        sendMessageButton = uiTool.GetOrAddComponentInChilden<Button>("SendMessageButton");
        sendMessageButton.onClick.AddListener(()=>
        {
            if (curChooseCharacterIndex >= 0 && inputField.text != "")
            {
                HChatGPTManager.Instance.SetPeopleIndex(curChooseCharacterIndex);
                HChatGPTManager.Instance.AskChatGPT(this, inputField.text);
            }
        });

        isInteractMode = true;
        inputField.gameObject.SetActive(!isInteractMode);
        chatAnswerText.gameObject.SetActive(!isInteractMode);
        sendMessageButton.gameObject.SetActive(!isInteractMode);
        chatAnswerText.gameObject.GetComponentInParent<Image>().enabled=!isInteractMode;
        
        var interactButton = uiTool.GetOrAddComponentInChilden<Button>("SelectPatternButton");
        interactButton.onClick.AddListener(() =>
        {
            isInteractMode = !isInteractMode;
            inputField.gameObject.SetActive(!isInteractMode);
            chatAnswerText.gameObject.SetActive(!isInteractMode);
            sendMessageButton.gameObject.SetActive(!isInteractMode);
            chatAnswerText.gameObject.GetComponentInParent<Image>().enabled=!isInteractMode;
            if (isInteractMode)
            {
                //change text
                interactButton.GetComponentInChildren<TMP_Text>().text = "交互模式";
            }
            else
            {
                interactButton.GetComponentInChildren<TMP_Text>().text = "鉴赏模式";
                #if BUILD_MODE  //发行版本暂不包含ChatGPT聊天功能
                    inputField.gameObject.SetActive(false);
                    chatAnswerText.gameObject.SetActive(false);
                    sendMessageButton.gameObject.SetActive(false);
                    chatAnswerText.gameObject.GetComponentInParent<Image>().enabled=false;
                    HMessageShowMgr.Instance.ShowMessage("BUILD_NOT_SHOW_GPT_MSG");
                #endif
            }

            if (curCharacter)
            {
                curCharacter.GetComponent<HCharacterInteracionInShow>().SetInteractMode(isInteractMode);
            }
        });
        
        CatcakeScrollView.SetActive(false);
        CharacterScrollView.SetActive(true);
        
        //开启雾效
        SetFogOnOrFalse(true);
    }
    void SetFogOnOrFalse(bool isOn)
    {
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogHeight",isOn, 2.31f);
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogDistance",isOn, 0.05f);
    }

    private void SetCatcake(string index)
    {
        if (index == curChooseCatcakeIndexStr)
        {
            return;
        }

        if (curCatcake != null)
        {
            GameObject.Destroy(curCatcake);
        }
        curChooseCatcakeIndexStr = index;
        string catcakePrefabLink = "Show_"+SD_RoguePetCSVFile.Class_Dic[index].addressableLink;
        
        //用Addressable来加载
        GameObject go = Addressables.InstantiateAsync(catcakePrefabLink).WaitForCompletion();
        go.transform.parent = YChooseCharacterShowPlace.transform;
        go.transform.localPosition = Vector3.zero + new Vector3(-0.95f, 0, 0);
        //旋转180度
        go.transform.localEulerAngles = new Vector3(0, 50, 0);
        curCatcake = go;
        go.gameObject.GetComponentInChildren<Animator>().SetBool("isCatStrike", true);
        
        //如果这个角色没解锁，就上锁shader，现在都默认上锁吧
        bool isUnLock = YCharacterInfoManager.GetPlayerContentUnLockStatusByID(index);
        YEffManager._Instance.SetPetLockedShaderOnOrOff(!isUnLock, go.transform);
        //如果此时选择的角色和猫猫糕都是锁定的，这个"BeginButton"就应该是不可点击的是灰色的
        UpdateLockState(false,!isUnLock);
        
    }
    
    //beginButton的交互状态
    // 用一个二级制数字表示 是否角色和宠物都是锁定的
    //如果角色是锁定的，宠物不锁，就是10，否则是01.都不锁就是00，都锁就是11
    //后面可以扩展成更多的东西，这样就只需要存一个变量：
    
    // 定义锁定状态
    const int CHARACTER_LOCKED = 0b10; // 2
    const int PET_LOCKED = 0b01;       // 1
    int currentState = 0b00;
    private void SetBeginButtonInteractableState(int state)
    {
        bool isCharacterLocked = (state &  CHARACTER_LOCKED) != 0;
        bool isPetLocked = (state & PET_LOCKED) != 0;

        //有一个锁定就不能,也就是说 要都解锁才能
        uiTool.GetOrAddComponentInChilden<Button>("BeginButton").interactable  = (!isCharacterLocked && !isPetLocked);
    }
    private void UpdateLockState(bool isCharacter, bool isLocked)
    {
        
        if (isCharacter)
        {
            //如果 isLocked 为 true，使用按位或操作符 | 将 currentState 与 CHARACTER_LOCKED 进行按位或操作，将角色锁定状态置为 1。
            currentState = isLocked ? (currentState | CHARACTER_LOCKED) : (currentState & ~CHARACTER_LOCKED);
        }
        else
        {
            currentState = isLocked ? (currentState | PET_LOCKED) : (currentState & ~PET_LOCKED);
        }

        SetBeginButtonInteractableState(currentState);
    }
    //end beginButton的交互状态
    
    private void SetCatcake(int index)
    {
        if (index == curChooseCatcakeIndex)
        {
            return;
        }

        if (curCatcake != null)
        {
            GameObject.Destroy(curCatcake);
        }
        curChooseCatcakeIndex = index;
        string catcakePrefabLink = "RogueGameCatcakeModel" + index;
        //用Addressable来加载
        GameObject go = Addressables.InstantiateAsync(catcakePrefabLink).WaitForCompletion();
        go.transform.parent = YChooseCharacterShowPlace.transform;
        go.transform.localPosition = Vector3.zero + new Vector3(-0.95f, 0, 0);
        //旋转180度
        go.transform.localEulerAngles = new Vector3(0, 50, 0);
        curCatcake = go;
        go.gameObject.GetComponentInChildren<Animator>().SetBool("isCatStrike", true);
    }
    
    public void SetCharacter(int index)
    {
        if(index==curChooseCharacterIndex)
        {
            return;
        }
        if (curCharacter!=null)
        {
            GameObject.Destroy(curCharacter);
        }
        curChooseCharacterIndex = index;
        
        Debug.Log("选择了角色"+index);
        int id = yPlanningTable.Instance.selectNames2Id["character"];
        string path = "Prefabs/YCharacter/"+yPlanningTable.Instance.SelectTable[id][index]+"Show";
        //string path = "Prefabs/YCharacter/2";
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(path));
        go.transform.parent = YChooseCharacterShowPlace.transform;
        go.transform.localPosition = Vector3.zero;
        curCharacter = go;

        go.GetComponent<HCharacterInteracionInShow>().SetInteractMode(isInteractMode);
        
        //设置角色
        //yPlanningTable.Instance.SetCharacter(i);
        
        //如果这个角色没解锁，就上锁shader，现在都默认上锁吧
        bool isUnLock = YCharacterInfoManager.GetCharacterUnLockStatusByID(index.ToString());
        YEffManager._Instance.SetLockedShaderOnOrOff(!isUnLock, go.transform);
        UpdateLockState(true,!isUnLock);
        //如果此时选择的角色是锁定的，这个"BeginButton"就应该是不可点击的是灰色的
        //uiTool.GetOrAddComponentInChilden<Button>("BeginButton").interactable = isUnLock;
    }
    //退出面板
    public override void OnExit()
    {
        base.OnExit();
        //关闭雾效
        SetFogOnOrFalse(false);
    }
}
