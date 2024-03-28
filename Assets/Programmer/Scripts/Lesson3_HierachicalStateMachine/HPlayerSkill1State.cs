using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPlayerSkill1State : HPlayerBaseState
{
    private HCharacterSkillBase skillScipt;
    //private bool isSkill1Using = false;
    public HPlayerSkill1State(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        skillScipt = _ctx.gameObject.GetComponent<HCharacterSkillBase>();
    }

    public override void EnterState()
    {
        _ctx.Animator.SetTrigger(_ctx.IsSkill1Hash);
        string characterName = _ctx.gameObject.name;
        Debug.Log("characterName: " + characterName);
        //HAudioManager.Instance.Play("PlayerSkill1", _ctx.gameObject);
        
        skillScipt.PlaySkill1();
    }

    public override void UpdateState()
    {
        
    }
    
    public override void ExitState()
    {
        
    }
    
    public override void CheckSwitchStates()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }
}
