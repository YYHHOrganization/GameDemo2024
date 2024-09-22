using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogMgr : MonoBehaviour
{
    private string npcName;
    private string npcId;
    private Animator animator;
    
    public void SetNpcBaseInfo(string name, string id)
    {
        npcName = name;
        npcId = id;
        animator = gameObject.GetComponent<Animator>();
    }

    public void ChangeNPCAnimation(string animName)
    {
        if (animator)
        {
            animator.SetTrigger("is" + animName);
        }
    }
}
