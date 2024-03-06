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
        postProcessVolume = GetComponent<Volume>();
    }

    //todo:先用一个非常简单的策划表来实现
    public void GetAttributesAndValuesFromDesigner(string postProcessingType, ref string VolumePPType, ref List<string>attributes, ref List<float> values)
    {
        if (postProcessingType == "Heibai")
        {
            VolumePPType = "ColorAdjustments";
            attributes.Add("saturation");
            values.Add(-100);
        }
    }
    
    public void SetAttributeAndValueFromTimeline(List<string> attributes, List<float> values)
    {
        for(int i=0;i<attributes.Count;i++)
        {
            string volumePPType = "";
            List<string> attributesList = new List<string>();
            List<float> valuesList = new List<float>();
            GetAttributesAndValuesFromDesigner(attributes[i], ref volumePPType, ref attributesList, ref valuesList);
            
            SetVolumePostProcessingNoob(volumePPType, attributesList, valuesList);
        }
    }

    public void ResetAttributesAndVolumes()
    {
        var components = postProcessVolume.profile.components;
        foreach (var component in components)
        {
            component.active = false;
        }
    }

    public void SetVolumePostProcessingNoob(string volumePPType, List<string> attributes, List<float> values)
    {
        var components = postProcessVolume.profile.components;
        foreach (var component in components)
        {
            if(component.GetType().Name == volumePPType)
            {
                component.active = true;
                if (volumePPType == "ColorAdjustments")
                {
                    var colorAdjustments = (ColorAdjustments) component;
                    for(int i = 0; i < attributes.Count; i++)
                    {
                        if(attributes[i] == "saturation")
                            colorAdjustments.saturation.value = values[i]; 
                    }
                }
            }
            
        }
    }
    
    public void SetVolumePostProcessing(string volumePPType, List<string> attributes, List<float> values)
    {
        var components = postProcessVolume.profile.components;
        foreach (var component in components)
        {
            //Debug.Log("now we are here" + volumePPType);
            //Debug.Log(component.GetType().Name);
            if(component.GetType().Name == volumePPType)
            {
                Debug.Log("Set component!!");
                //set the value of the component
                for(int i = 0;i < attributes.Count; i++)
                {
                    // var field = component.GetType().GetField(attributes[i]);
                    // field.SetValue(component, values[i]);
                    
                    //todo:反射好像做不了，没办法把ClampedFloatParameter转换成Float
                    if(attributes[i] == "saturation")
                    {
                        
                    }
                }
            }
            
        }
    }
}
