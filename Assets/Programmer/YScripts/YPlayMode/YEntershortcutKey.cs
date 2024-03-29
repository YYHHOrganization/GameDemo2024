using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class YEntershortcutKey : MonoBehaviour
{
    //单例
    
    L2PlayerInput playerInput;
    private void Awake()
    {
        playerInput = new L2PlayerInput();
        //playerInput.ShortcutKey.GetPuppet.started += EnterPuppet;
        
        playerInput.AfterSplitScreenShortCut.SplitScreen1.started += context => SetSplitScreen(0);
        playerInput.AfterSplitScreenShortCut.SplitScreen2.started += context => SetSplitScreen(1);
        playerInput.AfterSplitScreenShortCut.SplitScreen3.started += context => SetSplitScreen(2);
        
        playerInput.Interaction.interact.started += context =>SetInteractResult();

        playerInput.Always.Exit.started += context => ExitPanelOn();
        playerInput.Always.Enable();

        playerInput.LockView.Lock.started += context => SetLockView();
        playerInput.LockView.GiveUpBackPanel.started += context => ExitPanelOn();
    }

    // Start is called before the first frame update
    void Start()
    {
        YTriggerEvents.OnPuppetShortCutStateChanged += SetInputActionEnableOrDisable;
        
        //暂时在这里处理鼠标的锁定和解锁
        YTriggerEvents.OnMouseLockStateChanged += OnMouseLockStateChanged;
        
        //启用快捷键分屏事件
        YTriggerEvents.OnShortcutKeySplitScreenStateChanged += OnEnterSplitScreenStateChanged;
        
        ////启动出现提示面板，并可以使用快捷键进行交互
        YTriggerEvents.OnShortcutKeyInteractionStateChanged += SetInteractionEnableOrDisable;
        
        //退出快捷键启用与否
        YTriggerEvents.OnShortcutKeyEsc+=SetESCEnableOrDisable;
        
        //启用快捷键进入锁住视角
        YTriggerEvents.OnShortcutKeyLockView+=SetLockViewEnableOrDisable;
    }
    
    private void SetSplitScreen(int splitScreenType)
    {
        YPlayModeController.Instance.SetCameraLayout(splitScreenType);//全 小 半
    }

    //暂时在这里处理鼠标的锁定和解锁
    private void OnMouseLockStateChanged(object sender, YTriggerEventArgs e)
    {
        if (e.activated)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void EnterPuppet(InputAction.CallbackContext context)
    {
        Debug.Log("EnterPuppet");
        //进入开始场景
        YGameRoot.Instance.Pop();
        YGameRoot.Instance.Push(new YChooseScreenplayInPlayModePanel());
        YPlayModeController.Instance.SetCameraLayout(2);
        HAudioManager.Instance.Play("SelectPuppetActionMusic", HAudioManager.Instance.gameObject);
    }
    private void OnEnable()
    {
        playerInput.ShortcutKey.Enable();
    }
    
    public void SetInputActionEnableOrDisable(object sender, YTriggerEventArgs e)
    {
        if (e.activated)
        {
            //playerInput.ShortcutKey.Enable();
            playerInput.ShortcutKey.GetPuppet.started += EnterPuppet;
        }
        else
        {
            //playerInput.ShortcutKey.Disable();
            playerInput.ShortcutKey.GetPuppet.started -= EnterPuppet;
        }
    }
    public void SetESCEnableOrDisable(object sender, YTriggerEventArgs e)
    {
        if (e.activated)
        {
            playerInput.Always.Enable();
        }
        else
        {
            playerInput.Always.Disable();
        }
    }
    public GameObject interactGo;
    //启动出现提示面板，并可以使用快捷键进行交互
    public void SetInteractionEnableOrDisable(object sender, YTriggerGameObjectEventArgs e)
    {
        interactGo = e.gameObject;
        if (e.activated)
        {
            playerInput.Interaction.Enable();
        }
        else
        {
            playerInput.Interaction.Disable();
        }
    
    }
    private void SetInteractResult()
    {
        Debug.Log("SetInteractResult");
        // YTriggerTagUnit triggerTagUnit = interactGo.GetComponent<YTriggerTagUnit>();
        
        
        
        YTriggerTagUnit triggerTagUnit = interactGo.GetComponentInChildren<YTriggerTagUnit>();
        triggerTagUnit.OnActive();
    }
    public void SetLockViewEnableOrDisable(object sender, YTriggerEventArgs e)
    {
        if (e.activated)
        {
            playerInput.LockView.Enable();
        }
        else
        {
            playerInput.LockView.Disable();
        }
    }
    
    bool shouldLockPlayerInput=false;
    private void SetLockView()
    {
        shouldLockPlayerInput = !shouldLockPlayerInput;
        YPlayModeController.Instance.LockPlayerInput(shouldLockPlayerInput);
    }
    

    private void OnDisable()
    {
        playerInput.ShortcutKey.Disable();
        //playerInput.ShortcutKey.GetPuppet.started -= EnterPuppet;
    }
    
    private void OnEnterSplitScreenStateChanged(object sender, YTriggerEventArgs e)
    {
        //可以通过123来控制分屏的类型
        if (e.activated)
        {
            playerInput.AfterSplitScreenShortCut.Enable();
        }
        else
        {
            playerInput.AfterSplitScreenShortCut.Disable();
        }
    }
    
    private void ExitPanelOn()
    {
        YGameRoot.Instance.Push(new YExitPanel());
    }
}
