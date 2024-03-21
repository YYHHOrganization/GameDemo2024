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
    private string leftScrollItemPrefabLink = "Prefabs/UI/singleUnit/AnItemGroup";
    private string middleShowItemPrefabLink = "Prefabs/UI/singleUnit/BigItemShowGroup";
    
    public override void OnEnter()
    {
        leftScrollPanel = uiTool.GetOrAddComponentInChilden<Transform>("LeftScrollPanel").gameObject;
        middleShowPanel = uiTool.GetOrAddComponentInChilden<Transform>("MiddleShowPanel").gameObject;
        middleShowPanel.gameObject.SetActive(false);
        showPreciousMiddlePanel = middleShowPanel.transform.Find("ShowPreciousItemPanel").gameObject;
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

    public void SetMiddlePanelDeactivateFadeOff()
    {
        middleShowPanel.gameObject.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.4f);
    }
    
    public void ShowItemsMiddlePanel(Sprite image, string name, int count, string description)
    {
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
