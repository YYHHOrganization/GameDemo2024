using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class HRogueColliderHurtPlayerAndEnemy : MonoBehaviour
{
    public bool isTrigger = true;
    public int damage = 2;
    public ElementType hurtElement = ElementType.None;
    
    public bool detectInCD = false; //是否有cd检测时间
    public float detectCDTime = 0.8f; //cd时间

    private bool isInCD = false;
    private void OnTriggerStay(Collider other)
    {
        if (!detectInCD) return;
        if (!isTrigger) return;
        if (other.gameObject.CompareTag("Player"))
        {
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-damage);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("OnTriggerStay!!!" + other.name);
            //if (detectInCD)
            //{
                if (isInCD) return;
                isInCD = true;
                DOVirtual.DelayedCall(detectCDTime, () =>
                {
                    isInCD = false;
                });
            //}
            
            YPatrolAI patrolAI = other.gameObject.GetComponentInParent<YPatrolAI>();
            if (patrolAI != null)
            {
                patrolAI.die();
                return;//这里不需要传递伤害，因为敌人已经死了，特指蜘蛛怪
                //todo:写一个伤害的函数
            }
            HRogueEnemyPatrolAI enemyPatrolAI = other.gameObject.GetComponentInParent<HRogueEnemyPatrolAI>();
            if (enemyPatrolAI == null)
            {
                enemyPatrolAI = other.gameObject.GetComponent<HRogueEnemyPatrolAI>();
            }
            if (enemyPatrolAI != null)
            {
                enemyPatrolAI.UpdateEnemyCurrentElement(hurtElement);
                int finalDamage =
                    HRogueDamageCalculator.Instance.CalculateBaseDamage(damage, hurtElement, enemyPatrolAI.EnemyElementType,
                        out ElementReaction reaction);
                enemyPatrolAI.ChangeHealthWithReaction(-finalDamage, reaction);
                enemyPatrolAI.AddElementReactionEffects(reaction);
            }
        }
        //处理猫猫糕的问题
        else if (other.gameObject.GetComponent<YPetStateMachine>())
        {
            Debug.Log("猫猫糕炸了Trigger！");
            //todo:后面有需要的话做一个猫猫糕被炸飞的效果，暂时先不管了
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTrigger) return;
        if (other.gameObject.CompareTag("Player"))
        {
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-damage);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("OnTriggerEnter!!!"+ other.name);
            Debug.Log("男人！！！");
            YPatrolAI patrolAI = other.gameObject.GetComponentInParent<YPatrolAI>();
            if (patrolAI != null)
            {
                patrolAI.die();
                return;//这里不需要传递伤害，因为敌人已经死了，特指蜘蛛怪
                //todo:写一个伤害的函数
            }
            HRogueEnemyPatrolAI enemyPatrolAI = other.gameObject.GetComponentInParent<HRogueEnemyPatrolAI>();
            if (enemyPatrolAI == null)
            {
                enemyPatrolAI = other.gameObject.GetComponent<HRogueEnemyPatrolAI>();
            }
            if (enemyPatrolAI != null)
            {
                enemyPatrolAI.UpdateEnemyCurrentElement(hurtElement);
                int finalDamage =
                    HRogueDamageCalculator.Instance.CalculateBaseDamage(damage, hurtElement, enemyPatrolAI.EnemyElementType,
                        out ElementReaction reaction);
                enemyPatrolAI.ChangeHealthWithReaction(-finalDamage, reaction);
                enemyPatrolAI.AddElementReactionEffects(reaction);
            }
        }
        //处理猫猫糕的问题
        else if (other.gameObject.GetComponent<YPetStateMachine>())
        {
            Debug.Log("猫猫糕炸了Trigger！");
            //todo:后面有需要的话做一个猫猫糕被炸飞的效果，暂时先不管了
            
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
