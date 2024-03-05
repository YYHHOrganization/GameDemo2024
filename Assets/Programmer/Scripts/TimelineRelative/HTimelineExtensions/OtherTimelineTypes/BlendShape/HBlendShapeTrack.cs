using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(SkinnedMeshRenderer))]
[TrackClipType((typeof(HBlendShapeClip)))]
public class HBlendShapeTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<HBlendShapeMixer>.Create(graph, inputCount);
    }
}
