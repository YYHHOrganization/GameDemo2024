using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum EnumHPostProcessingType
{
    GlobalVolume,
    RenderFeature,
    UserDefShader,
}

public class HPostProcessingBehavior : PlayableBehaviour
{
    //enum PostprocessingType
    public string globalVolumeField; //要修改到GlobalVolume的哪个属性
    public List<string> attributes; 
    public List<float> values; //同上，只不过是参数值
    public List<float> defaultValues;
    public List<int> shouldLerp; //也通过策划表读取，用来判断在mix的时候是否有必要渐变，1表示需要，0表示不需要
    public EnumHPostProcessingType postProcessingType;
    
    // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    // {
    //     
    // }

    //实测应该是当timeline播放到这个clip的时候，应该去调用对应的函数
    // public override void OnBehaviourPlay(Playable playable, FrameData info)
    // {
    //     Debug.Log("OnBehaviorPlay");
    //     if (postProcessingType == EnumHPostProcessingType.GlobalVolume)
    //     {
    //         HPostProcessingFilters.Instance.SetAttributeAndValueFromTimeline(attributes, values);
    //     }
    //     
    // }
    //
    // //实测应该是当timeline没有播放该clip的时候，应该去调用对应的函数，只会在改变状态的时候调用一次，所以问题不大
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //Debug.Log("OnBehaviorPause");
        if (postProcessingType == EnumHPostProcessingType.GlobalVolume)
        {
            HPostProcessingFilters.Instance.ResetAttributesAndValuesFromTimeline(this);
        }
    }
}
