using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HBlendShapeClip : PlayableAsset
{
    public List<float> blendShapeValue;
    public List<int> blendShapeIndex;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<HBlendShapeBehavior>.Create(graph);
        var blendShapeBehavior = playable.GetBehaviour();
        blendShapeBehavior.blendShapeValue = blendShapeValue;
        blendShapeBehavior.blendShapeIndex = blendShapeIndex;
        return playable;
    }
}
