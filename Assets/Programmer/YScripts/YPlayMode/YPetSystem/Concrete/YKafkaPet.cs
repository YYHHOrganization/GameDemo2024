using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YKafkaPet :YPetBase
{
    [SerializeField]private GameObject Spider;
    [SerializeField]private GameObject OriginKafkaPet;
    [SerializeField]Animator SpiderAnimator;
    
    protected void Start()
    {
        base.Start();
        Spider.SetActive(false);
        OriginKafkaPet.SetActive(true);
    }
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
    
    public override void EnterChaseState()
    {
        OriginKafkaPet.SetActive(false);
        Spider.SetActive(true);
    }
    public override void EnterFollowState()
    {
        OriginKafkaPet.SetActive(true);
        Spider.SetActive(false);
    }
    public override void MuzzleShoot()
    {
        base.MuzzleShoot();
        SpiderAnimator.SetInteger("AnimState", 2);//2 is shoot
    }
    public override void MuzzleStopShoot()
    {
        base.MuzzleStopShoot();
        SpiderAnimator.SetInteger("AnimState", 0);//0 is walk
    }
}
