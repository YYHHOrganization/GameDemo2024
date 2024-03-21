using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YInteract2AndChest :YIInteractiveGroup
{
    public GameObject chest;
    public Transform chestTrans;
    
    public void Start()
    {
        base.Start();
        GetChestAndSetTrans();
    }
    public void GetChestAndSetTrans()
    {
        //寻找id是否存在于interactiveGroupDict中
        if (!yPlanningTable.Instance.interactiveGroupDict.ContainsKey(InteractGroupID))
        {
            Debug.Log("interactiveGroupDict中机关ID不存在");
            return;
        }
        int chestID = yPlanningTable.Instance.interactiveGroupDict[InteractGroupID].TreasureID;
        Debug.Log("宝箱ID：" + chestID);
        
        // HOpenWorldTreasureStruct treasure = HOpenWorldTreasureManager.Instance.GetTreasure(chestID);
        
        //更新宝箱位置 将宝箱放在chestTrans的位置
        chest.transform.position = chestTrans.position;
        chest.transform.rotation = chestTrans.rotation;
    }
    public override void SetResultOn()
    {
        ShowChest();
    }
    public override void SetResultOff()
    {
        HideChest();
    }
    private void ShowChest()
    {
        // 显示宝箱的逻辑
        chest.SetActive(true);
    }

    private void HideChest()
    {
        // 隐藏宝箱的逻辑
    }
}
