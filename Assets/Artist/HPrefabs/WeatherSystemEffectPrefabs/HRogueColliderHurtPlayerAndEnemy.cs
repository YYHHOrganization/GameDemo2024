using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using DG.Tweening;
using UnityEngine;

public class HRogueColliderHurtPlayerAndEnemy : MonoBehaviour
{
    public bool isTrigger = true;
    public int damage = 2;
    public ElementType hurtElement = ElementType.None;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter!!!");
        if (!isTrigger) return;
        if (other.gameObject.CompareTag("Player"))
        {
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-damage);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
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
