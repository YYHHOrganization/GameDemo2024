using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SundayBossSkillTest : MonoBehaviour
{
    public GameObject bossClock; //暂时用一个钟表代替，这个钟表时间会动态变化
    private Transform clockShizhen;
    private Transform clockFenzhen;
    private Transform player;

    private void Start()
    {
        clockShizhen = bossClock.transform.Find("Zhizhen/shizhen");
        clockFenzhen = bossClock.transform.Find("Zhizhen/fenzhen");
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
        clockShizhen.DOLocalRotate(new Vector3(0, 0, 360), 24f, RotateMode.FastBeyond360);
        clockFenzhen.DOLocalRotate(new Vector3(0, 0, 24 * 360), 24f, RotateMode.FastBeyond360);
        
        //开始放一些Muzzle的技能
        for (int i = 0; i < 4; i++)
        {
            int randomSkill = UnityEngine.Random.Range(1, 3);
            switch (randomSkill)
            {
                case 1:
                    SundaySkill1();
                    break;
                case 2:
                    SundaySkill2();
                    break;
                case 3:
                    SundaySkill3();
                    break;
            }
            yield return new WaitForSeconds(6f);
        }
    }

    private void SundaySkill1()
    {
        
    }
    
    private void SundaySkill2()
    {
        
    }

    private void SundaySkill3()
    {
        
    }

    private void Update()
    {
        if (testStartSunday)
        {
            StartSundayBossSkill();
            testStartSunday = false;
        }
    }
}
