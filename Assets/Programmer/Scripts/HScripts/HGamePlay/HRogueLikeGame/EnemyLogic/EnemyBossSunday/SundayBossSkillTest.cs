using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SundayBossSkillTest : MonoBehaviour
{
    public GameObject bossClock; //暂时用一个钟表代替，这个钟表时间会动态变化
    private Transform clockShizhen;
    private Transform clockFenzhen;
    private Transform player;
    
    private string bullet1PrefabLink= "BulletCircleMove01Enemy";
    private string bullet2BezierPrefabLink = "BulletBezier01Enemy";
    private GameObject bullet1Prefab;
    private GameObject bullet2Prefab;
    private string bulletFireType1 = "ShootBulletSpecialModal1;6;false";
    
    private void Start()
    {
        clockShizhen = bossClock.transform.Find("Zhizhen/shizhen");
        clockFenzhen = bossClock.transform.Find("Zhizhen/fenzhen");
        player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        bullet1Prefab = Addressables.LoadAssetAsync<GameObject>(bullet1PrefabLink).WaitForCompletion();
        bullet2Prefab = Addressables.LoadAssetAsync<GameObject>(bullet2BezierPrefabLink).WaitForCompletion();
    }

    private void StartSundayBossSkill()
    {
        StartCoroutine(DoSundayBossSkill());
    }

    public bool testStartSunday = false;

    IEnumerator DoSundayBossSkill()
    {
        yield return new WaitForSeconds(4f);
        HMessageShowMgr.Instance.ShowMessage("SUNDAY_BOSS_DAY1");
        yield return Day1Skill();
        
    }

    IEnumerator Day1Skill()
    {
        //第一天的技能，赐以真实
        //钟表指针转动,一共24s的技能，在此期间Shizhen转360度，Fenzhen转24 * 360度
        /*
         * 祂为万物定调，称时空为谱线，立十二等程之律制。
        1:6 实在与想象得以区别，事就这样成了。
        1:7 万物既已有实，有虚。这是头一日。
         */
        clockShizhen.DOLocalRotate(new Vector3(0, 0, 360), 24f, RotateMode.FastBeyond360);
        clockFenzhen.DOLocalRotate(new Vector3(0, 0, 24 * 360), 24f, RotateMode.FastBeyond360);
        
        //开始放一些Muzzle的技能
        for (int i = 0; i < 4; i++)
        {
            int randomSkill = UnityEngine.Random.Range(1, 3);
            randomSkill = 3;
            switch (randomSkill)
            {
                case 1:
                    SundayDay1Skill1();
                    yield return new WaitForSeconds(6f);
                    break;
                case 2:
                    yield return SundayDay1Skill2();
                    break;
                case 3:
                    yield return SundayDay1Skill3();
                    break;
            }
            
        }
    }

    private void SundayDay1Skill1()
    {
        //放一圈子弹,每次一个弧线这种
        GameObject muzzleObj = new GameObject("muzzle");
        muzzleObj.transform.position = transform.position - new Vector3(0, 1,0);
        //一个随机的60度内的旋转角
        muzzleObj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-60, 60), 0);
        muzzleObj.transform.parent = transform;
        HBulletMuzzleUtility muzzleUtility = muzzleObj.AddComponent<HBulletMuzzleUtility>();
        muzzleUtility.SetSelfDefInitializeAttribute(bullet1Prefab, bulletFireType1, true, 1, 10, 6, player.transform);
        muzzleUtility.Shoot();
        Destroy(muzzleObj, 20f);
    }
    
    IEnumerator SundayDay1Skill2()
    {
        //一共是6s的技能，朝玩家发射过来竖着，横着和斜着的刀光
        //每次两道刀光比较有感觉，横竖一次，斜着X形状一次，再横竖一次
        
        yield return null;
    }

    IEnumerator SundayDay1Skill3()
    {
        //天上一个表盘形状的子弹，用Bezier曲线去发射
        GameObject muzzleObj = new GameObject("muzzle");
        muzzleObj.transform.position = transform.position + new Vector3(0, 2,0);
        muzzleObj.transform.rotation = transform.rotation;
        muzzleObj.transform.parent = transform;
        muzzleObj.transform.localScale = new Vector3(3, 3, 3);
        HBulletMuzzleUtility muzzleUtility = muzzleObj.AddComponent<HBulletMuzzleUtility>();
        string bulletFireType = "ShootBulletMuzzleBezier;24;true";
        muzzleUtility.SetSelfDefInitializeAttribute(bullet2Prefab, bulletFireType, true, 1, 10, 8, player.transform);
        muzzleUtility.Shoot();
        Destroy(muzzleObj, 20f);

        yield return new WaitForSeconds(2f);
        bulletFireType = "ShootBulletMuzzleBezier;12;true";
        muzzleUtility.SetSelfDefInitializeAttribute(bullet2Prefab, bulletFireType, true, 1, 10, 8, player.transform);
        muzzleUtility.Shoot();
        yield return new WaitForSeconds(4f);
    }

    private void Update()
    {
        if (testStartSunday)
        {
            StartSundayBossSkill();
            testStartSunday = false;
        }

        RotateToPlayer();
    }

    private void RotateToPlayer()
    {
        Vector3 targetDir = player.position - transform.position;
        float step = 2 * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
