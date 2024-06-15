using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YPetWeapon_37IceAttack : YPetWeapon
{
    // [SerializeField]
    float duration = 6f;
    public override void SetDetectShootOn()
    {
        hitEff.transform.parent = transform;
        hitEff.transform.localPosition = Vector3.zero;
        hitEff.transform.localRotation = Quaternion.identity;
        boxCollider.enabled = true;
        if (hitEff != null)
        {
            //与parent解绑，否则会跟随parent的位置
            hitEff.transform.parent = null;
            hitEff.SetActive(true);
        }
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
