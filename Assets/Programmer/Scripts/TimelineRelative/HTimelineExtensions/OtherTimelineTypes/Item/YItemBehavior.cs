using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class YItemBehavior : PlayableBehaviour
{
    public List<float> ItemValue;
    public List<int> ItemIndex;
    public List<GameObject> ItemObjects;
    
    //实测应该是当timeline播放到这个clip的时候，应该去调用对应的函数
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("OnBehaviorPlay");
        
    }

    //实测应该是当timeline没有播放该clip的时候，应该去调用对应的函数，只会在改变状态的时候调用一次，所以问题不大
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log("OnBehaviorPause");
        
    }
}

