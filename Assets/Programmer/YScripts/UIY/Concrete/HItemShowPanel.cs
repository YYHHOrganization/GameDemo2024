using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HItemShowPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/GameItemShowPanel";
    public HItemShowPanel() : base(new UIType(path)){}

    private GameObject leftScrollPanel;
    private GameObject middleShowPanel;
    private string leftScrollItemPrefabLink = "Prefabs/UI/singleUnit/AnItemGroup";
    
    public override void OnEnter()
    {
        leftScrollPanel = uiTool.GetOrAddComponentInChilden<Transform>("LeftScrollPanel").gameObject;
    }

    public void ShowItemsLeftScroll(Sprite image, string name, int count)
    {
        var anItem = GameObject.Instantiate(Resources.Load<GameObject>(leftScrollItemPrefabLink), leftScrollPanel.transform);
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

    
}
