using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YGachaMachine :YRogue_TriggerGame
{
    protected override void StartInteract()
    {
        if (!yPlanningTable.Instance.isMihoyo)
        {
            HMessageShowMgr.Instance.ShowMessage("GACHA_MACHINE_TIP");
        }
        else
        {
            HPlayerSkillManager.instance.PushGachaPanel();
        }
        //后的效果
        //ui消失
        // getUI.gameObject.SetActive(false);
        
        
    }
}
