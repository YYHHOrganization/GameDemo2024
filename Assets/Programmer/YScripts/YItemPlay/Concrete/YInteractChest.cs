using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YInteractChest : YIInteractiveGroup
{
    public GameObject chest;
    Animator animator;
    public void Start()
    {
        base.Start();
        
        chest = gameObject;
        animator = chest.GetComponentInChildren<Animator>();
    }

    
    public override void SetResultOn()
    {
        Debug.Log("宝箱开启");
        animator.SetBool("isOpen", true);
        YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, triggers[0]);//如果是靠近宝箱打开 只会有一个trigger
        HOpenWorldTreasure tmp = chest.GetComponent<HOpenWorldTreasure>();
        if (tmp)
        {
            tmp.GiveOutAllTreasuresAndDestroy();
        }
        
    }
    public override void SetResultOff()
    {
        
    }
    
    public override void EnterField(bool isEnter, GameObject TriggergameObject)
    {
        base.EnterField(isEnter, TriggergameObject);
        if (isEnter)
        {
            animator.SetBool("isEnterField", true);
            Debug.Log("进入宝箱区域");
        }
        else
        {
            animator.SetBool("isEnterField", false);
            Debug.Log("离开宝箱区域");
        }
    
    }
    
}
