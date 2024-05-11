using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using DG.Tweening;
using UnityEngine;

public class HBulletMuzzleUtility : MonoBehaviour
{
    // 这个脚本用来写各种各样的子弹类型，支持玩家和敌人的
    // 存储一下子弹的基本属性
    private float bulletSpeed = 5f;
    private int bulletDamage = 1;
    private float bulletRange = 10f;
    private bool isEnemy = true;
    private string muzzleKind;
    private GameObject bulletPrefab;
    private Transform mTarget;
    private Class_RogueEnemyCSVFile enemy;
    
    public void SetInitializeAttribute(GameObject bulletPrefab, string muzzleKind, bool isEnemy, string curStateName, Class_RogueEnemyCSVFile enemy = null, Transform mTarget = null)
    {
        this.isEnemy = isEnemy;
        this.muzzleKind = muzzleKind;
        this.bulletPrefab = bulletPrefab;
        this.mTarget = mTarget;
        this.enemy = enemy;
        if (isEnemy)
        {
            SetEnemyBulletBaseAttribute(enemy, curStateName);
        }
        else
        {
            // 玩家的子弹属性 , 这个后面参考一下玩家的Shoot脚本里写的函数
        }
    }
    
    private void SetEnemyBulletBaseAttribute(Class_RogueEnemyCSVFile enemy, string curStateName)
    {
        string bulletAttribute = enemy.EnemyBulletAttribute;
        string[] attributes = bulletAttribute.Split(';');
        bulletSpeed = float.Parse(attributes[0]);
        bulletRange = float.Parse(attributes[1]);
        if(curStateName == "wander")
            bulletDamage = enemy._EnemyWanderDamage();
        else if(curStateName == "chase")
            bulletDamage = enemy._EnemyChaseDamage();
    }

    private void EnemyShoot()
    {
        // 这里写敌人的射击逻辑，不同的Muzzle类型会有不同的逻辑
        string[] muzzleKindParams = muzzleKind.Split(';');
        int bulletCnt = 1;
        bool chasePlayer = false;
        if (muzzleKindParams.Length >= 2)
        {
            bulletCnt = int.Parse(muzzleKindParams[1]);
            chasePlayer = bool.Parse(muzzleKindParams[2]);
        }
        switch (muzzleKindParams[0])
        {
            case "ShootBulletMuzzleCircleSlowlyZigzag":
                // 这里写慢速但是来回蠕动的一圈子弹
                StartCoroutine(ShootBulletMuzzleCircleSlowlyZigzag(bulletCnt));
                break;
            case "ShootBulletMuzzleCircleInterval":
                // 这里写环形子弹的逻辑
                StartCoroutine(ShootBulletMuzzleCircleInterval());
                break;
            case "ShootBulletMuzzleBumpingWithCnt":
                ShootFollowingBulletMuzzleBumping(bulletCnt, chasePlayer);
                break;
            case "ShootBulletMuzzleCircleBumping":
                // 环形子弹，具有弹跳效果
                ShootBulletMuzzleCircleBumping();
                break;
            case "ShootBulletMuzzleBezier":
                StartCoroutine(ShootBulletMuzzleBezier(bulletCnt, chasePlayer));
                break;
            case "LaserRotateAround":
                StartCoroutine(LaserRotateAround(bulletCnt));
                break;
                
        }
    }
    
    IEnumerator LaserRotateAround(int laserCnt)
    {
        List<GameObject> lasers = new List<GameObject>();
        transform.parent = null;  //不然会跟着父节点一起旋转
        // 先生成laserCnt个激光作为transform的子节点，然后过2s后muzzle开始缓慢旋转
        for(int i = 0; i<laserCnt; i++)
        {
            int angle = 360 / laserCnt * i;
            GameObject laser = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, angle, 0), transform);
            lasers.Add(laser);
        }
        yield return new WaitForSeconds(2f);
        //muzzle开始缓慢旋转
        for (int j = 0; j < 4; j++)
        {
            transform.DOLocalRotate(new Vector3(0, 90, 0), 4f, RotateMode.LocalAxisAdd);
            yield return new WaitForSeconds(4.1f);
        }
        
        Destroy(this.gameObject, 17f);
    }

    IEnumerator ShootBulletMuzzleBezier(int bulletCnt, bool chasePlayer)
    {
        List<GameObject> bullets = new List<GameObject>();
        //生成16颗子弹的子弹墙，然后每颗子弹以贝塞尔曲线移动
        for (int i = 0; i < bulletCnt; i++)
        {
            Vector3 shootDirection = new Vector3(0.2f,Mathf.Sin(i * 22.5f * Mathf.Deg2Rad), Mathf.Cos(i * 22.5f * Mathf.Deg2Rad));
            GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 2 + new Vector3(0, 2f,0) , Quaternion.Euler(0, i * 22.5f, 0), transform);
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletMoving(false);
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletAttribute(bulletSpeed, bulletDamage, bulletRange);
            if (chasePlayer)
            {
                bullet.GetComponent<HEnemyBulletMoveBase>().SetTarget(mTarget);
            }
            bullets.Add(bullet);
        }
        yield return new WaitForSeconds(2f);
        foreach (var bullet in bullets)
        {
            if (bullet)
            {
                bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletMoving(true);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void ShootFollowingBulletMuzzleBumping(int bulletCnt, bool chasePlayer)
    {
        //Debug.Log("ShootBulletMuzzleBumpingWithCnt");
        //等距发射bulletCnt颗子弹，每颗子弹都会弹跳
        for (int i = 0; i < bulletCnt; i++)
        {
            int angle = 20 * i;
            Vector3 shootDirection = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0.2f, Mathf.Cos(angle * Mathf.Deg2Rad));
            GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, i, 0));
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletAttribute(bulletSpeed, bulletDamage, bulletRange);
            if (chasePlayer)
            {
                bullet.GetComponent<HEnemyBulletMoveBase>().SetTarget(mTarget);
            }
        }
    }
    
    private void ShootBulletMuzzleCircleBumping()
    {
        for (int i = 0; i < 360; i += 20)
        {
            Vector3 shootDirection = new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), 0.2f, Mathf.Cos(i * Mathf.Deg2Rad));
            GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, i, 0));
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletAttribute(bulletSpeed, bulletDamage, bulletRange);
        }
    }

    IEnumerator ShootBulletMuzzleCircleInterval()
    {
        // 环形发射子弹，每过0.1f发射一颗
        for (int i = 0; i < 360; i += 15)
        {
            Vector3 shootDirection = new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), 0.2f, Mathf.Cos(i * Mathf.Deg2Rad));
            GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, i, 0));
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletAttribute(bulletSpeed, bulletDamage, bulletRange);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    IEnumerator ShootBulletMuzzleCircleSlowlyZigzag(int bulletCnt)
    {
        //慢速但是来回蠕动的一圈子弹
        yield return null;
    }

    public void Shoot()
    {
        if (isEnemy)
        {
            EnemyShoot();
        }
    }
    
}
