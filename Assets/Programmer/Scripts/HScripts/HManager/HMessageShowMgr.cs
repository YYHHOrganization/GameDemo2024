using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HMessageShowMgr : MonoBehaviour
{
    private Transform messagePanel;
    private Transform messageKind1Panel;
    private Transform messageKind2Panel;
    private string messageKind2UIPath = "messageKind2Prefab";
    private string messageKind3UIPath = "messageKind3Prefab";
    private string messageKind4UIPath = "messageKind4Prefab";
    
    private string aSimpleMessageUILink = "aSimpleMessageUILink"; //一条消息，会放在对应消息类型的Panel当中 
    private string aSimpleMessageUIKind4Link = "aSimpleMessageUIKind4Link"; 
    private GameObject canvas;
    private TMP_Text messageKind2Content;
    private TMP_Text messageKind7Content;

    private string messageConfirmBoxLink = "MessageConfirmBox";
    private string messageGiveoutBoxLink = "MessageGiveoutBox";
    private Transform messageKind5Panel;
    private Transform messageKind7Panel;

    private Transform messageKind8Panel; //tips消息对应，在右上角显示，比如说倒计时
    
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
        messageKind5Panel = messagePanel.Find("messageKind5Prefab");
        messageKind7Panel = messagePanel.Find("messageKind7Prefab");
        messageKind8Panel = messagePanel.Find("messageKind8Prefab");
        messageKind2Content = messageKind2Panel.GetComponentInChildren<TMP_Text>();
        messageKind7Content = messageKind7Panel.GetComponentInChildren<TMP_Text>();
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
    
    
    
    private void ShowMessageKind2(MessageBoxBaseStruct message)
    {
        transform.SetAsLastSibling();
        messageKind2Panel.gameObject.SetActive(true);
        //弹出在中间的弹窗，比如游戏的教程，可以有加载的教程内容链接,按x键直接关闭
        string messageContent = message.MessageContent;
        messageKind2Content.text = messageContent;
        string messageLink = message.MessageLink;
        var op2 = Addressables.InstantiateAsync(messageLink, messageKind2Panel);
        GameObject go = op2.WaitForCompletion();
        DoMessageTransitionEffect(go.transform, message.MessageTransitionEffect, message.MessageShowTime);
        YPlayModeController.Instance.LockPlayerInput(true);
    }
    
    private void ShowMessageKind7(MessageBoxBaseStruct message)
    {
        transform.SetAsLastSibling();
        messageKind7Panel.gameObject.SetActive(true);
        //弹出在中间的弹窗，比如游戏的教程，可以有加载的教程内容链接,按x键直接关闭
        string messageContent = message.MessageContent;
        messageKind7Content.text = messageContent;
        string messageLink = message.MessageLink;
        var op2 = Addressables.InstantiateAsync(messageLink, messageKind7Panel);
        GameObject go = op2.WaitForCompletion();
        DoMessageTransitionEffect(go.transform, message.MessageTransitionEffect, message.MessageShowTime);
        YPlayModeController.Instance.LockPlayerInput(true);
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
            case "ScaleFadeInNoOut":
                go.parent.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.4f).From();
                go.transform.GetComponent<CanvasGroup>().DOFade(1.0f, 1f);
                break;
        }
    }
    
    
    private void ShowMessageKind3(MessageBoxBaseStruct message)
    {
        //屏幕下方弹出的消息，
    }

    private void ShowMessageKind4(MessageBoxBaseStruct message)
    {
        //呈现在屏幕上方的UI
        string messageContent = message.MessageContent;
        float messageShowTime = message.MessageShowTime;
        string messageTransitionEffect = message.MessageTransitionEffect;

        var op2 = Addressables.InstantiateAsync(aSimpleMessageUIKind4Link, messageKind1Panel);
        GameObject go = op2.WaitForCompletion();
        go.transform.GetComponentInChildren<TMP_Text>().text = messageContent;
        DoMessageTransitionEffect(go.transform, messageTransitionEffect, messageShowTime);
        Destroy(go, messageShowTime);
    }

    private void ShowMessageKind5(MessageBoxBaseStruct message, Action confirmAction, Action cancelAction=null, Action closeAction=null)
    {
        transform.SetAsLastSibling();
        var op2 = Addressables.InstantiateAsync(messageConfirmBoxLink, messageKind5Panel);
        GameObject go = op2.WaitForCompletion();
        Button cancelButton = go.transform.Find("CancelButton").GetComponent<Button>();
        Button confirmButton = go.transform.Find("ConfirmButton").GetComponent<Button>();
        TMP_Text messageContent = go.transform.Find("messageContent").GetComponent<TMP_Text>();
        messageContent.text = message.MessageContent;
        
        Button closeButton = go.transform.Find("ReturnButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() =>
        {
            YPlayModeController.Instance.LockPlayerInput(false);
            transform.SetAsFirstSibling();
            closeAction?.Invoke();
            Destroy(go);
        });
        cancelButton.onClick.AddListener(() =>
        {
            YPlayModeController.Instance.LockPlayerInput(false);
            transform.SetAsFirstSibling();
            cancelAction?.Invoke();
            Destroy(go);
        });
        confirmButton.onClick.AddListener(() =>
        {
            YPlayModeController.Instance.LockPlayerInput(false);
            transform.SetAsFirstSibling();
            confirmAction?.Invoke();
            Destroy(go);
        });
        YTriggerEvents.RaiseOnMouseLockStateChanged(false); //鼠标呼出的状态
        YPlayModeController.Instance.LockPlayerInput(true);
    }
    
    private void ShowMessageKind6(MessageBoxBaseStruct message, Action confirmAction, Action cancelAction, Action closeAction, GameObject gameObject = null)
    {
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YPlayModeController.Instance.LockPlayerInput(true);
        
        HBagToGiveoutPanel bagPanel = new HBagToGiveoutPanel();
        YGameRoot.Instance.Push(bagPanel);
        
        var op2 = Addressables.InstantiateAsync(messageGiveoutBoxLink, messageKind5Panel);
        GameObject go = op2.WaitForCompletion();
        //从背包中提交物品的逻辑
        transform.SetAsLastSibling();
        bagPanel.uiTool.Get<HBagToGiveoutBaseLogic>().SetGiveoutPanel(go);
        
        TMP_Text messageContent = go.transform.Find("messageContent").GetComponent<TMP_Text>();
        messageContent.text = message.MessageContent;
        Button cancelButton = go.transform.Find("CancelButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(() =>
        {
            YTriggerEvents.RaiseOnMouseLeftShoot(true);
            Debug.Log("Cancel");
            YPlayModeController.Instance.LockPlayerInput(false);
            transform.SetAsFirstSibling();
            cancelAction?.Invoke();
            YGameRoot.Instance.Pop();
            Destroy(go);
        });
        //return button
        Button closeButton = go.transform.Find("ReturnButton").GetComponent<Button>();
        closeButton.onClick.AddListener(() =>
        {
            YTriggerEvents.RaiseOnMouseLeftShoot(true);
            Debug.Log("Close");
            YPlayModeController.Instance.LockPlayerInput(false);
            transform.SetAsFirstSibling();
            closeAction?.Invoke();
            YGameRoot.Instance.Pop();
            Destroy(go);
        });
        
        Button confirmButton = go.transform.Find("ConfirmButton").GetComponent<Button>();
        confirmButton.onClick.AddListener(() =>
        {
            YTriggerEvents.RaiseOnMouseLeftShoot(true);
            Debug.Log("Confirm");
            YPlayModeController.Instance.LockPlayerInput(false);
            transform.SetAsFirstSibling();
            HItemCounter.Instance.RemoveItem(go.GetComponent<HGiveoutPanelCollectBagInfo>().itemId,go.GetComponent<HGiveoutPanelCollectBagInfo>().itemCount);
            YTriggerEvents.RaiseOnGiveOutItemInBagForSlotMachine(go.GetComponent<HGiveoutPanelCollectBagInfo>().itemId,go.GetComponent<HGiveoutPanelCollectBagInfo>().itemCount);
            YGameRoot.Instance.Pop();
            confirmAction?.Invoke();
            Destroy(go);
        });
    }

    public void ShowMessageWithActions(string messageId, Action confirmAction, Action cancelAction, Action closeAction, GameObject gameobject=null)
    {
        MessageBoxBaseStruct message = yPlanningTable.Instance.Messages[messageId];
        if (message!=null)
        {
            int messageKind = message.MessageType;
            switch (messageKind)
            {
                case 5:
                    ShowMessageKind5(message, confirmAction, cancelAction, closeAction);
                    break;
                case 6:
                    ShowMessageKind6(message, confirmAction, cancelAction, closeAction, gameobject);
                    break;
            }
        }
    }
    
    
    
    public void ShowMessage(string messageId, string overrideMessageContent=null)
    {
        MessageBoxBaseStruct message = yPlanningTable.Instance.Messages[messageId];
        if (message!=null)
        {
            if (overrideMessageContent!=null)
            {
                message.SetMessage(overrideMessageContent);
            }
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
                case 7:
                    ShowMessageKind7(message);
                    break;
                    
            }
        }
    }

    public void ShowTickMessage(string messageInfo, int seconds, Action callFunction)
    {
        StartCoroutine(TickMessageWithContentAndTime(messageInfo, seconds, callFunction));
    }
    
    string FormatSeconds(int seconds)
    {
        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return $"{minutes:D2}:{remainingSeconds:D2}";
    }

    IEnumerator TickMessageWithContentAndTime(string messageInfo, int seconds, Action callFunction)
    {
        int startSeconds = seconds;
        string messageContent = messageInfo;
        var op2 = Addressables.InstantiateAsync(aSimpleMessageUILink, messageKind8Panel);
        GameObject go = op2.WaitForCompletion();
        while (startSeconds > 0)
        {
            string timeString = FormatSeconds(startSeconds);
            go.transform.GetComponentInChildren<TMP_Text>().text = messageContent + timeString;
            yield return new WaitForSeconds(1f);
            startSeconds--;
        }
        callFunction?.Invoke();
        Destroy(go, 1f);
    }
    
}
