using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
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
    
    AnimationTrack animationTrack;

    private List<string> displayNames = new List<string>();

    private List<float> starts = new List<float>();
    
    private List<float> durations = new List<float>();
    private List<AnimationClip> clippps = new List<AnimationClip>();
    
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
        
        //旧版设置固定timeline
        // SetTimelineAssetWithName("DemoTimeline_0");
       
        
        //新版动态加载轨道
        SetTimelineAssetWithName("EmptyTimeline_0");
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
        string trackAnimName = "CharacterAnimation";
        //以下代码含义是：如果bindingDict里有这个trackName，就把这个track的sourceObject设置为character的animator
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            playableDirector.SetGenericBinding(pb.sourceObject, character.GetComponent<Animator>());
        }
        
        if (bindingDict.TryGetValue(trackNameWithPos, out PlayableBinding pb2))
        {
            playableDirector.SetGenericBinding(pb2.sourceObject, character.GetComponent<Animator>());
        }
        if(bindingDict.TryGetValue(trackAnimName, out PlayableBinding pbAnim))
        {
            playableDirector.SetGenericBinding(pbAnim.sourceObject, character.GetComponent<Animator>());
        }

    }

    private float startShabi = 0;
    private float durationShabi = 3;
    //index指的是修改timeline动画轨道的第几个动作
    public void ChangeAnimationWithIndex(int index, AnimationClip clip)
    {
        string trackName = "CharacterAnimation";//"Character" + characterIndex;
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            AnimationTrack track = (AnimationTrack)pb.sourceObject;
            var clips = track.GetClips();
            var targetClip = clips.ElementAt(index).asset as AnimationPlayableAsset;
            
            var shabi = clips.ElementAt(index);
            shabi.start = startShabi + index * durationShabi;
            shabi.duration = durationShabi + 1;
            
            targetClip.clip = clip;
        }
    }
    public void AddAnimationTrackWithIndex(int indexInSequence, AnimationClip animationClip)
    {
        GetTimelineAssetInfos();
        // if (indexInSequence == 0)
        // {
        //     string trackName = "CharacterAnimation";
        //     animationTrack = currentTimelineAsset.CreateTrack<AnimationTrack>(null, trackName);
        // }
        string trackNameTemp = "CharacterAnimation";
        if (bindingDict.TryGetValue(trackNameTemp, out PlayableBinding pb))
        {
            animationTrack = (AnimationTrack)pb.sourceObject;
        }
        
        
        float start = indexInSequence * 7f;
        //playableDirector.SetGenericBinding(track.sourceObject, target.GetComponent<Animator>());
        
        displayNames.Add(animationClip.name);
        clippps.Add(animationClip);
        starts.Add(start);
        durations.Add(7f);
        
        var clip = animationTrack.CreateClip<AnimationPlayableAsset>();
        clip.displayName = animationClip.name;
        clip.start = start;
        clip.duration = 8f;
         clip.easeInDuration = 0.3f;
         clip.easeOutDuration = 0.2f;
         clip.blendInDuration = 0.3f;
         clip.blendOutDuration = 0.2f;
        
        var asset = clip.asset as AnimationPlayableAsset;
        asset.clip = animationClip;
        
        GetTimelineAssetInfos();
    
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
    public void ChangeCinemachine(int index, GameObject cinemachine)
    {
        string trackName = "CinemachineTrack";
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            CinemachineTrack track = (CinemachineTrack)pb.sourceObject;
            
            //获取轨道上的相机
            var clips =  track.GetClips();
            var ClipIndex = clips.ElementAt(index);
            ClipIndex.start = startShabi + index * durationShabi;
            ClipIndex.duration = durationShabi + 1;
            
            var cinemachineShot = ClipIndex.asset as CinemachineShot;
            // cinemachineShot.VirtualCamera = cinemachine.GetComponent<CinemachineVirtualCamera>() as CinemachineVirtualCameraBase;
            //cinemachineShot.VirtualCamera = cinemachine;
            
            //设置Cinemachine的follow
//            cinemachine.GetComponent<CinemachineVirtualCamera>().m_Follow = target.transform;

            cinemachine.GetComponent<CinemachineVirtualCamera>().Follow = target.transform;
            
            playableDirector.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, 
                cinemachine.GetComponent<CinemachineVirtualCamera>());
        }
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
        //ChangeAnimationssssss();
        playableDirector.playableAsset = currentTimelineAsset;
        
        playableDirector.Play();//playableDirector.Pause();
        // Invoke("PauseThenResume2", 3);
        // //Time.timeScale = 0;
        // Invoke("PauseThenResume", 5);
        //StartCoroutine(Resummm());
    }

    void ChangeAnimationssssss()
    {
        string trackNameTemp = "CharacterAnimation";
        if (bindingDict.TryGetValue(trackNameTemp, out PlayableBinding pb))
        {
            animationTrack = (AnimationTrack)pb.sourceObject;
            var clipss = animationTrack.GetClips();
            for(int index=0;index<displayNames.Count;index++)
            {
                var targetClip = clipss.ElementAt(index).asset as AnimationPlayableAsset;
                targetClip.clip = clippps[index];
            }
            
        }
    }

    IEnumerator Resummm()
    {
        yield return new WaitForSecondsRealtime(5f);
        Time.timeScale = 1;
    }
    public void PauseThenResume2()
    {
        //playableDirector重新播放
        //Time.timeScale = 1;
        playableDirector.Pause();
        
        playableDirector.Stop();
        //playableDirector.Play();
    }
    public void PauseThenResume()
    {
        //playableDirector重新播放
        //Time.timeScale = 1;
        playableDirector.Play();
        //playableDirector.Play();
    }
    
}
