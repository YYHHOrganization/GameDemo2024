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
        get { return instance; }
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
        //screenplayList.Add(screenplay);这个好像完全没用啊
        screenplay.OnEnter();
    }

    public void ConfirmScreenplay(Dictionary<string, int> selectId)
    {
        int clipCount = yPlanningTable.Instance.isMoveList.Count;

        yPlanningTable.Instance.BeforePlayTimeline();
        timelineController.BeforePlayTimeline();

        //selectId["animation1"] 与selectId["animation2"] 与selectId["animation3"] 与selectId["animation4"] ，selectId["animation5"] 都表示选择的动画
        for (int i = 1; i <= clipCount; i++)
        {
            YScreenplayBase screenplayAnimation =
                new YAnimationSP(yPlanningTable.Instance.selectNames2Id["animation" + i], selectId["animation" + i],
                    timelineController);
            AddInListAndEnter(screenplayAnimation);
            
            //destination
            YScreenplayBase screenplayDestination = new YDestinationSP(selectId["character"],
                yPlanningTable.Instance.selectNames2Id["destination" + i], selectId["destination" + i],
                timelineController);
            AddInListAndEnter(screenplayDestination);
        }

        YScreenplayBase screenplay = new YCharacterSP(yPlanningTable.Instance.selectNames2Id["character"],
            selectId["character"], timelineController);
        AddInListAndEnter(screenplay);

        for (int i = 1; i <= clipCount; i++)
        {
            //相机
            YScreenplayBase screenplayCamera = new YCameraSP(yPlanningTable.Instance.selectNames2Id["camera" + i],
                selectId["camera" + i], timelineController);
            AddInListAndEnter(screenplayCamera);

            //blendshape
            YScreenplayBase screenplayBlendshape = new YBlendshapeSP(selectId["character"],
                yPlanningTable.Instance.selectNames2Id["blendshape" + i], selectId["blendshape" + i],
                timelineController);
            AddInListAndEnter(screenplayBlendshape);

            //特效
            YScreenplayBase screenplayEffect = new HPostProcessingSP(
                yPlanningTable.Instance.selectNames2Id["effect" + i], selectId["effect" + i], timelineController);
            AddInListAndEnter(screenplayEffect);
        }



        PlayTheTimeline();
    }

    public void PlayTheTimeline()
    {
        timelineController.PlayTheTimeline();
    }

    public void EnterNewLevel()
    {
        if (timelineController)
        {
            timelineController.EnterNewLevel();
            //当进入新的关卡时，应当清空所有的List
        }
        
        // //应该要等到全load完毕再这个 不然就会出现木偶还是在原地  然后又再次触发死亡界面 所以写在timelineController内吧
        // isPuppetDie = false;
        
    }
    
    // private bool isPuppetDie = false;
    
    public void PuppetDie()
    {
        
        timelineController.PuppetDie();
        

    }


}
