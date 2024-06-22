using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YPetWeapon_Ruanmei  : YPetWeapon
{
    [SerializeField]YGrowVines growVines;
    // [SerializeField]
    private float duration = 3f;
    public override void SetDetectShootOn()
    {
        boxCollider.enabled = true;
        growVines.SetGrowVinesOn();
        
        // hitEff.transform.parent = transform;
        // hitEff.transform.localPosition = Vector3.zero;
        // hitEff.transform.localRotation = Quaternion.identity;
        //
        // if (hitEff != null)
        // {
        //     //与parent解绑，否则会跟随parent的位置
        //     hitEff.transform.parent = null;
        //     hitEff.SetActive(true);
        // }
        DOVirtual.DelayedCall(duration, () =>
        {
            SetDetectShootOff();
        }); 
    }
    public override void SetDetectShootOff()
    {
        boxCollider.enabled = false;
        growVines.SetGrowVinesOff();
        
        // if(hitEff != null)
        // {
        //     hitEff.SetActive(false);
        //      
        // }
    }
}
