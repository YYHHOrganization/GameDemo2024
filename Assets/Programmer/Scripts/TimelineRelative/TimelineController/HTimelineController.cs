using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine.Video;
using Random = UnityEngine.Random;

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
    SkinnedMeshRenderer currentSkinnedMeshRenderer;
    
    AnimationTrack animationTrack;

    private List<string> displayNames = new List<string>();

    private List<float> starts = new List<float>();
    
    private List<float> durations = new List<float>();
    private List<AnimationClip> clippps = new List<AnimationClip>();
    List<float> animationClipLengths = new List<float>();
    
    public string timelineAssetName = "EmptyTimeline_0";
    
    //到达目的地或播放完动画跳到下一个clip相关的变量
    private int m_clipIndex=0;
    bool m_isSammPlace = false;
    bool duringSamePlaceCoroutine = false;
    
    // public struct CameraStruct
    // {
    //     public string cameraName;
    //     public string cameraUIName;
    //     //是否是follow
    //     public bool isFollow;
    //     //是否是lookat
    //     public bool isLookAt;
    // }
    List<yPlanningTable.CameraStruct> cameraStructs = new List<yPlanningTable.CameraStruct>();
    
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
    
    [Obsolete("NEVER CALL THIS!! This method is deprecated, just for test")]
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
        SetTimelineAssetWithName(timelineAssetName);

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
            playableDirector.SetGenericBinding(pb.sourceObject, character.GetComponentInChildren<Animator>());
        }
        
        // if (bindingDict.TryGetValue(trackNameWithPos, out PlayableBinding pb2))
        // {
        //     playableDirector.SetGenericBinding(pb2.sourceObject, character.GetComponentInChildren<Animator>());
        // }
        if(bindingDict.TryGetValue(trackAnimName, out PlayableBinding pbAnim))
        {
            playableDirector.SetGenericBinding(pbAnim.sourceObject, character.GetComponentInChildren<Animator>());
        }
        if(bindingDict.TryGetValue("DestinationTrack", out PlayableBinding pbnav))
        {
            playableDirector.SetGenericBinding(pbnav.sourceObject, character.gameObject.GetComponent<NavMeshAgent>());
        }
    }

    private float testAnimationStart = 0;
    private float testAnimationDuration = 20;
    //index指的是修改timeline动画轨道的第几个动作
    public void ChangeAnimationWithIndex(int index, AnimationClip clip)
    {
        string trackName = "CharacterAnimation";//"Character" + characterIndex;
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            AnimationTrack track = (AnimationTrack)pb.sourceObject;
            var clips = track.GetClips();
            var targetClip = clips.ElementAt(index).asset as AnimationPlayableAsset;
            
            var thisClip = clips.ElementAt(index);
            thisClip.start = testAnimationStart + index * testAnimationDuration;
            thisClip.duration = testAnimationDuration + 1;
            
            targetClip.clip = clip;
        }
        //我们可以多做一件事，就是把这个clip的动画的时长存下来，后面可能会用到，但是还应该乘上这个clip的speed
        // float aniationClipOriginalDuration = clip.length * clip.frameRate;
        float aniationClipOriginalDuration = clip.length;
        animationClipLengths.Add(aniationClipOriginalDuration);
        // Debug.Log("clip length: " + aniationClipOriginalDuration);
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
    
    
    public void ChangeCinemachine(int index, GameObject cinemachine,string name,int selectId)
    {
        string trackName = "CinemachineTrack";
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            CinemachineTrack track = (CinemachineTrack)pb.sourceObject;
            
            //获取轨道上的相机
            var clips =  track.GetClips();
            var ClipIndex = clips.ElementAt(index);
            ClipIndex.start = testAnimationStart + index * testAnimationDuration;
            ClipIndex.duration = testAnimationDuration + 1;
            
            var cinemachineShot = ClipIndex.asset as CinemachineShot;
            // cinemachineShot.VirtualCamera = cinemachine.GetComponent<CinemachineVirtualCamera>() as CinemachineVirtualCameraBase;
            //cinemachineShot.VirtualCamera = cinemachine;
            
            //设置Cinemachine的follow
//            cinemachine.GetComponent<CinemachineVirtualCamera>().m_Follow = target.transform;

            yPlanningTable.CameraStruct cameraStruct = cameraStructs[selectId];
            if(cameraStruct.isFollow)
            {
                cinemachine.GetComponent<CinemachineVirtualCamera>().Follow = target.transform;
            }
            if(cameraStruct.isLookAt)
            {
                cinemachine.GetComponent<CinemachineVirtualCamera>().LookAt = target.transform;
            }
            // if(name != "FarSurveillanceCamera")
            // {
            //     cinemachine.GetComponent<CinemachineVirtualCamera>().Follow = target.transform;
            // }
            //
            // cinemachine.GetComponent<CinemachineVirtualCamera>().LookAt = target.transform;
            
            playableDirector.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, 
                cinemachine.GetComponent<CinemachineVirtualCamera>());
            
            // Debug.Log(name);
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
            currentSkinnedMeshRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
            playableDirector.SetGenericBinding(pb.sourceObject, currentSkinnedMeshRenderer);
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
            var TimelineClipIndex = clips.ElementAt(indexInTimeline);
            TimelineClipIndex.start = testAnimationStart + indexInTimeline * testAnimationDuration + 3f * 0.2f;
            TimelineClipIndex.duration = 4f  * 0.6f;
        }
    }

    public List<GameObject> gooo=new List<GameObject>();
    /// <summary>
    /// 用于更改每一段clip的目的地
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="indexInTimeline"></param>
    /// <param name="selectId"></param>
    public void ChangeDestinationWithIndex(int characterId, int indexInTimeline, int selectId)
    {
        string trackName = "DestinationTrack";
        
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            NavMeshAgentControlTrack track = (NavMeshAgentControlTrack)pb.sourceObject;
            var clips = track.GetClips();
            var ClipIndex = clips.ElementAt(indexInTimeline);
            ClipIndex.start = testAnimationStart + indexInTimeline * testAnimationDuration;
            ClipIndex.duration = testAnimationDuration;
            var navMeshAgentControlClip = ClipIndex.asset as NavMeshAgentControlClip;
            //Debug.Log("five??" + indexInTimeline);
            
            GameObject go1 = yPlanningTable.Instance.GetDestination(selectId).gameObject;
            playableDirector.SetReferenceValue(navMeshAgentControlClip.destination.exposedName,
                go1.transform);
            // playableDirector.SetReferenceValue(navMeshAgentControlClip.destination.exposedName,
            //     go1.transform);
            
            //无用，后面的版本没问题可以把下面的删掉
            gooo.Add(go1);
            //pringGIII(gooo);


            //Debug.Log("destination  "+ yPlanningTable.Instance.GetDestination(selectId).position);

        }
    }

    void pringGIII(List<GameObject> go)
    {
        foreach (var VARIABLE in gooo)
        {
            Debug.Log("go  "+VARIABLE + "  "+VARIABLE.transform.position);
        }
    }
    
    //把index对应轨道的effect设置为某个新的效果
    public void ChangePostProcessingWithIndex(int indexInTimeline, int selectId)
    {
        // string PostProcessingName = yPlanningTable.Instance.postEffectNames[selectId];
        // if(PostProcessingName == "null")
        // {
        //     return;
        // }
        
        //todo:这里不太会写，后面补充一下，关于后处理的逻辑
        //另外，如果要调整的参数不是float类型的怎么办？比如说颜色，后面再说
        string trackName = "PostProcessingTrack";
        if (bindingDict.TryGetValue(trackName, out PlayableBinding pb))
        {
            HPostProcessingTrack track = (HPostProcessingTrack)pb.sourceObject;
            var clips = track.GetClips();
            var targetClip = clips.ElementAt(indexInTimeline).asset as HPostProcessingClip;
            
            //test
            // targetClip.name = PostProcessingName + indexInTimeline;
            // Debug.Log("targetClip name: " + targetClip.name);
            
            List<string> attributesName = yPlanningTable.Instance.postEffectAttributeNames[selectId];
            List<float> attributesValue = yPlanningTable.Instance.postEffectAttributeValues[selectId];
            List<float> attributesDefaultValue = yPlanningTable.Instance.postEffectDefaultValues[selectId];
            List<int> attributesShouldLerp = yPlanningTable.Instance.postEffectShouldLerp[selectId];
            string fieldName = yPlanningTable.Instance.postEffectFieldNames[selectId];
            string type = yPlanningTable.Instance.postEffectTypes[selectId];

            targetClip.attributes = attributesName;
            targetClip.values = attributesValue;
            targetClip.defaultValues = attributesDefaultValue;
            targetClip.shouldLerp = attributesShouldLerp;
            targetClip.globalVolumeField = fieldName;
            targetClip.postProcessingType = EnumHPostProcessingType.GlobalVolume;
            
            var TimelineClipIndex = clips.ElementAt(indexInTimeline);
            TimelineClipIndex.start = testAnimationStart + indexInTimeline * testAnimationDuration;
            TimelineClipIndex.duration = testAnimationDuration +1;
        }
    }
    private bool startPlaying = false;

    public void ChangePostProcessingWithIndexAndRenderFeature(int indexInTimeline, int selectId,
        ScriptableRendererFeature unitRendererFeature)
    {
        //todo:RenderFeature是不是也是直接从策划表当中读取比较好？
    }
    
    public void PlayTheTimeline()
    {
       
        
        playableDirector.playableAsset = currentTimelineAsset;
        
        playableDirector.Play();//playableDirector.Pause();
        //playableDirector.time = 5;
        startPlaying = true;
        
        //如果玩家没有选择任何东西，直接结束
        checkToEnd();
        
        //判断第一帧是否是同一个地方
        if (yPlanningTable.Instance.isMoveList.Count > 0&&yPlanningTable.Instance.isMoveList[0] == false)
        {
            m_isSammPlace = true;
        }
        
    }

    public void BeforePlayTimeline()
    {
        cameraStructs = yPlanningTable.Instance.cameraStructs;
    }

    private void Update()
    {
        changeClip();
    }

    bool isEnd = false;
    public bool checkToEnd()
    {
        int allClipCount = yPlanningTable.Instance.isMoveList.Count;
        if(m_clipIndex>=allClipCount)
        {
            //如果已经到达末尾 应该直接消失  或者到达末尾
            playableDirector.time = 5* testAnimationDuration+0.1;
            startPlaying = false;
            return true;
        }
        return false;
       
    }
    public void changeClip()
    {
        if (startPlaying)
        {
            if (m_clipIndex >= gooo.Count)
            {
                return;
            }
            //其实初始也应该判断sameplace
            if (m_isSammPlace == true && duringSamePlaceCoroutine == false)
            {
                duringSamePlaceCoroutine = true;
                //3秒后再走  TODO:这个时间应该是策划表中设置的或者直接是这个动画的完整长度
                StartCoroutine(WaitAndGo(animationClipLengths[m_clipIndex]));
                
            }
            if (!duringSamePlaceCoroutine&&(Vector3.Distance(target.transform.position, gooo[m_clipIndex].transform.position) < 1f))
            {
                changeToNextClip();
            }
            
        }
    }
    IEnumerator WaitAndGo(float time)
    {
        //yield return new WaitFor
        //等待多少帧
        // float timer = 0f;
        // while (timer < time * Time.deltaTime)
        // {
        //     timer += Time.deltaTime;
        //     yield return null; // 等待下一帧
        // }
        
        yield return new WaitForSeconds(time);
        m_isSammPlace = false;
        duringSamePlaceCoroutine = false;
        changeToNextClip();
    }
    public void changeToNextClip()
    {
        
        //todo：突然跳帧有个问题，动画可能不连贯
        m_clipIndex++;
        
        if (checkToEnd())
        {
            return;
        }
        
        float clipLocateTime = (m_clipIndex) * testAnimationDuration;
        playableDirector.time = clipLocateTime;
        
        if (m_clipIndex < gooo.Count&&gooo[m_clipIndex]==gooo[m_clipIndex-1])//如果是同一个地方，那么就不要再走了
        {
            m_isSammPlace = true;
        }
        else
        {
            m_isSammPlace = false;
        }
        
        blendshapeController.ResetBlendshape( currentSkinnedMeshRenderer,characterIndex);
    }
    
}
