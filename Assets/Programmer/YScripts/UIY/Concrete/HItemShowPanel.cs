using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HItemShowPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/GameItemShowPanel";
    public HItemShowPanel() : base(new UIType(path)){}

    private GameObject leftScrollPanel;
    private GameObject middleShowPanel;
    private GameObject showPreciousMiddlePanel;
    private GameObject giveOutItemPanel;
    private GameObject showPreciousGiveoutPanel;
    private string leftScrollItemPrefabLink = "Prefabs/UI/singleUnit/AnItemGroup";
    private string middleShowItemPrefabLink = "Prefabs/UI/singleUnit/BigItemShowGroup";
    private string giveOutItemPrefabLink = "Prefabs/UI/singleUnit/GiveoutItemGroup";
    private Button cancelButton;
    private Button confirmButton;

    private YInteractPortal portal;
    
    public override void OnEnter()
    {
        leftScrollPanel = uiTool.GetOrAddComponentInChilden<Transform>("LeftScrollPanel").gameObject;
        middleShowPanel = uiTool.GetOrAddComponentInChilden<Transform>("MiddleShowPanel").gameObject;
        middleShowPanel.gameObject.SetActive(false);
        showPreciousMiddlePanel = middleShowPanel.transform.Find("ShowPreciousItemPanel").gameObject;
        
        giveOutItemPanel = uiTool.GetOrAddComponentInChilden<Transform>("GiveOutPanel").gameObject;
        giveOutItemPanel.gameObject.SetActive(false);
        showPreciousGiveoutPanel = giveOutItemPanel.transform.Find("ItemShowPanel").gameObject;
        
        cancelButton = giveOutItemPanel.transform.Find("CancelButton").GetComponent<Button>();
        confirmButton = giveOutItemPanel.transform.Find("ConfirmButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }
    
    private void OnCancelButtonClick()
    {
        SetGiveOutPanelActive(false);
        YPlayModeController.Instance.LockPlayerInput(false);
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        Pop();
    }
    
    private void OnConfirmButtonClick()
    {
        bool result = portal.CheckCountIsRightOrNot();
        if (result)
        {
            Debug.Log("效果正确");
            HItemCounter.Instance.RemoveItem(portal.NeedId, portal.NeedCount);
            SetGiveOutPanelActive(false);
            YPlayModeController.Instance.LockPlayerInput(false);
            YTriggerEvents.RaiseOnMouseLockStateChanged(true);
            Pop();
            
            //TEST
            YLevelManager.NextLevel();
        }
        else
        {
            Debug.Log("效果不正确");
        }
        
    }

    public void ShowGiveOutItems(Sprite image, int needCount, int actualCount, YInteractPortal portal)
    {
        this.portal = portal;
        GameObject anItem = GameObject.Instantiate(Resources.Load<GameObject>(giveOutItemPrefabLink), showPreciousGiveoutPanel.transform);
        anItem.transform.DOLocalMoveX(0, 0.5f).From(true);
        Transform itemIcon = anItem.transform.Find("IconImage");
        if (itemIcon != null)
        {
            itemIcon.GetComponent<Image>().sprite = image;
        }
        else
        {
            Debug.LogError("itemIcon is null");
        }
        anItem.GetComponentInChildren<TMP_Text>().text = actualCount + " / " + needCount;
    }

    public void ShowItemsLeftScroll(Sprite image, string name, int count)
    {
        GameObject anItem = GameObject.Instantiate(Resources.Load<GameObject>(leftScrollItemPrefabLink), leftScrollPanel.transform);
        //Debug.Log(anItem.transform.localPosition); //为啥localPosition的x是100呢？
        //anItem.transform.DOLocalMoveX(200, 2);
        //anItem.transform.DOMoveX(5, 0.2f).From(true);
        anItem.transform.DOScale(new Vector3(0.9f, 0.9f, 1.0f), 0.2f).From(true);
        anItem.GetComponent<CanvasGroup>().DOFade(1.0f, 0.2f);
        Transform itemIcon = anItem.transform.Find("AnItem").Find("itemIcon");
            
        //uiTool.GetOrAddComponentInChilden<Transform>("itemIcon");
        if (itemIcon != null)
        {
            itemIcon.GetComponent<Image>().sprite = image;
        }
        else
        {
            Debug.LogError("itemIcon is null");
        }
        
        //anItem.transform.Find("itemIcon").GetComponent<Image>().sprite = image;
        anItem.GetComponentInChildren<TMP_Text>().text = name + " ×" + count;
        GameObject.Destroy(anItem, 3f);
    }
    
    public void SetLeftScrollPanelActive(bool active)
    {
        leftScrollPanel.SetActive(active);
    }
    
    public void SetMiddlePanelActive(bool active)
    {
        middleShowPanel.gameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        middleShowPanel.SetActive(active);
    }

    public void SetGiveOutPanelActive(bool active)
    {
        giveOutItemPanel.gameObject.SetActive(active);
        
    }

    public void SetMiddlePanelDeactivateFadeOff()
    {
        if (middleShowPanel)
        {
            middleShowPanel.gameObject.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.4f);
        }
    }
    
    public void ShowItemsMiddlePanel(Sprite image, string name, int count, string description)
    {
        if (!showPreciousMiddlePanel)
        {
            return;
        }
        GameObject anItem = GameObject.Instantiate(Resources.Load<GameObject>(middleShowItemPrefabLink), showPreciousMiddlePanel.transform);
        anItem.transform.DOScale(new Vector3(0.9f, 0.9f, 1.0f), 0.2f).From(true);
        anItem.GetComponent<CanvasGroup>().DOFade(1.0f, 0.2f);
        // anItem.GetComponent<Button>().onClick.AddListener(() =>
        // {
        //     var itemDescription = anItem.transform.Find("ItemDescription").gameObject;
        //     itemDescription.transform.Find("descriptionText").GetComponent<TMP_Text>().text = description;
        //     itemDescription.SetActive(true);
        // });
        Transform itemIcon = anItem.transform.Find("IconImage");
        itemIcon.GetComponent<Image>().sprite = image;
        anItem.transform.Find("ItemName").GetComponent<TMP_Text>().text = name;
        anItem.transform.Find("ItemCount").GetComponent<TMP_Text>().text = count.ToString();
    }

    
}
