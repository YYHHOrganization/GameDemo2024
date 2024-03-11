using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class YChooseScreenplayPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YChooseScreenplayPanelNew";

    //改为字典
    Dictionary<string,int> selectId = yPlanningTable.Instance.selectId;
    
    //改为可扩展的List
    List<string> dropdownNames = yPlanningTable.Instance.dropdownNames;
    List<string> selectNames = yPlanningTable.Instance.selectNames;
    
    List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown>();
    //TMP_Dropdown初始话为dropdownNames.Count个 不使用确定的数字 比如8

    //List<List<string>> dropdownOptions = yPlanningTable.Instance.SelectTable;
    List<List<string>> dropdownOptions = yPlanningTable.Instance.UISelectTable;
    
    private GameObject panelToAddUnitParent;
    
    //int[] selectId=new int[5];
    
    List<GameObject> Units = new List<GameObject>();
    public int curChooseUnitIndex=1;
    
   
    
    
    public YChooseScreenplayPanel() : base(new UIType(path)){}
    public override void OnEnter()
    {
        panelToAddUnitParent = uiTool.GetOrAddComponentInChilden<Transform>("PanelToAddUnit").gameObject;
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
        uiTool.GetOrAddComponentInChilden<Button>("AddMoveButton").onClick.AddListener(()=>
        {
            if (!CheckIndex(curChooseUnitIndex))
            {
                return;
            }
            //新增移动单元 新增到panelToAddUnitParent下
            GameObject go = Resources.Load<GameObject>("Prefabs/UI/singleUnit/PanelUnitMove");
            GameObject unit = GameObject.Instantiate(go,panelToAddUnitParent.transform);
            AddDropdownInUnitMove(unit);
            Units.Add(unit);
            curChooseUnitIndex++;
        });
        uiTool.GetOrAddComponentInChilden<Button>("AddNoMoveButton").onClick.AddListener(()=>
        {
            if (!CheckIndex(curChooseUnitIndex))
            {
                return;
            }
            CheckIndex(curChooseUnitIndex);
            //新增固定单元
            GameObject go = Resources.Load<GameObject>("Prefabs/UI/singleUnit/PanelUnitNoMove");
            GameObject unit = GameObject.Instantiate(go,panelToAddUnitParent.transform);
            AddDropdownInUnitNoMove(unit);
            Units.Add(unit);
            curChooseUnitIndex++;
        });
        
        TMP_Dropdown dropdown = uiTool.GetOrAddComponentInChilden<TMP_Dropdown>("DropdownCharacter");
        int id = yPlanningTable.Instance.selectNames2Id["character"];
        dropdown.AddOptions(dropdownOptions[id]);
        dropdown.onValueChanged.AddListener((int value)=>
        {
            OnDropdownValueChange(id,value);
        });
    }
    bool CheckIndex(int index)
    {
        if (index>5)//1-5
        {
            Debug.LogError("超过了最大的index");
            
            return false;
        }
        
        return true;
    }
    void  AddDropdownInUnitNoMove(GameObject unit)
    {
        AddDropdownInUnit(unit,"animation","Animation",curChooseUnitIndex,false);
        AddDropdownInUnit(unit,"camera","Camera",curChooseUnitIndex);
        AddDropdownInUnit(unit,"blendshape","Blendshape",curChooseUnitIndex);
        AddDropdownInUnit(unit,"effect","Effect",curChooseUnitIndex);
        //AddDropdownInUnit(unit,"destination","Destination",curChooseUnitIndex);
    }
    void  AddDropdownInUnitMove(GameObject unit)
    {
        AddDropdownInUnit(unit,"animation","Animation",curChooseUnitIndex,true);
        AddDropdownInUnit(unit,"camera","Camera",curChooseUnitIndex);
        AddDropdownInUnit(unit,"blendshape","Blendshape",curChooseUnitIndex);
        AddDropdownInUnit(unit,"effect","Effect",curChooseUnitIndex);
        AddDropdownInUnit(unit,"destination","Destination",curChooseUnitIndex);
    }
    void AddDropdownInUnit(GameObject unit,string name,string Name,int indexInUnit)
    {
        TMP_Dropdown dropdown = uiTool.GetOrAddComponentInChilden<TMP_Dropdown>("Dropdown"+Name,unit.transform);
        int id = yPlanningTable.Instance.selectNames2Id[name+indexInUnit];
        dropdown.AddOptions(dropdownOptions[id]);
        dropdown.onValueChanged.AddListener((int value)=>
        {
            OnDropdownValueChange(id,value);
        });
    }
    void AddDropdownInUnit(GameObject unit,string name,string Name,int indexInUnit,bool isAnimMove)
    {
        TMP_Dropdown dropdown = uiTool.GetOrAddComponentInChilden<TMP_Dropdown>("Dropdown"+Name,unit.transform);
        int id = yPlanningTable.Instance.selectNames2Id[name+indexInUnit];
        
        yPlanningTable.Instance.UpdateMoveOrNoMoveAnimationList(id,indexInUnit,isAnimMove);
        
        dropdown.AddOptions(dropdownOptions[id]);
        dropdown.onValueChanged.AddListener((int value)=>
        {
            OnDropdownValueChange(id,value);
        });
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