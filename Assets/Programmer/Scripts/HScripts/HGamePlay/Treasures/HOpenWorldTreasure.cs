using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class HOpenWorldTreasure : MonoBehaviour
{
    public class ItemToShow
    {
        private string itemChineseName;
        private int number;
        private string itemImageLink;
        private bool isExpensive;
        private string description;
        
        //gets
        public string Description
        {
            get { return description; }
        }
        public string ItemChineseName
        {
            get { return itemChineseName; }
        }
        public int Number
        {
            get { return number; }
        }
        public string ItemImageLink
        {
            get { return itemImageLink; }
        }
        public bool IsExpensive
        {
            get { return isExpensive; }
        }

        public ItemToShow(string itemChineseName, int number, string itemImageLink, bool isExpensive, string description)
        {
            this.number = number;
            this.itemChineseName = itemChineseName;
            this.itemImageLink = itemImageLink;
            this.isExpensive = isExpensive;
            this.description = description;
        }
    }
    HItemShowPanel panel = new HItemShowPanel();
    
    // 每个大世界摆放的宝箱，提供用户交互后的逻辑，依据策划表获得奖励，并显示在UI界面上
    public HOpenWorldTreasureStruct treasure;
    private List<ItemToShow> itemsToShow = new List<ItemToShow>();

    public void SetTreasure(HOpenWorldTreasureStruct treasureStruct)
    {
        treasure = treasureStruct;
    }
    
    private void GiveoutTreasures()
    {
        //此时要解析字符串，并生成奖励
        string [] items = treasure.FixedItemString.Split(';');
        string [] nums = treasure.FixedNumString.Split(';');
        for (int i = 0; i < items.Length; i++)
        {
            string itemId = items[i];
            var thisItem = yPlanningTable.Instance.worldItems[itemId];
            
            string itemName = thisItem.chineseName;
            int itemNum = int.Parse(nums[i]);
            HItemCounter.Instance.AddItem(itemId, itemNum);
            Debug.Log("宝箱类型是： " + treasure.treasureType + ", 开出了 "+ itemNum + "个" + itemName);
            //todo:这些数据可以存于数据库当中，或者暂存在本地，但要给UI提供要显示的东西
            ItemToShow itemToShow = new ItemToShow(itemName, itemNum, thisItem.UIIconLink, thisItem.isExpensive, thisItem.description);
            itemsToShow.Add(itemToShow);
        }
        
        //解析一下随机奖励
        GiveOutRandomTreasures();
        Debug.Log("====================================================");
        //itemsToShow.Sort();
    }

    private void GiveOutRandomTreasures()
    {
        string[] itemGroups = treasure.RandomItemLogicString.Split('|');
        for (int i = 0; i < itemGroups.Length; i++)
        {
            string[] itemLogics = itemGroups[i].Split(':');
            int startIndex = int.Parse(itemLogics[0]);
            int endIndex = startIndex + int.Parse(itemLogics[1]);
            int count = int.Parse(itemLogics[2]);
            RollRandomItemAndAddToList(startIndex, endIndex, count);
        }

    }

    private void RollRandomItemAndAddToList(int startIndex, int endIndex, int count)
    {
        string[] itemIdAndCounts = treasure.RandomItemAndCountString.Split(";");
        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(startIndex, endIndex + 1);
            string idAndCounts = itemIdAndCounts[randomIndex];
            string itemId = idAndCounts.Split(':')[0];
            int itemNum = int.Parse(idAndCounts.Split(':')[1]);
            var thisItem = yPlanningTable.Instance.worldItems[itemId];
            string itemName = thisItem.chineseName;
            
            HItemCounter.Instance.AddItem(itemId, itemNum);
            Debug.Log("随机开出的东西 " + treasure.treasureType + ", 开出了 "+ itemNum + "个" + itemName);
            ItemToShow itemToShow = new ItemToShow(itemName,itemNum, thisItem.UIIconLink, thisItem.isExpensive, thisItem.description);
            itemsToShow.Add(itemToShow);
        }
    }
    
    public bool giveMeTreasure = false;
    
    //外界调用的入口函数，开启宝箱的全部逻辑
    public void GiveOutAllTreasuresAndDestroy()
    {
        if (treasure != null)
        {
            HAudioManager.Instance.Play("OpenChestMusic", gameObject);
            GiveoutTreasures();
            StartCoroutine(UIShowTreasures());
        }

        StartCoroutine(DestroySelf());
    }

    IEnumerator UIShowTreasures()
    {
        yield return new WaitForSeconds(0.2f);
        ShowItemsWithType();
    }

    private void ShowItemsWithType()
    {
        //todo:先把UI的逻辑写在这，后面看看能不能把这种弹窗的逻辑抽象出来
        switch (treasure.UILayoutType)
        {
            case "LeftRoll": //直接暴力的全部显示在左侧
                StartCoroutine(showEachItemOnLeftScroll(itemsToShow)); 
                break;
            case "LeftRollAndPreciousMiddle": //贵重物品中间显示，其他的左侧显示
                StartCoroutine(ShowPreciousItemInMiddle(itemsToShow)); 
                break;
        }
    }

    IEnumerator ShowPreciousItemInMiddle(List<ItemToShow> items)
    {
        //YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YGameRoot.Instance.Push(panel);
        panel.SetLeftScrollPanelActive(false);
        panel.SetMiddlePanelActive(true);
        yield return new WaitForSeconds(1f);
        foreach (var item in items)
        {
            if (item.IsExpensive)
            {
                AsyncOperationHandle<Sprite> handle =
                    Addressables.LoadAssetAsync<Sprite>(item.ItemImageLink);
                yield return handle;
                if (panel!=null)
                {
                    panel.ShowItemsMiddlePanel(handle.Result, item.ItemChineseName, item.Number, item.Description);
                }
                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(2f);
        panel.SetMiddlePanelDeactivateFadeOff();
        yield return new WaitForSeconds(0.5f);
        panel.Pop();
        StartCoroutine(showEachItemOnLeftScroll(items));
    }
    
    IEnumerator showEachItemOnLeftScroll(List<ItemToShow> items)
    {
        YGameRoot.Instance.Push(panel);
        panel.SetLeftScrollPanelActive(true);
        panel.SetMiddlePanelActive(false);
        foreach (var item in items)
        {
            //Debug.Log(item.ItemChineseName);
            AsyncOperationHandle<Sprite> handle =
                Addressables.LoadAssetAsync<Sprite>(item.ItemImageLink);
            yield return handle;
            panel.ShowItemsLeftScroll(handle.Result, item.ItemChineseName, item.Number);
            //Debug.Log(item.ItemChineseName);
            //Debug.Log("ddddddddddddddddddddd");
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(2f);
        YGameRoot.Instance.Pop();
        
    }

    public GameObject lightPlane;
    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(2f);
        //todo:这里调用一下消融效果可以加一个音效
        DissolvingControllery dissolving = gameObject.AddComponent<DissolvingControllery>();
        dissolving.SetMaterialsPropAndBeginDissolve(gameObject);

        yield return new WaitForSeconds(0.5f);
        if (lightPlane != null)
        {
            lightPlane.SetActive(false);
        }
        
        
    }

    private void Update()
    {
        if (giveMeTreasure)
        {
            if (treasure!=null)
            {
                GiveOutAllTreasuresAndDestroy();
                giveMeTreasure = false;
            }
        }
    }
}
