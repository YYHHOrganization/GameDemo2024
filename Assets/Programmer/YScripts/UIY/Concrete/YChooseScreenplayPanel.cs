using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class YChooseScreenplayPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YChooseScreenplayPanel";

    //改为字典
    Dictionary<string,int> selectId = new Dictionary<string, int>()
    {
        {"character",0},
        {"animation",0},
        {"audio",0},
        //加上特效和相机
        {"effect",0},
        {"camera",0},
        {"origin",0},
        {"destination",0},
        //宝藏年份
        {"treasure",0},
        
    };
    string[] dropdownNames = new string[8]{"DropdownCha","DropdownAnimation",
        "DropdownAudio","DropdownEffect","DropdownCamera","DropdownOrigin","DropdownDestination","DropdownTreasure"};
    //加上特效和相加
    string[] selectNames = new string[8]{"character","animation","audio","effect","camera","origin","destination","treasure"};
    public YChooseScreenplayPanel() : base(new UIType(path)){}
    
    TMP_Dropdown[] dropdowns = new TMP_Dropdown[8];
    

    List<List<string>> dropdownOptions = yPlanningTable.Instance.SelectTable;
    
    
    //int[] selectId=new int[5];
    
    
    public override void OnEnter()
    {
        uiTool.GetOrAddComponentInChilden<Button>("OkButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了开始按钮");
            //YGameRoot.Instance.SceneSystem.SetScene(new YMainScene());
            ConfirmScreenplay();
            Pop();
            Push(new YMainPanel());
            
        });
        uiTool.GetOrAddComponentInChilden<Button>("BackButton").onClick.AddListener(()=>
        {
            Pop();
            Push(new StartPanel());
        });
        for (int i = 0; i < dropdownNames.Length; i++)
        {
            dropdowns[i] = uiTool.GetOrAddComponentInChilden<TMP_Dropdown>(dropdownNames[i]);
            dropdowns[i].AddOptions(dropdownOptions[i]);
            int index = i;
            dropdowns[i].onValueChanged.AddListener((int value)=>
            {
                OnDropdownValueChange(index,value);
            });
        }
        
    }
    
    public void OnDropdownValueChange(int index,int value)
    {
        Debug.Log("Dropdown"+index+"选择了" + value);
        selectId[selectNames[index]] = value;
    }
    
    public void ConfirmScreenplay()
    {
        YScreenPlayController.Instance.ConfirmScreenplay(selectId);
    }
}