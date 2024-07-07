using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YRuiZaoPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/RuiZaoLiuHePanel/YRuiZaoPanel";
    YRuiZaoScripts yRuiZaoScripts;

    public YRuiZaoPanel(YRuiZaoScripts yRuiZaoScripts) : base(new UIType(path))
    {
        this.yRuiZaoScripts = yRuiZaoScripts;
    }
    public YRuiZaoPanel(YRuiZaoScripts yRuiZaoScripts,bool isRogue) : base(new UIType(path))
    {
        this.yRuiZaoScripts = yRuiZaoScripts;
    }
    
    public override void OnEnter()
    {
        //出现鼠标
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        //锁住人物运动
        
        
        //顺时针或者逆时针旋转片
       //RotateBtnA0
       //调用YRuiZaoScripts的RotateBtnA0方法
       uiTool.GetOrAddComponentInChilden<Button>("RotateBtnA0").onClick.AddListener(()=>
       {
           yRuiZaoScripts.RotateBtnClick(0);
       });
       uiTool.GetOrAddComponentInChilden<Button>("RotateBtnA1").onClick.AddListener(()=>
       {
           yRuiZaoScripts.RotateBtnClick(1);
       });

       //选择哪一片
        //SelectMianBtn0-5 调用YRuiZaoScripts的SelectMianBtnClick方法yRuiZaoScripts.BtnClickSetBtnIndex();
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            uiTool.GetOrAddComponentInChilden<Button>("SelectMianBtn" + i).onClick.AddListener(()=>
            {
                yRuiZaoScripts.BtnClickSetBtnIndex(index);
            });
        }
        
        //旋转视角/相机
        //RotateViewleft 调用YRuiZaoScripts的yRuiZaoScripts.BtnClickRotateCamera(0);
        //RotateViewRight 调用YRuiZaoScripts的yRuiZaoScripts.BtnClickRotateCamera(1);
        uiTool.GetOrAddComponentInChilden<Button>("RotateViewleft").onClick.AddListener(()=>
        {
            yRuiZaoScripts.BtnClickRotateCamera(0);
        });
        uiTool.GetOrAddComponentInChilden<Button>("RotateViewright").onClick.AddListener(()=>
        {
            yRuiZaoScripts.BtnClickRotateCamera(1);
        });
        //ExitButton
        uiTool.GetOrAddComponentInChilden<Button>("ExitButton").onClick.AddListener(()=>
        {
            //弹出是否真的要退出挑战？
             HMessageShowMgr.Instance.ShowMessageWithActions("ConfirmExitRuizao", ExitRuizao, null,null);
        });
        uiTool.GetOrAddComponentInChilden<Button>("TutButton").onClick.AddListener(()=>
        {
            //弹出tutorial
            HMessageShowMgr.Instance.ShowMessage("Tut_LearnRuizao");
            
        });
    }

    public void ExitRuizao()
    {
        //Pop();
        RemoveSelfPanel();
        Debug.Log("退出挑战");
        yRuiZaoScripts.ExitAndNoWin();
    }
}