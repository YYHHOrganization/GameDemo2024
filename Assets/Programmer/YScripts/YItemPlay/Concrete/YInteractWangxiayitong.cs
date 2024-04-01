using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class YInteractWangxiayitong : YIInteractiveGroup
{
    public GameObject chest;
    
    Animator animator;
    public void Start()
    {
        base.Start();
        
        chest = gameObject;
        //animator = chest.GetComponentInChildren<Animator>();
    }
    
    public override void SetResultOn()
    {
        Debug.Log("宝箱开启");
        //animator.SetBool("isOpen", true);

        PlayableDirector director = chest.GetComponent<PlayableDirector>();
        director.Play();
        
        YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, triggers[0],null);//如果是靠近宝箱打开 只会有一个trigger
        HOpenWorldTreasure tmp = chest.GetComponent<HOpenWorldTreasure>();
        if (tmp)
        {
            tmp.GiveOutAllTreasuresAndDestroy();
        }
        
    }
    public override void SetResultOff()
    {
        
    }
    
    public override void EnterField(bool isEnter, GameObject TriggergameObject,Transform showUIPlace)
    {
        base.EnterField(isEnter, TriggergameObject,showUIPlace);
        // if (isEnter)
        // {
        //     animator.SetBool("isEnterField", true);
        //     Debug.Log("进入宝箱区域");
        // }
        // else
        // {
        //     animator.SetBool("isEnterField", false);
        //     Debug.Log("离开宝箱区域");
        // }
    
    }
    
}
