using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YChooseScreenplayInPlayModePanel  : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YChooseScreenplayInPlayModePanel";
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
    
    private bool shouldLockPlayerInput=false;
    
    public void ConfirmPlayFunc()
    {
        shouldLockPlayerInput = false;
        YPlayModeController.Instance.LockPlayerInput(shouldLockPlayerInput);
        ConfirmScreenplay();
        Pop();
        Push(new YMainPlayModePanel());
        HAudioManager.Instance.Play("PlayWithPuppetMusic", HAudioManager.Instance.gameObject);
    }
    
    public YChooseScreenplayInPlayModePanel() : base(new UIType(path)){}
    public override void OnEnter()
    {
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YTriggerEvents.RaiseOnShortcutKeyLockViewStateChanged(true);//触发 锁住视角 的快捷键的监听了
        // panelToAddUnitParent = uiTool.GetOrAddComponentInChilden<Transform>("PanelToAddUnit").gameObject;
        //YContentInScrollView
        panelToAddUnitParent = uiTool.GetOrAddComponentInChilden<Transform>("YContentInScrollView").gameObject;
        uiTool.GetOrAddComponentInChilden<Button>("OkButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了开始按钮");
            //YGameRoot.Instance.SceneSystem.SetScene(new YMainScene());
            HMessageShowMgr.Instance.ShowMessageWithActions("ConfirmPuppetAction",ConfirmPlayFunc,null,null);
            // shouldLockPlayerInput = false;
            // YPlayModeController.Instance.LockPlayerInput(shouldLockPlayerInput);
            // ConfirmScreenplay();
            // Pop();
            // Push(new YMainPlayModePanel());
            // HAudioManager.Instance.Play("PlayWithPuppetMusic", HAudioManager.Instance.gameObject);
            //
            // //设置为半屏
            // YPlayModeController.Instance.SetCameraLayout(2);
        });
        
        // uiTool.GetOrAddComponentInChilden<Button>("BackButton").onClick.AddListener(()=>
        // {
        //     Pop();
        //     Push(new StartPanel());
        // });
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
        // LockButton
        uiTool.GetOrAddComponentInChilden<Button>("LockButton").onClick.AddListener(()=>
        {
            
            shouldLockPlayerInput = !shouldLockPlayerInput;
            YPlayModeController.Instance.LockPlayerInput(shouldLockPlayerInput);
        });
        uiTool.GetOrAddComponentInChilden<Button>("ExitButton").onClick.AddListener(()=>
        {
            //显示出是否退出面板
            Push(new YExitPanel());
           
        });
        
        TMP_Dropdown dropdown = uiTool.GetOrAddComponentInChilden<TMP_Dropdown>("DropdownCharacter");
        int id = yPlanningTable.Instance.selectNames2Id["character"];
        dropdown.AddOptions(dropdownOptions[id]);
        dropdown.onValueChanged.AddListener((int value)=>
        {
            OnDropdownValueChange(id,value);
        });
        
        //UndoButton 撤回添加的单元
        uiTool.GetOrAddComponentInChilden<Button>("UndoButton").onClick.AddListener(()=>
        {
            if (curChooseUnitIndex<=1)
            {
                Debug.Log("已经没有单元了");
                return;
            }
            //把这个unit删除
            
            int index = Units.Count-1;
            GameObject unit = Units[index];
            GameObject.Destroy(unit);
            // Debug.Log("删除了"+unit.name+"xxx"+ Units.Count);
            Units.RemoveAt(index);
            yPlanningTable.Instance.RemoveMoveOrNoMoveAnimationList(index);
            
            curChooseUnitIndex--;
            
        });
        
        //GiveUpButton
        uiTool.GetOrAddComponentInChilden<Button>("GiveUpButton").onClick.AddListener(()=>
        {
            Pop();
            Push(new YMainPlayModeOriginPanel());
            YPlayModeController.Instance.GiveUpPanel();
            
            
        });
    }
    /// <summary>
    /// 是否超过了最大的index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
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
    
    public  override void OnExit()
    {
        base.OnExit();
        YTriggerEvents.RaiseOnShortcutKeyLockViewStateChanged(false);//停止 锁住视角 的快捷键的监听了
    }
}