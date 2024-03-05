using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class YChooseScreenplayPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YChooseScreenplayPanel";

    //改为字典
    Dictionary<string,int> selectId = yPlanningTable.Instance.selectId;
    
    //改为可扩展的List
    List<string> dropdownNames = yPlanningTable.Instance.dropdownNames;
    List<string> selectNames = yPlanningTable.Instance.selectNames;
    
    List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown>();
    //TMP_Dropdown初始话为dropdownNames.Count个 不使用确定的数字 比如8

    //List<List<string>> dropdownOptions = yPlanningTable.Instance.SelectTable;
    List<List<string>> dropdownOptions = yPlanningTable.Instance.UISelectTable;
    
    
    //int[] selectId=new int[5];
    
    public YChooseScreenplayPanel() : base(new UIType(path)){}
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
        for (int i = 0; i < dropdownNames.Count; i++)
        {
            dropdowns.Add(uiTool.GetOrAddComponentInChilden<TMP_Dropdown>(dropdownNames[i]));
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