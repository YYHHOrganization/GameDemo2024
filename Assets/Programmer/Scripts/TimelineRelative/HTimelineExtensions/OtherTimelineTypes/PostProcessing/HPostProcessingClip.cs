using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HPostProcessingClip : PlayableAsset
{
    public List<string> attributes; 
    public List<float> values;
    public EnumHPostProcessingType postProcessingType;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<HPostProcessingBehavior>.Create(graph);
        var postProcessingBehavior = playable.GetBehaviour();
        postProcessingBehavior.attributes = attributes;
        postProcessingBehavior.values = values;
        postProcessingBehavior.postProcessingType = postProcessingType;
        
        return playable;
    }
}
