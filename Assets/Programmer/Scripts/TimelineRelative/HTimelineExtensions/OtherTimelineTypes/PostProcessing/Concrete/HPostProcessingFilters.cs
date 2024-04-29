using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HPostProcessingFilters : HPostProcessingBase
{
    //单例模式
    private static HPostProcessingFilters _instance;
    public static HPostProcessingFilters Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HPostProcessingFilters>();
            }
            return _instance;
        }
    }
    
    private Volume postProcessVolume;

    public Volume PostProcessVolume
    {
        get => postProcessVolume;
    }
    
    private void Start()
    {
       
    }

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();
        PreloadRenderFeature();
    }
    
    
    public void SetAttributeAndValueFromTimelineNew(HPostProcessingBehavior input, float inputWeight)
    {
        if (input.postProcessingType == EnumHPostProcessingType.GlobalVolume)
        {
            SetGlobalVolumeAttributeAndValue(input, inputWeight, false);
        }
        else if(input.postProcessingType == EnumHPostProcessingType.RenderFeature)
        {
            SetRenderFeatureAttributeAndValueFromTimeline(input, inputWeight, false);
        }
    }

    public void ResetAttributesAndValuesFromTimeline(HPostProcessingBehavior input)
    {
        if (input.postProcessingType == EnumHPostProcessingType.GlobalVolume)
        {
            SetGlobalVolumeAttributeAndValue(input, 0, true);
        }
    }

    private float CalculateSetValue(float inputWeight, int shouldLerp, float value, float defaultValue, bool isReset)
    {
        //用shouldLerp来控制是否需要渐变
        if (inputWeight >= 0.0001) //weight仍在控制，用shouldLerp做调整
        {
            if (shouldLerp == 0) inputWeight = 1;
        } 
        float addValue = value * inputWeight; //在写逻辑的时候保证value[i]在加入到默认值上的时候是合理的
        float setValue = defaultValue + addValue;
        if (isReset)
        {
            setValue = defaultValue;
        }

        return setValue;
    }

    private void SetGlobalVolumeAttributeAndValue(HPostProcessingBehavior input, float inputWeight, bool isReset)
    {
        if (!postProcessVolume) return;
        var components = postProcessVolume.profile.components;
        string volumePPType = input.globalVolumeField;
        List<string> attributes = input.attributes;
        List<float> values = input.values;
        List<float> defaultValues = input.defaultValues;
        List<int> shouldLerp = input.shouldLerp;
        
        foreach (var component in components)
        {
            if(component.GetType().Name == volumePPType)
            {
                if (volumePPType == "ColorAdjustments")
                {
                    //Debug.Log("call ColorAdjustments");
                    var colorAdjustments = (ColorAdjustments) component;
                    for(int i = 0; i < attributes.Count; i++)
                    {
                        float setValue = CalculateSetValue(inputWeight, shouldLerp[i], values[i], defaultValues[i], isReset);
                        switch (attributes[i])
                        {
                            case "saturation":
                                //Debug.Log("now we are in saturation");
                                colorAdjustments.saturation.value = setValue;
                                break;
                            case "postExposure":
                                //Debug.Log("now we are in postExposure");
                                colorAdjustments.postExposure.value = setValue;
                                break;
                            case "contrast":
                                //Debug.Log("now we are in contrast");
                                colorAdjustments.contrast.value = setValue;
                                break;
                            case "hueShift":
                                colorAdjustments.hueShift.value = setValue;
                                break;
                        }
                            
                    }
                }
                else if (volumePPType == "HRadialBlurSettings")
                {
                    var radialBlur = (HRadialBlurSettings) component;
                    for(int i = 0; i < attributes.Count; i++)
                    {
                        float setValue = CalculateSetValue(inputWeight, shouldLerp[i], values[i], defaultValues[i], isReset);
                        switch (attributes[i])
                        {
                            case "blurRadius":
                                radialBlur.blurRadius.value = setValue; 
                                break;
                            case "blurIterations":
                                radialBlur.blurIterations.value = (int)setValue;
                                break;
                            
                        }
                    }
                }
                
                else if (volumePPType == "Vignette")
                {
                    var vignette = (Vignette) component;
                    for(int i = 0; i < attributes.Count; i++)
                    {
                        float setValue = CalculateSetValue(inputWeight, shouldLerp[i], values[i], defaultValues[i], isReset);
                        
                        switch (attributes[i])
                        {
                            case "intensity":
                                vignette.intensity.value = setValue; 
                                break;
                            case "smoothness":
                                vignette.smoothness.value = setValue;
                                break;
                        }
                    }
                }
                
                else if (volumePPType == "LensDistortion")
                {
                    var lensDistortion = (LensDistortion) component;
                    for(int i = 0; i < attributes.Count; i++)
                    {
                        float setValue = CalculateSetValue(inputWeight, shouldLerp[i], values[i], defaultValues[i], isReset);
                        
                        switch (attributes[i])
                        {
                            case "intensity":
                                lensDistortion.intensity.value = setValue; 
                                break;
                        }
                    }
                }
            }
            
        }
    }

    
    public void SetRenderFeatureAttributeAndValueFromTimeline(HPostProcessingBehavior input, float inputWeight, bool isReset)
    {
        if (!postProcessVolume) return;
        var components = postProcessVolume.profile.components;
        
        string name = input.globalVolumeField;
        List<string> attributes = input.attributes;
        List<float> values = input.values;
        List<float> defaultValues = input.defaultValues;
        List<int> shouldLerp = input.shouldLerp;
    }

    public void SetPostProcessingWithNameAndTime(string effect, float time)
    {
        StartCoroutine(SetPostProcessingEffect(effect, time));

    }
    
    public void SetPostProcessingWithNameAndValue(string effect, float value)
    {
        Vignette vignette;
        switch (effect)
        {
            case "Vignette":
                if (postProcessVolume.profile.TryGet<Vignette>(out vignette))
                {
                    vignette.intensity.value = value;
                }
                break;
        }
    }

    IEnumerator SetPostProcessingEffect(string effect, float time)
    {
        float originValue = 0f;
        LensDistortion lensDistortion;
        ColorAdjustments colorAdjustments;
        ColorCurves colorCurves;
        
        switch (effect)
        {
            case "xiaojingxi":
                if(postProcessVolume.profile.TryGet<LensDistortion>(out lensDistortion))
                {
                    lensDistortion.intensity.value = 1f;
                    yield return new WaitForSeconds(time);
                    lensDistortion.intensity.value = originValue;
                }
                break;
            case "Sexiangpianyi":
                if (postProcessVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
                {
                    colorAdjustments.hueShift.value = 180f;
                    yield return new WaitForSeconds(time);
                    colorAdjustments.hueShift.value = originValue;
                }
                break;
            case "SexiangpianyiMove":
                if (postProcessVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
                {
                    var sequence = DOTween.Sequence();
                    //15s的时间，hueShift从0到180，再从180到-180，每0.1秒更新一次
                    sequence.Append(DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x, 180f, time/4)); 
                    sequence.Append(DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x, -180f, time/4));
                    sequence.Append(DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x, 180f, time/4)); 
                    sequence.Append(DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x, -180f, time/4));
                    yield return new WaitForSeconds(time);
                }
                colorAdjustments.hueShift.value = originValue;
                break;
            case "HeibaiHong":
                //开启我为逝者哀哭
                if (postProcessVolume.profile.TryGet<ColorCurves>(out colorCurves))
                {
                    colorCurves.active = true;
                    if (postProcessVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
                    {
                        var sequence = DOTween.Sequence();
                        //2s的时间把saturation从0到-100，过10s之后从-100重置回originValue
                        sequence.Append(DOTween.To(() => colorAdjustments.saturation.value, x => colorAdjustments.saturation.value = x, -60f, 2f));
                        yield return new WaitForSeconds(time);
                        colorAdjustments.saturation.value = originValue;
                        colorCurves.active = false;
                    }
                    
                }

                break;
                
        }
    }
    //测试
    public ScriptableRendererFeature unitRendererFeature;
    List<ScriptableRendererFeature> srfList;
    void PreloadRenderFeature()
    {
        ScriptableRendererFeature unitRendererFeature=null;
        UniversalRenderPipelineAsset _pipelineAssetCurrent = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;  // 通过GraphicsSettings获取当前的配置
        _pipelineAssetCurrent = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;  // 通过QualitySettings获取当前的配置
        //_pipelineAssetCurrent = QualitySettings.GetRenderPipelineAssetAt(QualitySettings.GetQualityLevel()) as UniversalRenderPipelineAsset;  // 通过QualitySettings获取不同等级的配置

        // 也可以通过QualitySettings.names遍历所有配置

        srfList = _pipelineAssetCurrent.scriptableRenderer.GetType().
                GetProperty("rendererFeatures",
                    BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(_pipelineAssetCurrent.scriptableRenderer, null)
            as List<ScriptableRendererFeature>;

    }
    public ScriptableRendererFeature GetRenderFeature(string featureName)
    {
        // 创建一个字典用于存储特效名称和对应的特效对象
        // Dictionary<string, ScriptableRendererFeature> effsDict = new Dictionary<string, ScriptableRendererFeature>();

        foreach (ScriptableRendererFeature srf in srfList)
        {
            if (!string.IsNullOrEmpty(srf.name) && srf.name==featureName)
            {
                return srf;
            }
        }
        return null;
        /*
        使用的时候
        //test后处理unitRendererFeature
        unitRendererFeature = GetRenderFeature("FullScreenDoubleBonus");
        //开启这个特效
        if (unitRendererFeature != null)
        {
            unitRendererFeature.SetActive(true);
        }
        或者
        ScriptableRendererFeature renderFeature = HPostProcessingFilters.Instance.GetRenderFeature("FullScreenInvincible");
        renderFeature.SetActive(true);
        
         */
    }
}