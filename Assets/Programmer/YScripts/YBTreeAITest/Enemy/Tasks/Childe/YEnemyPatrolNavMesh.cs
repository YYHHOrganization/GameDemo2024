
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Core.AI;
using UnityEngine;


public class YEnemyPatrolNavMesh : YBTEnemyAction
{
    public float moveSpeed = 3f;
    public float minDistance = 2f;
    public SharedString runAnimation;
    
    public override void OnStart()
    {
        // rb.isKinematic = true;
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
        // var playerDir = (player.transform.position - transform.position).normalized;
        //
        // if(Vector3.Distance(player.transform.position,transform.position) < minDistance)//如果玩家和敌人的距离小于最小距离
        // {
        //     rb.velocity = Vector3.zero;
        // }
        // else
        // {
        //     rb.velocity = playerDir * moveSpeed;
        // }
        //

        
        //使用navmesh
        
        enemyBT.mNavMeshAgent.enabled = true;
        enemyBT.mNavMeshAgent.destination = player.transform.position;
        enemyBT.mNavMeshAgent.speed = moveSpeed;
        
        if(Vector3.Distance(player.transform.position,transform.position) < minDistance)//如果玩家和敌人的距离小于最小距离
        {
            enemyBT.mNavMeshAgent.isStopped = true;
        }
        else
        {
            enemyBT.mNavMeshAgent.isStopped = false;
        }
        
        return TaskStatus.Running;
    }
    public override void OnEnd()
    {
        // rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        animator.SetBool(runAnimation.Value,false);
        enemyBT.mNavMeshAgent.enabled = false;
    }
}
