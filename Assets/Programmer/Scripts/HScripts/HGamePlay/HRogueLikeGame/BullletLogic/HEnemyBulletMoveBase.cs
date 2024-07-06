using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.VFX;

public class HEnemyBulletMoveBase : MonoBehaviour
{
    protected float bulletSpeed = 5f;
    public int bulletDamage = 1;
    public float bulletRange = 10f;
    protected Vector3 originPos;
    public GameObject hitPrefab;
    [SerializeField]protected bool isFriendSide = false;
    [SerializeField]protected bool isBothSide = false;
    private bool bulletMoving = true;
    public bool BulletMoving
    {
        get { return bulletMoving; }
    }
    public virtual void SetBulletMoving(bool moving)
    {
        bulletMoving = moving;
    }
    protected virtual void Start()
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
    
    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    protected virtual void BulletMoveLogic()
    {
        if (!bulletMoving) return;
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

    void Update()
    {
        BulletMoveLogic();
    }

    protected Transform shootTarget;
    protected bool hasShootTarget = false;
    protected bool isChasingYAxis = false;
    public void SetTarget(Transform target, bool chasingY = false)
    {
        hasShootTarget = true;
        shootTarget = target;
        isChasingYAxis = chasingY;
    }

    protected virtual void CollisionEnterLogic(Collision co)
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
        else if ((isFriendSide || isBothSide) && Tag == "Enemy") //子弹打到了敌人，给敌人传递伤害
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
        else if(Tag == "Player")  //子弹打到了Player，给Player传递伤害
        {
            if (isBothSide || !isFriendSide)
            {
                //bulletDamage 是伤害，要把伤害传递给角色
                HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-bulletDamage);
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
                if (hitVFX.GetComponent<VisualEffect>())
                {
                    hitVFX.GetComponent<VisualEffect>().Play();
                    Destroy(hitVFX, 5f);
                }
                else
                {
                    var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, psChild.main.duration);
                }
            }
        }
        DealWithBulletAfterCollision(gameObject);
    }
    
    protected void OnCollisionEnter (Collision co)
    {
        CollisionEnterLogic(co);
    }

    protected virtual void DealWithBulletAfterCollision(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
