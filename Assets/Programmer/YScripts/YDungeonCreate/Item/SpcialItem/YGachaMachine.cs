using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YGachaMachine :YRogue_TriggerGame
{
    protected override void StartInteract()
    {
        //后的效果
        //ui消失
        // getUI.gameObject.SetActive(false);
        HPlayerSkillManager.instance.PushGachaPanel();
        
    }
}
