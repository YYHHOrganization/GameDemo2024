using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HEnemyBulletMoveBase : MonoBehaviour
{
    private float bulletSpeed = 5f;
    public int bulletDamage = 1;
    public float bulletRange = 10f;
    private Vector3 originPos;
    public GameObject hitPrefab;
    void Start()
    {
        originPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Destroy(gameObject, 10f);
    }

    public void SetBulletAttribute(float speed, int damage, float range)
    {
        bulletSpeed = speed;
        bulletDamage = damage;
        bulletRange = range;
    }

    void Update()
    {
        if (bulletSpeed != 0)
        {
            if (hasShootTarget && shootTarget != null)
            {
                if (!isChasingYAxis)
                {
                    transform.LookAt(new Vector3(shootTarget.position.x,0.5f, shootTarget.position.z));
                }
                else
                {
                    transform.LookAt(new Vector3(shootTarget.position.x,shootTarget.position.y + shootTarget.localScale.y * 0.5f, shootTarget.position.z));
                }
                transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
            }
            else
            {
                transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
            }
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

    private Transform shootTarget;
    private bool hasShootTarget = false;
    private bool isChasingYAxis = false;
    public void SetTarget(Transform target, bool chasingY = false)
    {
        hasShootTarget = true;
        shootTarget = target;
        isChasingYAxis = chasingY;
    }

    protected void OnCollisionEnter (Collision co)
    {
        bulletSpeed = 0;

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
        else if(Tag == "Player")  //子弹打到了Player，给Player传递伤害
        {
            //bulletDamage 是伤害，要把伤害传递给角色
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-bulletDamage);
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
