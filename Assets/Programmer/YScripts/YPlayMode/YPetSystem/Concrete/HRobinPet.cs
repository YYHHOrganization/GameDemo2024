using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class HRobinPet :YPetBase
{
    protected override void InitStateMachine()
    {
        var states = new Dictionary<Type, YPetBaseState>
        {
            { typeof(YPetFollowState), new YPetFollowState(this) },
            { typeof(YPetChaseState), new YPetChaseState(this) },
            { typeof(YPetAttackState), new YPetAttackState(this) }
        };
        GetComponent<YPetStateMachine>().SetStates(states);
    }

    protected override void EnterNewRoom(object sender, YTriggerEnterRoomTypeEventArgs e)
    {
        if (e.roomType == RoomType.BattleRoom || e.roomType == RoomType.BossRoom)
        {
            shouldCheckEnemyCount = true;
            //战歌，起！
            Debug.Log("welcome to my world↑ → ↓ ←");
            HAudioManager.Instance.EaseOutAndStop(HAudioManager.instance.gameObject);
            DOVirtual.DelayedCall(1.5f,() =>
            {
                HAudioManager.Instance.Play("RobinCatcakeBattleAudio", HAudioManager.instance.gameObject, 33.5f);
            });
            //mPetStateMachine.JustSwitchState(typeof(YPetAttackState));
        }
    }

    public override void EnterFollowState()
    {
        base.EnterFollowState();
        HAudioManager.Instance.EaseOutAndStop(HAudioManager.instance.gameObject);
        DOVirtual.DelayedCall(1.5f,() =>
        {
            HAudioManager.Instance.Play("StartRogueAudio", HAudioManager.instance.gameObject);
        });
    }


    public override void MuzzleShoot()
    {
        base.MuzzleShoot();
        
    }
    public override void MuzzleStopShoot()
    {
        base.MuzzleStopShoot();
    }
}
