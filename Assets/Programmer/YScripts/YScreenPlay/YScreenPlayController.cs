using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YScreenPlayController : MonoBehaviour
{
    //单例
    private static YScreenPlayController instance;
    public GameObject timeline;
    
    public static YScreenPlayController Instance
    {
        get
        {
            return instance;
        }
    }
    HTimelineController timelineController;
    //设置blendshape
    BlendshapeController blendshapeController;

    private void Awake()
    {
        instance = this;
        timelineController = timeline.gameObject.GetComponent<HTimelineController>();
        blendshapeController = new BlendshapeController();
    }

    //剧本list 用来存放剧本的每个选项YScreenplayBase类型
    private List<YScreenplayBase> screenplayList = new List<YScreenplayBase>();
    
    //调用timeline
    public void CallTimeline()
    {
        //调用timeline
        // TimelineController.instance.Set(screenplayList);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void AddInListAndEnter(YScreenplayBase screenplay)
    {
        screenplayList.Add(screenplay);
        screenplay.OnEnter();
    }

    public void ConfirmScreenplay( Dictionary<string,int> selectId)
    {
        YScreenplayBase screenplay = new YCharacterSP(selectId["character"],timelineController);
        AddInListAndEnter(screenplay);
        
        YScreenplayBase screenplayEff = new YEffectSP(selectId["effect"],timelineController);
        AddInListAndEnter(screenplayEff);
        
        YScreenplayBase screenplaySound = new YAudioSP(selectId["audio"],timelineController);
        AddInListAndEnter(screenplaySound);

        //animation
        YScreenplayBase screenplayAnimation = new YAnimationSP(1,selectId["animation"],timelineController);
        AddInListAndEnter(screenplayAnimation);
        
        YScreenplayBase screenplayAnimation2 = new YAnimationSP(8,selectId["animation2"],timelineController);
        AddInListAndEnter(screenplayAnimation2);
        
        //camera
        YScreenplayBase screenplayCamera = new YCameraSP(selectId["camera"],timelineController);
        AddInListAndEnter(screenplayCamera);
        
        //treasure
        YScreenplayBase screenplayTreasure = new YTreasureSP(selectId["treasure"], timelineController);
        AddInListAndEnter(screenplayTreasure);
        
        // blendshape
         YScreenplayBase screenplayBlendshape = new YBlendshapeSP(selectId["character"],0,selectId["blendshape"],timelineController);
         AddInListAndEnter(screenplayBlendshape);
        
        timelineController.PlayTheTimeline();
    }

}
