using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using UnityEngine;

public class YPetWeapon : MonoBehaviour
{
    public BoxCollider boxCollider;
    [SerializeField]int bulletDamage = 1;
    [SerializeField] private GameObject hitEff;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    //每攻击一次 检测一次碰撞
    public void SetDetectShootOn()
    {
        boxCollider.enabled = true;
    }
    public void SetDetectShootOff()
    {
        boxCollider.enabled = false;
        if(hitEff != null)
        {
            hitEff.SetActive(true);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                hitEff.SetActive(false);
            });  
        }
    }
    protected void OnCollisionEnter (Collision co)
    {
        ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        //Debug.Log("Bullet Hit: " + co.gameObject.name);
        
        string Tag = co.gameObject.tag;
        if(Tag == "CouldBroken")
        {
            Debug.Log("CouldBroken: ");
            //YFractureExplosionObject fractureExplosionObject = hitObject.GetComponent<YFractureExplosionObject>();
            YFractureExplosionObject fractureExplosionObject 
                = co.gameObject.GetComponentInParent<YFractureExplosionObject>();
            if (fractureExplosionObject != null)
            {
                fractureExplosionObject.TriggerExplosion(contact.point);
            }
        }
        else if (Tag == "Enemy") //子弹打到了敌人，给敌人传递伤害
        {
            //hitObject.GetComponent<YHandleHitPuppet>().HandleHitPuppet();
            YPatrolAI patrolAI = co.gameObject.GetComponentInParent<YPatrolAI>();
            if (patrolAI != null)
            {
                patrolAI.die();
                return;//这里不需要传递伤害，因为敌人已经死了，特指蜘蛛怪
                //todo:写一个伤害的函数
            }
            HRogueEnemyPatrolAI enemyPatrolAI = co.gameObject.GetComponentInParent<HRogueEnemyPatrolAI>();
            if (enemyPatrolAI == null)
            {
                enemyPatrolAI = co.gameObject.GetComponent<HRogueEnemyPatrolAI>();
            }
            if (enemyPatrolAI != null)
            {
                
                enemyPatrolAI.ChangeHealth(-bulletDamage);
            }
        }
        
        // if(hitPrefab != null)
        // {
        //     var hitVFX = Instantiate(hitPrefab, pos, rot);
        //     var psHit = hitVFX.GetComponent<ParticleSystem>();
        //     if (psHit != null) 
        //     {
        //         Destroy(hitVFX, psHit.main.duration);
        //     }
        //     else
        //     {
        //         var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
        //         Destroy(hitVFX, psChild.main.duration);
        //     }
        // }
        
        //检测一次过后就停止检测
        // boxCollider.enabled = false;
        
    }
}
