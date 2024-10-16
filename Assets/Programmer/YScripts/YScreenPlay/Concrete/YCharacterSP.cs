using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//EnumPlayableCharacter
public enum EnumPlayableCharacter
{
    //玩家
    character1,
    character2,
    character3,
}
public class YCharacterSP : YScreenplayBase
{
    public YCharacterSP(int id,int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        this.id = id;
        path = "Prefabs/YCharacter/"+yPlanningTable.Instance.SelectTable[id][selectId];
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    
    public override void LoadResourcesAndSetTimeline()
    {
        ////加载资源character.transform.position = yPlanningTable.Instance.GetCharacterGeneratePosition().transform.position;
        // GameObject obj = Resources.Load<GameObject>(path);
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(path),
            yPlanningTable.Instance.GetCharacterGeneratePosition().transform.position,
            Quaternion.identity);
        timelineController.ChangeCharacter(selectId,go);
    }

}
