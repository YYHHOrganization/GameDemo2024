using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YInteract2AndChest :YIInteractiveGroup
{
    public GameObject chest;
    
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
