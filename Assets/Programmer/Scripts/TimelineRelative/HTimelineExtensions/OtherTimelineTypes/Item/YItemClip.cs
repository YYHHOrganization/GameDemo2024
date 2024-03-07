using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class YItemClip : PlayableAsset
{
    public List<float> ItemValue;
    public List<int> ItemIndex;
    public List<GameObject> ItemObjects;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<YItemBehavior>.Create(graph);
        var itemBehavior = playable.GetBehaviour();
        itemBehavior.ItemValue = ItemValue;
        itemBehavior.ItemIndex = ItemIndex;
        itemBehavior.ItemObjects = ItemObjects;
        return playable;
    }
}
