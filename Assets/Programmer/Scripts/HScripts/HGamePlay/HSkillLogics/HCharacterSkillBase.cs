using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class HCharacterSkillBase : MonoBehaviour
{
    //所有角色技能的基类，
    protected GameObject characterSkillVFX;  //角色的特效物品
    protected string skillVFXPath;  //角色特效的路径,Addressable
    protected float skillCDTime;  //技能的冷却时间
    
    protected float skillLastTime;  //技能的持续时间
    private float skillCDTimer;  //技能冷却计时器
    private bool isSkill1CD = false;  //技能1是否在冷却中
    private bool isSkill1Using = false;  //技能1是否正在使用
    private float isSkill1LastTimer;
    
    protected GameObject countDownUI;
    protected string countDownUIlink;
    protected TMP_Text countDownText;
    protected Image countDownImage;
    
    
    private void Start()
    {
        LoadVFXEffect();
        //LoadSkillInfoFromDesignTable();
    }

    public bool isSkill1Valid()
    {
        return !isSkill1CD && !isSkill1Using;
    }
    
    protected virtual void LoadVFXEffect()
    {
        Addressables.InstantiateAsync(skillVFXPath, transform.position, Quaternion.identity, gameObject.transform).Completed += OnLoadVFXEffect;
    }
    
    protected virtual void OnLoadVFXEffect(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            characterSkillVFX = obj.Result;
            characterSkillVFX.gameObject.SetActive(false);
        }
    }

    //第一个技能——小技能
    public virtual void PlaySkill1()
    {
        if (isSkill1Using || isSkill1CD) return;
        
        isSkill1Using = true;
        StartCoroutine(SkillUsingTick());
        isSkill1LastTimer = skillLastTime;
        characterSkillVFX.gameObject.SetActive(true);
    }

    protected virtual void EndSkill1()
    {
        characterSkillVFX.gameObject.SetActive(false);
    }

    IEnumerator SkillUsingTick()
    {
        countDownUI.gameObject.SetActive(true);
        
        isSkill1Using = true;
        int tickCount = (int)(skillLastTime / 0.1f);
        for(int i = 0;i < tickCount;i++)
        {
            yield return new WaitForSeconds(0.1f);
            float remainTime = skillLastTime - i * 0.1f;
            countDownText.text = remainTime.ToString("F1");
            countDownImage.fillAmount = remainTime / skillLastTime;
        }
        EndSkill1();
        isSkill1CD = true;
        isSkill1Using = false;
        
        int skillCDTickCount = (int)(skillCDTime / 0.1f);
        for(int i=0;i<skillCDTickCount;i++)
        {
            yield return new WaitForSeconds(0.1f);
            float remainTime = skillCDTime - i * 0.1f;
            //更新UI为remainTime
            countDownText.text = remainTime.ToString("F1");
            countDownImage.fillAmount = remainTime / skillCDTime;
            Debug.Log(remainTime / skillCDTime);
        }
        isSkill1CD = false;
        countDownUI.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        // if (isSkill1Using)
        // {
        //     isSkill1LastTimer -= Time.deltaTime;
        //     if (isSkill1LastTimer <= 0)
        //     {
        //         isSkill1Using = false;
        //         EndSkill1();
        //         isSkill1CD = true;
        //         skillCDTimer = skillCDTime;
        //     }
        // }
        // if (isSkill1CD)
        // {
        //     skillCDTimer -= Time.deltaTime;
        //     if (skillCDTimer <= 0)
        //     {
        //         isSkill1CD = false;
        //     }
        // }
    }
}
