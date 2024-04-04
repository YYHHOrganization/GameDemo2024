using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HShajinPlayerSkill : HCharacterSkillBase
{
    //砂金的技能
    private void Start()
    {
        skillVFXPath = "HVFX_ShajinShield";
        skillCDTime = 2.0f;
        skillLastTime = 8.0f;
        skillLockTime = 3.0f;
        countDownUIlink = "Prefabs/UI/singleUnit/Skill1CountDownPanel";
        countDownUI = Instantiate(Resources.Load<GameObject>(countDownUIlink), GameObject.Find("Canvas").transform);
        countDownUI.gameObject.SetActive(false);
        countDownText = countDownUI.GetComponentInChildren<TMP_Text>();
        countDownImage = countDownUI.transform.Find("skillIcon").GetComponent<Image>();
        LoadVFXEffect();
    }
    
    public override void PlaySkill1()
    {
        StartCoroutine(WaitForAnimationAndSummonShield());
    }
    
    

    IEnumerator WaitForAnimationAndSummonShield()
    {
        yield return new WaitForSeconds(1.5f);
        base.PlaySkillOn();
        HAudioManager.Instance.Play("ShieldSummonAudio", this.gameObject);
    }
}
