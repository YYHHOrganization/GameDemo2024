using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
       
    }

    private void Awake()
    {
        postProcessVolume = GetComponent<Volume>();
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
                        //用shouldLerp来控制是否需要渐变
                        if (inputWeight >= 0.0001) //weight仍在控制，用shouldLerp做调整
                        {
                            if (shouldLerp[i] == 0) inputWeight = 1;
                        } 
                        float addValue = values[i] * inputWeight; //在写逻辑的时候保证value[i]在加入到默认值上的时候是合理的
                        float setValue = defaultValues[i] + addValue;

                        if (isReset)
                        {
                            setValue = defaultValues[i];
                        }
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
                        //用shouldLerp来控制是否需要渐变
                        if (inputWeight >= 0.0001) //weight仍在控制，用shouldLerp做调整
                        {
                            if (shouldLerp[i] == 0) inputWeight = 1;
                        } 
                        float addValue = values[i] * inputWeight; //在写逻辑的时候保证value[i]在加入到默认值上的时候是合理的
                        float setValue = defaultValues[i] + addValue;
                        if (isReset)
                        {
                            setValue = defaultValues[i];
                        }
                        
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

}