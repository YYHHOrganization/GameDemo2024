using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HMessageShowMgr : MonoBehaviour
{
    private Transform messagePanel;
    private Transform messageKind1Panel;
    private Transform messageKind2Panel;
    private string messageKind2UIPath = "messageKind2Prefab";
    private string messageKind3UIPath = "messageKind3Prefab";
    private string messageKind4UIPath = "messageKind4Prefab";
    
    private string aSimpleMessageUILink = "aSimpleMessageUILink"; //一条消息，会放在对应消息类型的Panel当中 
    private GameObject canvas;
    
    //单例模式
    private static HMessageShowMgr instance;
    public static HMessageShowMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HMessageShowMgr>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        canvas = GameObject.Find("Canvas");
        messagePanel = canvas.transform.Find("HShowMessagePanel");
        messageKind1Panel = messagePanel.Find("messageKind1Prefab");
        messageKind2Panel = messagePanel.Find("messageKind2Prefab");
    }

    private void ShowMessageKind1(MessageBoxBaseStruct message)
    {
        //呈现在屏幕上方的UI
        string messageContent = message.MessageContent;
        float messageShowTime = message.MessageShowTime;
        string messageTransitionEffect = message.MessageTransitionEffect;

        var op2 = Addressables.InstantiateAsync(aSimpleMessageUILink, messageKind1Panel);
        GameObject go = op2.WaitForCompletion();
        go.transform.GetComponentInChildren<TMP_Text>().text = messageContent;
        DoMessageTransitionEffect(go.transform, messageTransitionEffect, messageShowTime);
        Destroy(go, messageShowTime);
    }

    private void DoMessageTransitionEffect(Transform go, string messageTransitionEffect, float showTime)
    {
        //消息的过渡效果，比如淡入淡出，从左到右，从右到左等等
        switch (messageTransitionEffect)
        {
            case "FadeInAndFadeOut":
                go.transform.GetComponent<CanvasGroup>().DOFade(1.0f, showTime/4.0f);
                go.transform.GetComponent<CanvasGroup>().DOFade(0.0f, showTime/4.0f).SetDelay(showTime/2.0f);
                break;
            case "FadeInNoOut":
                go.transform.GetComponent<CanvasGroup>().DOFade(1.0f, 0.3f);
                break;
        }
    }
    
    
    private void ShowMessageKind2(MessageBoxBaseStruct message)
    {
        //弹出在中间的弹窗，比如游戏的教程，可以有加载的教程内容链接,按x键直接关闭
        
    }
    
    private void ShowMessageKind3(MessageBoxBaseStruct message)
    {
        //屏幕下方弹出的消息，
    }

    private void ShowMessageKind4(MessageBoxBaseStruct message)
    {
        //屏幕中间弹出的消息，
    }
    
    public void ShowMessage(string messageId)
    {
        MessageBoxBaseStruct message = yPlanningTable.Instance.Messages[messageId];
        if (message!=null)
        {
            int messageKind = message.MessageType;
            switch (messageKind)
            {
                case 1:
                    ShowMessageKind1(message);
                    break;
                case 2:
                    ShowMessageKind2(message);
                    break;
                case 3:
                    ShowMessageKind3(message);
                    break;
                case 4:
                    ShowMessageKind4(message);
                    break;
                    
            }
        }
    }
    
}
