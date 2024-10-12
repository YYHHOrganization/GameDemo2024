using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class YRogueEnemyFly : HRogueEnemyPatrolAI
{
    // InitStateMachine()
    protected override void InitStateMachine()
    {
        var states = new Dictionary<Type, HRogueEnemyBaseState>
        {
            {typeof(YRogueEnemyFlyState), new YRogueEnemyFlyState(this) },
            {typeof(YRogueEnemyThrowState), new YRogueEnemyThrowState(this)},
            {typeof(HRogueEnemyCommonBeFrozenState), new HRogueEnemyCommonBeFrozenState(this)}
        };
        GetComponent<HRogueEnemyCommonStateMachine>().SetStates(states);
    }

    public override void Fly()
    {
        //将自身的位置设置到飞行高度
        transform.position = new Vector3(transform.position.x, flyheight, transform.position.z);
        StartCoroutine(FlyCoroutine());
    }
    
    float flyheight = 2f;

    IEnumerator FlyCoroutine()
    {
        Debug.Log("EnemyMoveRandomly");
        while (true)
        {
            float moveTime = Random.Range(0, wanderMaxMoveTime);
            float moveDistance = enemy._RogueEnemyWanderSpeed() * moveTime;
            int count = 0; //开一个计数器，防止怪物卡死
            while (isMoving)
            {
                Vector3 moveDirection = new Vector3(Random.Range(-1f, 1f)* moveDistance,  0, Random.Range(-1f, 1f)*moveDistance) ;
                Vector3 destination = transform.position + moveDirection;
                //怪物要转向前进的目标
                transform.LookAt(destination);
                animator.SetBool(isWalkingHash, true);
                
                while (transform.position != destination && isMoving)
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * enemy._RogueEnemyWanderSpeed());
                    count++;
                    if (count == 100)
                    {
                        count = 0;
                        isMoving = false;
                    }
                    yield return null;
                }
                isMoving = false;
            }

            if (!isMoving)
            {
                animator.SetBool(isWalkingHash, false);
                float stopTime = Random.Range(0, wanderMaxStopTime);
                yield return new WaitForSeconds(stopTime);
                isMoving = true;
            }
        }
    }

    protected override void DieShowEff()
    {
        transform.DOScale(0.01f, 1f).SetEase(Ease.InExpo).onComplete = () =>
        {
            Destroy(this.gameObject, 1f);
        };
        //往相反方向飞出去
        //开启重力
        GetComponent<Rigidbody>().useGravity = true;
        
    }
}
