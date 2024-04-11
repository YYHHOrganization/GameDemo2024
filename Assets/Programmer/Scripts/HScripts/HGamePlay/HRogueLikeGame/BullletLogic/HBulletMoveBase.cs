using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HBulletMoveBase : MonoBehaviour
{
    private float speed;
    public GameObject hitPrefab;
    private float bulletRange = 10f;
    private Vector3 originPos;
    void Start()
    {
        originPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        bulletRange = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueShootRange"];
        speed = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueBulletSpeed"];
    }

    void Update()
    {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
            if(Vector3.Distance(originPos, transform.position) >= bulletRange)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    void OnCollisionEnter (Collision co)
    {
        speed = 0;

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
        else if (Tag == "Enemy")
        {
            Debug.Log("Shhhhhhhhhhh");
            //hitObject.GetComponent<YHandleHitPuppet>().HandleHitPuppet();
            YPatrolAI patrolAI = co.gameObject.GetComponentInParent<YPatrolAI>();
            if (patrolAI != null)
            {
                patrolAI.die();
                //todo:写一个伤害的函数
            }

            HRogueEnemyPatrolAI enemyPatrolAI = co.gameObject.GetComponentInParent<HRogueEnemyPatrolAI>();
            if (enemyPatrolAI != null)
            {
                enemyPatrolAI.ChangeHealth(-(int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueCharacterCurDamage"]);
            }
        }

        if(hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null) 
            {
                Destroy(hitVFX, psHit.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }
        Destroy(gameObject);
    }
}
