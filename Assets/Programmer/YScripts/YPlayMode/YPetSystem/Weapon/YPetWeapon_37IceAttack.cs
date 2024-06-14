using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YPetWeapon_37IceAttack : YPetWeapon
{
    [SerializeField]float duration = 4f;
    public override void SetDetectShootOn()
    {
        boxCollider.enabled = true;
        if(hitEff != null)
            hitEff.SetActive(true);
        DOVirtual.DelayedCall(duration, () =>
        {
            SetDetectShootOff();
        }); 
    }
    public override void SetDetectShootOff()
    {
        boxCollider.enabled = false;
        if(hitEff != null)
        {
            hitEff.SetActive(false);
             
        }
    }
}
