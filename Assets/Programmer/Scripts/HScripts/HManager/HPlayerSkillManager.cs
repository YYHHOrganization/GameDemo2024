using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HPlayerSkillManager : MonoBehaviour
{
    //这个Manager用于管理角色相关的基本技能
    //单例模式
    public static HPlayerSkillManager instance;
    private bool checkScanningSkillCD = false;
    private float scanningSkillCD = 3f;
    private float scanningSkillCDTimer = 0.0f;
    Volume postProcessVolume;
    private float originalIntensity;
    Coroutine ResumeIntensityCoroutine;
    private float scanningIntensity = -2f;

    private bool bagBeenPushed = false;
    public void SetBagBeenPushed(bool value)
    {
        bagBeenPushed = value;
    }

    private bool gachaPanelBeenPushed = false;
    public void SetGachaPanelBeenPushed(bool value)
    {
        gachaPanelBeenPushed = value;
    }

    private void SetOriginalIntensity()
    {
        postProcessVolume.profile.TryGet(out ColorAdjustments settings);
        originalIntensity = settings.postExposure.value;
    }
    private void Awake()
    {
        instance = this;
        postProcessVolume = HPostProcessingFilters.Instance.PostProcessVolume;
        
    }

    private void Start()
    {
        if(!postProcessVolume)
            postProcessVolume = HPostProcessingFilters.Instance.PostProcessVolume;
        ResumeIntensityCoroutine = null;
        SetOriginalIntensity();
    }


    public void SkillScanningTerrian()
    {
        if (checkScanningSkillCD) return;
        checkScanningSkillCD = true;
        //intensity set to -2
        if (postProcessVolume)
        {
            postProcessVolume.profile.TryGet(out ColorAdjustments settings);
            settings.postExposure.value = scanningIntensity;
        }

        ResumeIntensityCoroutine = null;

        YPlayModeController.Instance.DetectViewOn();
    }

    public void CheckSpaceSkill()
    {
        if (YPlayModeController.Instance.LockEveryInputKey) return;
        if (HRoguePlayerAttributeAndItemManager.Instance.IsUsingGMPanel) return;
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            //Debug.Log("Space");
            SkillScanningTerrian();
            HAudioManager.Instance.Play("SweepSceneMusic", this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!bagBeenPushed)
            {
                YGameRoot.Instance.Push(new HBagPanel());
                bagBeenPushed = true;   
            }
        }

        // if (Input.GetKeyDown(KeyCode.F3))
        // {
        //     if (!gachaPanelBeenPushed)
        //     {
        //         YGameRoot.Instance.Push(new HGachaBasePanel());
        //         gachaPanelBeenPushed = true;
        //     }
        // }
        
    }
    
    public void PushGachaPanel()
    {
        if (!gachaPanelBeenPushed)
        {
            YGameRoot.Instance.Push(new HGachaBasePanel());
            gachaPanelBeenPushed = true;
        }
    }
    IEnumerator ResumeIntensity()
    {
        if (postProcessVolume)
        {
            postProcessVolume.profile.TryGet(out ColorAdjustments settings);
            float intensityDiff = originalIntensity - scanningIntensity; //todo:现在写死了-2，懒得改了，后面再用变量控制
            float timeStep = 0.02f;
            float intensityDelta = intensityDiff / 10;
            for (int i = 0; i < 10; i++)
            {
                float value = scanningIntensity + intensityDelta * i;
                settings.postExposure.value = value;
                yield return new WaitForSeconds(timeStep);
            }
            settings.postExposure.value = originalIntensity;
        }
    }

    private void Update()
    {
        CheckSpaceSkill();
        if (checkScanningSkillCD)
        {
            scanningSkillCDTimer += Time.deltaTime;
            if (scanningSkillCDTimer >= scanningSkillCD)
            {
                scanningSkillCDTimer = 0.0f;
                checkScanningSkillCD = false;
                
                //调用YPlayModeController的DetectViewOff
                YPlayModeController.Instance.DetectViewOff();
                
                ResumeIntensityCoroutine = StartCoroutine(ResumeIntensity());
            }
            //用Timer对全局扫描的RenderFeature进行控制
            
            if (postProcessVolume)
            {
                var profile = postProcessVolume.profile;
                if (profile)
                {
                    profile.TryGet(out HTerrianScanRenderFeatureSettings settings);
                    if (settings)
                    {
                        settings.scanDepth.value = scanningSkillCDTimer * 0.5f;
                    }
                }
            }
            else
            {
                postProcessVolume = HPostProcessingFilters.Instance.PostProcessVolume;
                postProcessVolume.profile.TryGet(out ColorAdjustments settings);
                originalIntensity = settings.postExposure.value;
            }
        }
    }
}
