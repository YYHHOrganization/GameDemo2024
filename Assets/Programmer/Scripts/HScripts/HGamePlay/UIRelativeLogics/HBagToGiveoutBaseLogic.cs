using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HBagToGiveoutBaseLogic : MonoBehaviour
{
    //提交物品的Panel，与背包逻辑类似，但是Panel不同，因为背包太大了
    public Transform headIconKindGroups;
    
    public Transform ItemContent;

    public TMP_Text detailItemName;
    public TMP_Text detailItemKind;
    public Image detailItemIcon;

    public TMP_Text itemBaseDescription;
    
    public Image itemDescriptionBg;
    public TMP_Text itemDescriptionCount;

    public Transform itemDetailPanel;
    private string itemShowLink = "Prefabs/UI/singleUnit/AnItemInBagPanel";
    
    private GameObject confirmGiveoutPanel;
    private HGiveoutPanelCollectBagInfo script;
    private TMP_Text giveOutItemCount;
    private GameObject giveOutItemStars;
    private Image giveOutItemIcon;
    private Image giveOutItemBg;
    private Transform giveOutThingPanel;

    public void SetGiveoutPanel(GameObject go)
    {
        confirmGiveoutPanel = go;
        giveOutThingPanel = go.transform.Find("GiveOutThingPanel");
        giveOutItemCount = giveOutThingPanel.Find("ItemCountText").GetComponent<TMP_Text>();
        giveOutItemStars = giveOutThingPanel.Find("Stars").gameObject;
        giveOutItemIcon = giveOutThingPanel.Find("ItemIcon").GetComponent<Image>();
        giveOutItemBg = giveOutThingPanel.Find("BgImage").GetComponent<Image>();
        script = confirmGiveoutPanel.gameObject.GetComponent<HGiveoutPanelCollectBagInfo>();
    }
    
    private void Start()
    {
        InstantiateAllItems();
    }
    
    private void ShowItemDetailNothing()
    {
        detailItemIcon.sprite = Addressables.LoadAssetAsync<Sprite>("IX").WaitForCompletion();
        detailItemName.text = "<color=red>\u2588\u2588\u2588\u2588IX\u2588\u2588\u2588</color>";
        detailItemKind.text = "<color=red>一切归于虚无</color>";
        itemBaseDescription.text = "<color=red>你似乎来到了没有知识的荒漠......</color>";
        itemDescriptionCount.text = "<color=red>× -1</color>";
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

        if (thisItem.couldBeExchanged)
        {
            item.transform.SetAsFirstSibling();
            //添加点击的监听事件
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                string tmpItemId = itemId;
                ShowItemDetail(tmpItemId, itemCount);
                itemDetailPanel.gameObject.SetActive(true);
                if (confirmGiveoutPanel)
                {
                    giveOutThingPanel.gameObject.SetActive(true);
                    giveOutItemCount.text = itemCount.ToString();
                    for (int i = 0; i < thisItem.starLevel; i++)
                    {
                        giveOutItemStars.transform.GetChild(i).gameObject.SetActive(true);
                    }

                    giveOutItemIcon.sprite = go;
                    giveOutItemBg.color = bgImage.color;
                    
                    script.itemId = tmpItemId;
                    script.itemCount = itemCount;
                }
                
            });
        }
        else
        {
            item.GetComponent<Button>().interactable = false;
        }
        
    }

    private void ShowItemDetail(string itemId, int itemCount)
    {
        HOpenWorldItemStruct thisItem = yPlanningTable.Instance.worldItems[itemId];
        detailItemIcon.sprite = Addressables.LoadAssetAsync<Sprite>(thisItem.UIIconLink).WaitForCompletion();
        detailItemName.text = thisItem.chineseName;
        detailItemKind.text = thisItem.itemKindChinese;
        itemBaseDescription.text = thisItem.description;
        
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
