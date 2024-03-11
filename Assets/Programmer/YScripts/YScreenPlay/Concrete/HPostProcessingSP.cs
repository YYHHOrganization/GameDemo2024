using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPostProcessingSP : YScreenplayBase
{
   int indexInTimeline;

   public HPostProcessingSP(int id, int selectId, HTimelineController timelineController) : base(selectId,
      timelineController)
   {
      this.id = id;
      this.indexInTimeline = yPlanningTable.Instance.selectSequenceInSelfClass[id];
      this.selectId = selectId;
   }

   public override void LoadResourcesAndSetTimeline()
   {
      //直接修改timeline的对应资产
      timelineController.ChangePostProcessingWithIndex(indexInTimeline, selectId);
   }
}
