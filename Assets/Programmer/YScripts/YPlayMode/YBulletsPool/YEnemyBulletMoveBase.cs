
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Profiling;

public class YEnemyBulletMoveBase  : MonoBehaviour
{
    private float bulletSpeed = 5f;
    private float oribulletSpeed = 5f;
    public int bulletDamage = 1;
    public float bulletRange = 10f;
    private Vector3 originPos;
    public GameObject hitPrefab;
    public string hitEffFromPoolid;

    private bool isActived = false;
    public Rigidbody rb;
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        
        return;
        originPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        oribulletSpeed = bulletSpeed;
        
        // Destroy(gameObject, 10f);
    }
    //当set active true时
    //停用 GameObject 将禁用每个组件，包括附加的渲染器、碰撞体、刚体和脚本。
    //例如，Unity 将不再调用附加到已停用 GameObject 的脚本的 Update() 方法。
    //当 GameObject 收到 SetActive(true) 或 SetActive(false) 时，将调用 OnEnable 或 /OnDisable/。
    private void OnEnable()
    {
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        
        bulletSpeed = oribulletSpeed;isActived = true;
        originPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        isActived = true;
        
        DOVirtual.DelayedCall(10f, () => //保证10秒后自动回收
        {
            gameObject.SetActive(false);
        });
        return;
        originPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.identity; // 添加这行代码来重置旋转
        
        isActived = true;
    }
    void OnDisable()
    {
        isActived = false;
        rb.isKinematic = true;
    }

    public void SetBulletAttribute(float speed, int damage, float range)
    {
        bulletSpeed = speed;
        oribulletSpeed = speed;
        bulletDamage = damage;
        bulletRange = range;
    }

    void Update()
    {
        if (isActived&&bulletSpeed != 0)
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
        // gameObject.SetActive(false);isActived = false;
        // return;
        if(!isActived)
        {
            return;
        }
        isActived = false;
        
        bulletSpeed = 0;

        ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);//这句话是：将一个向量从up旋转到contact.normal
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
            // var hitVFX = Instantiate(hitPrefab, pos, rot);
            GameObject hitVFX = YObjectPool._Instance.Spawn(hitEffFromPoolid);
            hitVFX.transform.position = pos;
            hitVFX.transform.rotation = rot;
            hitVFX.SetActive(true);
            
            //改成从对象池中读取
            
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null) 
            {
                //Destroy(hitVFX, psHit.main.duration);
                //应该是回收，而不是销毁 则让他psHit.main.duratio
                DOVirtual.DelayedCall(psHit.main.duration, () =>
                {
                    hitVFX.SetActive(false);
                });          
                
                
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                // Destroy(hitVFX, psChild.main.duration);
                DOVirtual.DelayedCall(psChild.main.duration, () =>
                {
                    hitVFX.SetActive(false);
                });
            }
        }
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
