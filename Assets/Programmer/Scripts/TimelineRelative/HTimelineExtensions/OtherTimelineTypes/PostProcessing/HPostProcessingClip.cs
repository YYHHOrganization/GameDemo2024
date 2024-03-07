using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HPostProcessingClip : PlayableAsset
{
    public string globalVolumeField; //要修改到GlobalVolume的哪个属性
    public List<string> attributes; 
    public List<float> values; //同上，只不过是参数值
    public List<int> shouldLerp; //也通过策划表读取，用来判断在mix的时候是否有必要渐变，1表示需要，0表示不需要
    public List<float> defaultValues;
    public EnumHPostProcessingType postProcessingType;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<HPostProcessingBehavior>.Create(graph);
        var postProcessingBehavior = playable.GetBehaviour();
        postProcessingBehavior.attributes = attributes;
        postProcessingBehavior.values = values;
        postProcessingBehavior.postProcessingType = postProcessingType;
        postProcessingBehavior.shouldLerp = shouldLerp;
        postProcessingBehavior.globalVolumeField = globalVolumeField;
        postProcessingBehavior.defaultValues = defaultValues;
        
        return playable;
    }
}
