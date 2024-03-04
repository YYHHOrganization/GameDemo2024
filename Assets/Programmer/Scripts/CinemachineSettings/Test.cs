using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test: MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform targetPos;
    private bool shouldNav = false;
    public AnimationClip clip;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void OnValueChange(int index)
    {
        Debug.Log(index);
    }
    
    public void SetNavAgent(bool isOn)
    {
        shouldNav = isOn;
        Animator animator = transform.GetComponent<Animator>();
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        Debug.Log(animatorOverrideController);
        animatorOverrideController["Walking"] = clip;
        animator.runtimeAnimatorController = animatorOverrideController;
        animator.SetBool("isActing", true);
    }

    private void Update()
    {
        if(shouldNav)
            MoveToDestination(targetPos);
    }

    //利用NavMesh移动到目的地
    public void MoveToDestination(Transform pos)
    {
        //print("should move To target");
        agent.SetDestination(pos.position);
    }
    

}
