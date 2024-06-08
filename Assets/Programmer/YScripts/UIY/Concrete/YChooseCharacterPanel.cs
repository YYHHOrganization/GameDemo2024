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
    
    
    public override void OnEnter()
    {
        HAudioManager.instance.Play("SelectCharacterMusic", HAudioManager.instance.gameObject);
        //YChooseCharacterShowPlace = GameObject.Find("YChooseCharacterShowPlace");
        YChooseCharacterShowPlace = GameObject.Instantiate(Resources.Load<GameObject>(pathPlace));
        
        CharacterScrollView = uiTool.GetOrAddComponentInChilden<Transform>("CharacterScrollView").gameObject;
        CatcakeScrollView = uiTool.GetOrAddComponentInChilden<Transform>("CatcakeScrollView").gameObject;
        
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
            YPlayModeController.Instance.SetCharacter(curChooseCharacterIndex);
            
        });
        
        //todo:后面有需求写一下换猫猫糕和加载拥有的猫猫糕的逻辑
        // uiTool.GetOrAddComponentInChilden<Button>("ChooseCatcakeButton").onClick.AddListener(()=>
        // {
        //     //Push(new SelectCatcakeForRogueGamePanel());
        //     
        // });
        
        
        Debug.Log("角色数量"+yPlanningTable.Instance.GetCharacterNum());
        //循环遍历角色列表
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
        
        SetCharacter(2);
        SetCatcake(0);
        
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
    }
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
        //如果此时选择的角色是锁定的，这个"BeginButton"就应该是不可点击的是灰色的
        uiTool.GetOrAddComponentInChilden<Button>("BeginButton").interactable = isUnLock;
    }
    //退出面板
    public override void OnExit()
    {
        base.OnExit();
        //关闭雾效
        SetFogOnOrFalse(false);
    }
}
