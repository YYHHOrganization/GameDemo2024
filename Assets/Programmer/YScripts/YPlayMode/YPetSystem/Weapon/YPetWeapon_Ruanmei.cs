using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YPetWeapon_Ruanmei  : YPetWeapon
{
    [SerializeField]YGrowVines growVines;
    [SerializeField]
    private float DetectCollisionDelay = 1f;
    [SerializeField]
    private float duration = 3f;
    void Start()
    {
        boxCollider.enabled = false;
    }
    public override void SetDetectShootOn()
    {
        growVines.transform.parent = transform;
        growVines.transform.localPosition = Vector3.zero;
        growVines.transform.localRotation = Quaternion.identity;
        growVines.transform.parent = null;
        growVines.SetGrowVinesOn();
        //
        if (hitEff != null)
        {
            //与parent解绑，否则会跟随parent的位置
            //
            hitEff.SetActive(true);
        }
        DOVirtual.DelayedCall(DetectCollisionDelay, () =>
        {
            boxCollider.enabled = true;
        });
        DOVirtual.DelayedCall(duration, () =>
        {
            SetDetectShootOff();
        }); 
    }
    public override void SetDetectShootOff()
    {
        boxCollider.enabled = false;
        growVines.SetGrowVinesOff();
        
        if(hitEff != null)
        {
            hitEff.SetActive(false);
        }
    }
}
