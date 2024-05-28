using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YFriendBulletMuzzleUtility : HBulletMuzzleUtility
{
    private Class_RoguePetCSVFile pet;
    private string petID;
    //构造函数：
    public YFriendBulletMuzzleUtility(string petID)
    {
        this.petID = petID;
        ReadTableAndSetAttribute();
    }
    private void ReadTableAndSetAttribute()
    {
        
    }
    public void SetInitializeAttribute(GameObject bulletPrefab, string muzzleKind, string curStateName, Class_RoguePetCSVFile roguePetCsvFile, Transform mTarget = null)
    {
        this.isEnemy = isEnemy;
        this.muzzleKind = muzzleKind;
        this.bulletPrefab = bulletPrefab;
        this.mTarget = mTarget;
        this.pet = roguePetCsvFile;
        SetBulletBaseAttribute(pet, curStateName);
    }
    private void SetBulletBaseAttribute(Class_RoguePetCSVFile enemy, string curStateName)
    {
        string bulletAttribute = enemy.BulletAttribute;
        string[] attributes = bulletAttribute.Split(';');
        bulletSpeed = float.Parse(attributes[0]);
        bulletRange = float.Parse(attributes[1]);
        bulletDamage = enemy._WanderDamage();
        // if(curStateName == "RangedAttack")
        //     bulletDamage = enemy._WanderDamage();
        if (curStateName == "MeleeAttack") //如果是近战
            InitMeleeWeapon();
    }

    private GameObject weapon;
    private bool isMelee;
    void InitMeleeWeapon()
    {
        isMelee = true;
        weapon =  Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.Euler(-23, 0, -6), transform);
        weapon.SetActive(false);
    }
    bool duringShoot = false;
    Coroutine shootCoroutine;
    public void ShootSpecialBullet()
    {
        if(isMelee) weapon.SetActive(true);
        duringShoot = true;
        shootCoroutine=StartCoroutine(ShootSpecialBulletWithMuzzle());
    }

    public void ShootOff()
    {
        if(isMelee) weapon.SetActive(false);
        
        duringShoot = false;
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
    }
    public IEnumerator ShootSpecialBulletWithMuzzle()
    {
        // 只负责生成Muzzle，至于Muzzle的逻辑写在Muzzle挂的脚本上
        while(duringShoot)
        {
            PetShoot();
            yield return new WaitForSeconds(pet._WanderShootInterval());
        }
    }
    
    protected void PetShoot()
    {
        //后面这个分割放到start里面就好
        // 这里写敌人的射击逻辑，不同的Muzzle类型会有不同的逻辑
        string[] muzzleKindParams = muzzleKind.Split(';');  //shootCircle;10;true
        int bulletCnt = 1;
        bool chasePlayer = false;
        if (muzzleKindParams.Length >= 2)
        {
            bulletCnt = int.Parse(muzzleKindParams[1]);
            chasePlayer = bool.Parse(muzzleKindParams[2]);
        }
        switch (muzzleKindParams[0])
        {
            case "ShootBulletSpecialModal1":
                StartCoroutine(ShootBulletSpecialModal1(bulletCnt));
                break;
            case "ShootBulletMuzzleCircleInterval":
                // 这里写环形子弹的逻辑
                StartCoroutine(ShootBulletMuzzleCircleInterval());
                break;
            case "ShootBulletMuzzleCircleAtOnce":
                ShootBulletMuzzleCircleAtOnce(bulletCnt);
                break;
            case "ShootBulletMuzzleCircleBumping":
                // 环形子弹，具有弹跳效果
                ShootBulletMuzzleCircleBumping(bulletCnt);
                break;
            case "UseMeleeWeapons"://使用近战武器
                UseMeleeWeapons();
                break;
        }
    }

    private void UseMeleeWeapons()
    {
        ////想在这里使用dotween等实现一个挥舞球棒的动作，例如斜上方挥到斜下方那种
        // 创建一条路径，使其从斜上方挥到斜下方并带有弧线效果
        // 通过定义沿着弧线旋转的路径来实现球棒挥舞的效果

        Vector3 startAngle = new Vector3(-23, 0, -6);  // 球棒起始的旋转角度, 可以根据需要调整
        Vector3 middleAngle = new Vector3(67 ,45, 85); // 球棒中间的旋转角度, 像挥棒的中段
        Vector3 endAngle = new Vector3(-23, 0, -6);   // 球棒结束的旋转角度, 可以根据需要调整
        
        Sequence swingSequence = DOTween.Sequence();

       
        swingSequence.Append(weapon.transform.DOLocalRotate(startAngle, 0f));
        swingSequence.Append(weapon.transform.DOLocalRotate(middleAngle, 0.2f).SetEase(Ease.OutQuad)); 
        swingSequence.Append(weapon.transform.DOLocalRotate(endAngle, 0.4f).SetEase(Ease.InQuad)); 
        // 你可以根据需要调整时间 (0.5秒) 和缓动类型 (Ease.OutQuad 和 Ease.InQuad)
    }

    protected void ShootBulletMuzzleCircleBumping(int bulletCnt)
    {
        int angleStep = (int)360 / bulletCnt;
        for (int i = 0; i < 360; i += angleStep)
        {
            Vector3 shootDirection = new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), 0.2f, Mathf.Cos(i * Mathf.Deg2Rad));
            GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, i, 0));
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletAttribute(bulletSpeed, bulletDamage, bulletRange);
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletMoving(true);
        }
    }
    IEnumerator ShootBulletSpecialModal1(int bulletCnt)
    {
        // transform.parent = null; //不然会跟着父节点一起旋转
        int offset = 0;
        for (int i = 0; i < bulletCnt; i++)  //一共发射bulletCnt颗子弹
        {
            //以当前transform为原点发射四分之一圆的五颗子弹
            for (int j = 0; j < 12; j++)
            {
                Vector3 shootDirection = new Vector3(Mathf.Sin(j * 30 * Mathf.Deg2Rad), 0.2f, Mathf.Cos(j * 30 * Mathf.Deg2Rad));
                GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 2, Quaternion.Euler(0, j * 90, 0), transform);
                bullet.transform.forward = new Vector3(shootDirection.x, 0, shootDirection.z);
                
                bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletAttribute(bulletSpeed, bulletDamage, bulletRange);
                Debug.Log(bulletSpeed + " " + bulletDamage + " " + bulletRange);
                bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletMoving(true);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void ShootBulletMuzzleCircleAtOnce(int bulletCnt)
    {
        // 同时发出一圈子弹
        float angleStep = 360f / bulletCnt;
        float currentAngle = 0f;
        for (int i = 0; i < bulletCnt; i++)
        {
            // 计算每个子弹的发射方向
            float bulletDirX = Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            float bulletDirZ = Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            Vector3 bulletDirection = new Vector3(bulletDirX, 0, bulletDirZ);
            Vector3 spawnPosition = transform.position + bulletDirection * 2; // Adjust spawn distance if needed
            
            // 实例化子弹
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(bulletDirection, Vector3.up), transform);
            // 设置子弹方向
            bullet.transform.forward = bulletDirection.normalized;
            // 设置子弹属性和移动
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletAttribute(bulletSpeed, bulletDamage, bulletRange);
            bullet.GetComponent<HEnemyBulletMoveBase>().SetBulletMoving(true);
            
            // 更新当前角度
            currentAngle += angleStep;

        }

    }

}
