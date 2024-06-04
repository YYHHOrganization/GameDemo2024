using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YXingPet :YPetBase
{
    //球棒
    [SerializeField]private GameObject bat;
    protected override void Start()
    {
        base.Start();
    }
    protected override void InitStateMachine()
    {
        var states = new Dictionary<Type, YPetBaseState>
        {
            { typeof(YPetFollowState), new YPetFollowState(this) },
            { typeof(YPetChaseAndAttackEnemyState), new YPetChaseAndAttackEnemyState(this) },
        };
        GetComponent<YPetStateMachine>().SetStates(states);
    }
    
    // public override void MuzzleShoot()
    // {
    //     
    //     ////想在这里使用dotween等实现一个挥舞球棒的动作，例如斜上方挥到斜下方那种
    //     // 创建一条路径，使其从斜上方挥到斜下方并带有弧线效果
    //     // 通过定义沿着弧线旋转的路径来实现球棒挥舞的效果
    //
    //     Vector3 startAngle = new Vector3(-23, 0, -6);  // 球棒起始的旋转角度, 可以根据需要调整
    //     Vector3 middleAngle = new Vector3(67 ,45, 85); // 球棒中间的旋转角度, 像挥棒的中段
    //     Vector3 endAngle = new Vector3(-23, 0, -6);   // 球棒结束的旋转角度, 可以根据需要调整
    //     
    //     Sequence swingSequence = DOTween.Sequence();
    //
    //     swingSequence.Append(bat.transform.DOLocalRotate(startAngle, 0f));
    //     swingSequence.Append(bat.transform.DOLocalRotate(middleAngle, 0.5f).SetEase(Ease.OutQuad)); 
    //     swingSequence.Append(bat.transform.DOLocalRotate(endAngle, 0.5f).SetEase(Ease.InQuad)); 
    //     // 你可以根据需要调整时间 (0.5秒) 和缓动类型 (Ease.OutQuad 和 Ease.InQuad)
    // }
    //
    // public override void MuzzleStopShoot()
    // {
    //     
    // }
}
