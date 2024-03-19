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
    }
    public override void SetResultOff()
    {
        
    }
    
}
