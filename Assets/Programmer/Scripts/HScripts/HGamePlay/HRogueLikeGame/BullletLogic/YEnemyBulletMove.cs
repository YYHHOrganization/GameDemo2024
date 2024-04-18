using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YEnemyBulletMove :HEnemyBulletMoveBase
{
    
    // Update is called once per frame
    void Update()
    {
        //
    }
    // protected void OnCollisionEnter (Collision co)
    // {
    //     base.OnCollisionEnter(co);
    // }
    protected void OnTriggerEnter(Collider co)
    {
        // bulletSpeed = 0;

        // ContactPoint contact = co.ClosestPointOnBounds(transform.position);
        // Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        // Vector3 pos = contact.point;
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
                // fractureExplosionObject.TriggerExplosion(contact.point);
                fractureExplosionObject.TriggerExplosion(transform.position);
            }
            
        }
        else if(Tag == "Player")  //子弹打到了Player，给Player传递伤害
        {
            //bulletDamage 是伤害，要把伤害传递给角色
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-bulletDamage);
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
        
    }
}
