using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//以下这句话表示 用于指定轨道需要绑定到的对象类型 比如物体的话感觉可以是绑定到角色身上或者直接生成在地上都可能
// [TrackBindingType(typeof(GameObject))]//不确定是不是这样
[TrackClipType((typeof(YItemClip)))]
public class YItemTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<YItemMixer>.Create(graph, inputCount);
    }
}
