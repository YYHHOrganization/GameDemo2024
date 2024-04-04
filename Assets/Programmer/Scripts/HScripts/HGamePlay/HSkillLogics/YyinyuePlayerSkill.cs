using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class YyinyuePlayerSkill : HCharacterSkillBase
{
    
    //砂金的技能
    private void Start()
    {
        skillVFXPath = "YVFX_yinyueLaser2";
        skillCDTime = 2.0f;
        skillLastTime = 2.0f;
        delayEffTime = 0.6f;
        skillLockTime = skillLastTime + delayEffTime;
        countDownUIlink = "Prefabs/UI/singleUnit/Skill1CountDownPanel";
        countDownUI = Instantiate(Resources.Load<GameObject>(countDownUIlink), GameObject.Find("Canvas").transform);
        countDownUI.gameObject.SetActive(false);
        countDownText = countDownUI.GetComponentInChildren<TMP_Text>();
        countDownImage = countDownUI.transform.Find("skillIcon").GetComponent<Image>();

        clipStringPath = "Prefabs/YAnimation/MaleCrouchPose2S";
        //LoadVFXEffect();
        LoadVFXEffect(withRotation:true);
        LoadAnimationClip();
    }
    
    public override void PlaySkill1()
    {
        StartCoroutine(WaitForAnimationAndLaser());
        
    }

    IEnumerator WaitForAnimationAndLaser()
    {
        yield return new WaitForSeconds(delayEffTime);
        base.PlaySkillOn();
        HAudioManager.Instance.Play("ShieldSummonAudio", this.gameObject);
    }

    

}
