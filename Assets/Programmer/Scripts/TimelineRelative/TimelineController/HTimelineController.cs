using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine.Video;

public class HTimelineController : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private readonly string timelineAssetPath = "TimelineAssets/";
    private TimelineAsset currentTimelineAsset;
    
    //这里是所有的timeline轨道的绑定信息
    private readonly Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();

    private int characterIndex;
    private GameObject target;

    private ScriptableRendererFeature signalThing;
    
    BlendshapeController blendshapeController;
    
    //根据索引设置对应的TimelineAsset，扩展后面可能有多幕的剧情
    public void SetTimelineAsset(int index)
    {
        //这样clone出一个新的timelineAsset，不会影响原来的timelineAsset
        //todo:为什么动画资产会替代掉原来的
        TimelineAsset asset = Resources.Load<TimelineAsset>(timelineAssetPath + index.ToString());
        currentTimelineAsset = Instantiate(asset);
    }

    public void SetTimelineAssetWithName(string name)
    {
        TimelineAsset asset = Resources.Load<TimelineAsset>(timelineAssetPath + name);
        currentTimelineAsset = Instantiate(asset);
    }

    void TestSetTimelineContentEverything()
    {
        //设置角色
        GameObject xina = Instantiate(Resources.Load<GameObject>("Test/Xina/XinaSecond"));
        ChangeCharacter(0, xina);
        
        //设置动画
        AnimationClip clip = Resources.Load<AnimationClip>("Test/Xina/Fast Run Test");
        ChangeAnimationWithIndex(0, clip);
        
        //设置不同的机位
        GameObject cinemachine1 = Instantiate(Resources.Load<GameObject>("Test/Xina/Cinemachine1"));
        ChangeCinemachineWithIndex(0, cinemachine1);
        
        //设置不同的Effect
        ParticleSystem particle = Instantiate(Resources.Load<ParticleSystem>("Test/Xina/Particle1"));
        
        //设置不同的sound
        AudioClip audio = Resources.Load<AudioClip>("Test/Xina/Audio2");
        ChangeSoundWithIndex(0, audio);
    }

    //获取timeline的所有信息
    private void GetTimelineAssetInfos()
    {
        foreach (var bind in currentTimelineAsset.outputs)
        {
            bindingDict.TryAdd(bind.streamName, bind);
            Debug.Log(bind.streamName);
        }
    }
    
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        //暂时先设置第一幕
        //SetTimelineAsset(0);
        SetTimelineAssetWithName("DemoTimeline_0");
        GetTimelineAssetInfos();
        blendshapeController = new BlendshapeController();
        //TestSetTimelineContentEverything();
        //PlayTheTimeline();
    }

    //根据索引设置对应的角色
    public void ChangeCharacter(int index, GameObject character)
    {
        characterIndex = index;
        target = character.gameObject;
        //修改对应track的角色，要求角色要有Animator组件
        string trackName = "Character" + index;
        string trackNameWithPos = "Character" + index + "Pos";
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            playableDirector.SetGenericBinding(pb.sourceObject, character.GetComponent<Animator>());
        }
        
        if (bindingDict.TryGetValue(trackNameWithPos, out PlayableBinding pb2))
        {
            playableDirector.SetGenericBinding(pb2.sourceObject, character.GetComponent<Animator>());
        }
    }

    //index指的是修改timeline动画轨道的第几个动作
    public void ChangeAnimationWithIndex(int index, AnimationClip clip)
    {
        string trackName = "Character" + characterIndex;
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            AnimationTrack track = (AnimationTrack)pb.sourceObject;
            var clips = track.GetClips();
            var targetClip = clips.ElementAt(index).asset as AnimationPlayableAsset;
            targetClip.clip = clip;
        }
    }

    public void ChangeCinemachineWithIndex(int index, GameObject cinemachine)
    {
        // 把index对应轨道的物体设置为某个新的cinemachine
        string trackName = "Cinemachine" + index;
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            playableDirector.SetGenericBinding(pb.sourceObject, cinemachine);
        }
        //cinemachine跟随的对象也要进行实际的更新
        //todo:后面要修改一下Cinemachine的类型，并且有些预制的参数要进行修改
        cinemachine.GetComponent<CinemachineVirtualCamera>().m_Follow = target.transform;
        cinemachine.GetComponent<CinemachineVirtualCamera>().m_LookAt = target.transform;
    }

    public void ChangeEffectWithIndex(int index, GameObject effect)
    {
        //把index对应轨道的effect设置为某个新的效果
        //todo: 回来再弄，现在累了
    }

    public void SetSignalThingsActive(bool active)
    {
        if (signalThing)
        {
            signalThing.SetActive(active);
        }
    }

    public void ChangeSciptableRenderFeatureEffectWithSignal(ScriptableRendererFeature effect)
    {
        signalThing = effect;
    }

    public void ChangeSoundWithIndex(int index, AudioClip sound)
    {
        //index是要修改第几段音频
        string trackName = "Audio Track";
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            AudioTrack track = (AudioTrack)pb.sourceObject;
            var clips = track.GetClips();
            var targetClip = clips.ElementAt(index).asset as AudioPlayableAsset;
            targetClip.clip = sound;
        }
    }

    public void ChangeCertainObjectWithName(string name, GameObject obj)
    {
        //return;
        //name = "Movie";
        string trackName = name + "Track";
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            playableDirector.SetGenericBinding(pb.sourceObject, obj);
            obj.GetComponent<VideoPlayer>().targetCamera = Camera.main;
            // obj.GetComponent<Camera>().
        }
    }

    public void ChangeTresureWithIndex(int index, string path)
    {
        //https://docs.unity3d.com/cn/2021.3/Manual/class-VideoPlayer.html
        
        

    }
    //blendshape
    public void ChangeBlendshapeWithIndex(int characterId, int indexInTimeline, int selectId)
    {
        string trackName = "BlendShapeTrack";
        //blendshapeController.SetBlendshape( characterId, target.GetComponentInChildren<SkinnedMeshRenderer>(),selectId, true); 
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            playableDirector.SetGenericBinding(pb.sourceObject, target.GetComponentInChildren<SkinnedMeshRenderer>());
            HBlendShapeTrack track = (HBlendShapeTrack)pb.sourceObject;
            var clips = track.GetClips();
            var targetClip = clips.ElementAt(indexInTimeline).asset as HBlendShapeClip;
            targetClip.blendShapeIndex = blendshapeController.GetIndexes(characterId, selectId);
            List<float> blendshapeValue = new List<float>();
            //todo:这个值后面可能也要在策划表中设置
            for(int j=0;j<targetClip.blendShapeIndex.Count;j++)
            {
                blendshapeValue.Add(100);
            }

            targetClip.blendShapeValue = blendshapeValue;
        }
    }
        //把index对应轨道的effect设置为某个新的效果
    public void PlayTheTimeline()
    {
        playableDirector.playableAsset = currentTimelineAsset;
        playableDirector.Play();
    }
}
