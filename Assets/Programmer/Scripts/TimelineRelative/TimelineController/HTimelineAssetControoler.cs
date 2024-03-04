using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class HTimelineAssetControoler : MonoBehaviour
{
    private PlayableDirector playableDirector;
    public AnimationClip tmpClipTest;
    public TimelineAsset testTimelineAsset;

    private readonly Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();
    // Start is called before the first frame update
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        //ChangeTimelineAsset();
        foreach (var bind in playableDirector.playableAsset.outputs)
        {
            bindingDict.TryAdd(bind.streamName, bind);
            //Debug.Log(bind.streamName);
        }
        //SetAnimationClip("XinaAnimationClips", 0);
        PlayTheTimeline();
    }
    
    void ChangeTimelineAsset()
    {
        playableDirector.playableAsset = testTimelineAsset;
    }
    
    void PlayTheTimeline()
    {
        playableDirector.Play();
    }

    public void SetAnimationClip(string trackName, int index)
    {
        //修改对应track的对应index的clip
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            AnimationTrack track = (AnimationTrack)pb.sourceObject;
            var clips = track.GetClips();
            var clip = clips.ElementAt(index).asset as AnimationPlayableAsset;
            clip.clip = tmpClipTest;
        }
    }
    
    //动态设置轨道的绑定物体，这个函数应该最简单的是修改Cinemachine的类型
    public void SetTrackOwnerDynamic(string trackName, GameObject gameObject)
    {
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            playableDirector.SetGenericBinding(pb.sourceObject, gameObject);
        }
    }
  

    // Update is called once per frame
    void Update()
    {
        
    }
}
