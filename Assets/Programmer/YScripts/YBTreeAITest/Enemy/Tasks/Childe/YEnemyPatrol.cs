using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Core.AI;
using UnityEngine;


public class YEnemyPatrol : YBTEnemyAction
{
    public float moveSpeed = 2f;
    public float minDistance = 8f;
    public SharedString runAnimation;
    
    public override void OnStart()
    {
        animator.SetBool(runAnimation.Value,true);
        //PLAYERDIR
        // var playerDir = (player.transform.position - transform.position).normalized;
        // if(Vector3.Distance(player.transform.position,transform.position) < minDistance)
        // {
        //     rb.velocity = playerDir * moveSpeed;
        // }
        // else
        // {
        //     rb.velocity = Vector3.zero;
        // }
    }
    public override TaskStatus OnUpdate()
    {
        var playerDir = (player.transform.position - transform.position).normalized;
        
        if(Vector3.Distance(player.transform.position,transform.position) < minDistance)//如果玩家和敌人的距离小于最小距离
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = playerDir * moveSpeed;
        }
        

        return TaskStatus.Running;
    }
    public override void OnEnd()
    {
        rb.velocity = Vector3.zero;
        animator.SetBool(runAnimation.Value,false);
    }
}
