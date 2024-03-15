using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class YEntershortcutKey : MonoBehaviour
{
    //单例
    
    L2PlayerInput playerInput;
    
    private void Awake()
    {
        playerInput = new L2PlayerInput();
        playerInput.ShortcutKey.GetPuppet.started += EnterPuppet;
    }

    // Start is called before the first frame update
    void Start()
    {
        YTriggerEvents.OnPuppetShortCutStateChanged += SetInputActionEnableOrDisable;
        
        //暂时在这里处理鼠标的锁定和解锁
        YTriggerEvents.OnMouseLockStateChanged += OnMouseLockStateChanged;
        
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
    }
    private void OnEnable()
    {
        //playerInput.ShortcutKey.Enable();
    }
    
    public void SetInputActionEnableOrDisable(object sender, YTriggerEventArgs e)
    {
        if (e.activated)
        {
            playerInput.ShortcutKey.Enable();
        }
        else
        {
            playerInput.ShortcutKey.Disable();
        }
    }
    
    public void SetInputActionDisableOrEnable(bool InputActionEnable)
    {
        if (InputActionEnable)
        {
            playerInput.ShortcutKey.Enable();
        }
        else
        {
            playerInput.ShortcutKey.Disable();
        }
    }
    

    private void OnDisable()
    {
        playerInput.ShortcutKey.Disable();
    }
}
