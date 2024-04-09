using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HBagPanelBaseLogic : MonoBehaviour
{
    //挂在BagPanel下面，因为要更新的东西太多了，直接挂上去
    public TMP_Text xingQiongNumber;
    public TMP_Text xinyongdianNumber;
    public Button addXingqiongButton;

    public Transform headIconKindGroups;
    
    public Transform ItemContent;

    public TMP_Text detailItemName;
    public TMP_Text detailItemKind;
    public Image detailItemIcon;

    public TMP_Text itemBaseDescription;
    public TMP_Text itemDetailDescription;
    
    public Image itemDescriptionBg;
    public TMP_Text itemDescriptionCount;

    private string itemShowLink = "Prefabs/UI/singleUnit/AnItemInBagPanel";

    public Button rogueItemUseButton;
    
    private void Start()
    {
        LoadBaseInfos();
        InstantiateAllItems();
        InstantiateAllRogueItems();
    }

    private void LoadBaseInfos()
    {
        //显示星琼和信用点数
        string xingQiongId = "20000000";
        string xinYongdianId = "20000001";
        int xingQiongNum = HItemCounter.Instance.CheckCountWithItemId(xingQiongId);
        int xinYongdianNum = HItemCounter.Instance.CheckCountWithItemId(xinYongdianId);
        xingQiongNumber.text = xingQiongNum.ToString();
        xinyongdianNumber.text = xinYongdianNum.ToString();
    }
    
    private void ShowItemDetailNothing()
    {
        detailItemIcon.sprite = Addressables.LoadAssetAsync<Sprite>("IX").WaitForCompletion();
        detailItemName.text = "<color=red>\u2588\u2588\u2588\u2588IX\u2588\u2588\u2588</color>";
        detailItemKind.text = "<color=red>一切归于虚无</color>";
        itemBaseDescription.text = "<color=red>你似乎来到了没有知识的荒漠......</color>";
        itemDetailDescription.text = "<color=red>烫烫烫烫烫烫烫烫烫</color>";
        itemDescriptionCount.text = "<color=red>× -1</color>";
    }

    private void InstantiateAllRogueItems()
    {
        int itemKindCount = HItemCounter.Instance.GetRogueDictLength();
        if (itemKindCount == 0)
        {
            ShowItemDetailNothing();
        }
        Dictionary<string, int> rogueItemCounts = HItemCounter.Instance.RogueItemCounts;
        bool isFirstItem = true;
        foreach (var item in rogueItemCounts)
        {
            string itemId = item.Key;
            int itemCount = item.Value;
            InstantiateAnRogueItem(itemId, itemCount);
            if (isFirstItem)
            {
                ShowRogueItemDetail(itemId, itemCount);
                isFirstItem = false;
            }
        }
        rogueItemUseButton.GetComponent<Button>().onClick.AddListener(
            ()=>
            {
                if (nowClickItemId!=null)
                {
                    HRougeAttributeManager.Instance.UsePositiveItem(nowClickItemId);
                    HItemCounter.Instance.RemoveItemInRogue(nowClickItemId, 1);
                    RefleshBagPanel();
                }
            });
    }
    
    private void RemoveAndDestroyAllChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void RefleshBagPanel()
    {
        RemoveAndDestroyAllChildren(ItemContent.transform);
        InstantiateAllItems();
        InstantiateAllRogueItems();
    }
    
    private void InstantiateAllItems()
    {
        int itemKindCount = HItemCounter.Instance.GetDictLength();
        if (itemKindCount == 0)
        {
            ShowItemDetailNothing();
        }
        Dictionary<string, int> worldItemCounts = HItemCounter.Instance.WorldItemCounts;
        bool isFirstItem = true;
        foreach (var item in worldItemCounts)
        {
            string itemId = item.Key;
            int itemCount = item.Value;
            InstantiateAnItem(itemId, itemCount);
            if (isFirstItem)
            {
                ShowItemDetail(itemId, itemCount);
                isFirstItem = false;
            }
        }
    }

    private string nowClickItemId;
    private void InstantiateAnItem(string itemId, int itemCount)
    {
        //实例化一个物品
        GameObject item = Instantiate(Resources.Load<GameObject>(itemShowLink), ItemContent);
        TMP_Text itemCountText = item.GetComponentInChildren<TMP_Text>();
        itemCountText.text = itemCount.ToString();
        
        HOpenWorldItemStruct thisItem = yPlanningTable.Instance.worldItems[itemId];
        
        //设置物品的图标
        Image itemIcon = item.transform.Find("ItemIcon").GetComponent<Image>();
        string iconLink = thisItem.UIIconLink;
        var op2 = Addressables.LoadAssetAsync<Sprite>(iconLink);
        Sprite go = op2.WaitForCompletion();
        itemIcon.sprite = go;
        
        //设置背景颜色和物品星级
        Image bgImage = item.transform.Find("BgImage").GetComponent<Image>();
        int starLevel = thisItem.starLevel;
        switch (starLevel)
        {
            case 3:
                bgImage.color = new Color(0.251f,0.8078f,1,0.8196f);
                break;
            case 4:
                bgImage.color = new Color(1, 0.251f, 0.847f, 0.8196f);
                break;
            case 5:
                bgImage.color = new Color(1,0.741f,0.251f,0.8196f);
                break;
        }
        Transform starGroupParent = item.transform.Find("Stars");
        for (int i = 0; i < starLevel; i++)
        {
            starGroupParent.GetChild(i).gameObject.SetActive(true);
        }
        
        //添加点击的监听事件
        item.GetComponent<Button>().onClick.AddListener(() =>
        {
            string tmpItemId = itemId;
            ShowItemDetail(tmpItemId, itemCount);
        });
    }
    
    private void InstantiateAnRogueItem(string itemId, int itemCount)
    {
        //实例化一个物品
        GameObject item = Instantiate(Resources.Load<GameObject>(itemShowLink), ItemContent);
        TMP_Text itemCountText = item.GetComponentInChildren<TMP_Text>();
        itemCountText.text = itemCount.ToString();
        
        RogueItemBaseAttribute thisItem = yPlanningTable.Instance.rogueItemBases[itemId];
        
        //设置物品的图标
        Image itemIcon = item.transform.Find("ItemIcon").GetComponent<Image>();
        string iconLink = thisItem.rogueItemIconLink;
        var op2 = Addressables.LoadAssetAsync<Sprite>(iconLink);
        Sprite go = op2.WaitForCompletion();
        itemIcon.sprite = go;
        
        //设置背景颜色和物品星级
        Image bgImage = item.transform.Find("BgImage").GetComponent<Image>();
        int starLevel = thisItem.starLevel;
        switch (starLevel)
        {
            case 3:
                bgImage.color = new Color(0.251f,0.8078f,1,0.8196f);
                break;
            case 4:
                bgImage.color = new Color(1, 0.251f, 0.847f, 0.8196f);
                break;
            case 5:
                bgImage.color = new Color(1,0.741f,0.251f,0.8196f);
                break;
            default:
                bgImage.color = new Color(0.251f,0.8078f,1,0.6196f);
                break;
        }
        Transform starGroupParent = item.transform.Find("Stars");
        for (int i = 0; i < starLevel; i++)
        {
            starGroupParent.GetChild(i).gameObject.SetActive(true);
        }
        
        //添加点击的监听事件
        item.GetComponent<Button>().onClick.AddListener(() =>
        {
            string tmpItemId = itemId;
            nowClickItemId = tmpItemId;
            ShowRogueItemDetail(tmpItemId, itemCount);
        });
    }

    private void ShowRogueItemDetail(string itemId, int itemCount)
    {
        RogueItemBaseAttribute thisItem = yPlanningTable.Instance.rogueItemBases[itemId];
        detailItemIcon.sprite = Addressables.LoadAssetAsync<Sprite>(thisItem.rogueItemIconLink).WaitForCompletion();
        detailItemName.text = thisItem.itemChineseName;
        detailItemKind.text = thisItem.itemFollowXingshenChinese;
        itemBaseDescription.text = thisItem.rogueItemDescription;
        itemDetailDescription.text = "暂时还没有写好";
        switch (thisItem.starLevel)
        {
            case 3:
                itemDescriptionBg.color = new Color(0.251f,0.8078f,1,0.6196f);
                break;
            case 4:
                itemDescriptionBg.color = new Color(1, 0.251f, 0.847f, 0.6196f);
                break;
            case 5:
                itemDescriptionBg.color = new Color(1,0.741f,0.251f,0.6196f);
                break;
            default:
                itemDescriptionBg.color = new Color(0.251f,0.8078f,1,0.6196f);
                break;
        }
        itemDescriptionCount.text = "×" + itemCount.ToString();
        if (thisItem.rogueItemKind == "Positive")
        {
            rogueItemUseButton.gameObject.SetActive(true);
        }
        else
        {
            rogueItemUseButton.gameObject.SetActive(false);
        }
    }

    private void ShowItemDetail(string itemId, int itemCount)
    {
        HOpenWorldItemStruct thisItem = yPlanningTable.Instance.worldItems[itemId];
        detailItemIcon.sprite = Addressables.LoadAssetAsync<Sprite>(thisItem.UIIconLink).WaitForCompletion();
        detailItemName.text = thisItem.chineseName;
        detailItemKind.text = thisItem.itemKindChinese;
        itemBaseDescription.text = thisItem.description;

        itemDetailDescription.text = "暂时还没有写好";
        switch (thisItem.starLevel)
        {
            case 3:
                itemDescriptionBg.color = new Color(0.251f,0.8078f,1,0.6196f);
                break;
            case 4:
                itemDescriptionBg.color = new Color(1, 0.251f, 0.847f, 0.6196f);
                break;
            case 5:
                itemDescriptionBg.color = new Color(1,0.741f,0.251f,0.6196f);
                break;
        }

        itemDescriptionCount.text = "×" + itemCount.ToString();
    }
}

