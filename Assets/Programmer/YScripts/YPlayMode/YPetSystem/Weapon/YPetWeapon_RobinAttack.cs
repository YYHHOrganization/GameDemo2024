using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class YPetWeapon_RobinAttack : YPetWeapon
{
    // [SerializeField]
    float duration = 6f;
    public List<Material> materials;

    IEnumerator EaseMusicEffectInOrOut(bool isIn)
    {
        float startValue = 2.0f;
        float endValue = 0.9f;
        float deltaTime = 0.02f;
        if (isIn)
        {
            for (float i = startValue; i >= endValue; i -= deltaTime)
            {
                for(int j = 0; j < materials.Count; j++)
                {
                    materials[j].SetFloat("_Grow", i);
                }
                yield return new WaitForSeconds(deltaTime);
            }
        }
        else
        {
            startValue = 0.9f;
            endValue = 2.0f;
            deltaTime = 0.02f;
            for (float i = startValue; i <= endValue; i += deltaTime)
            {
                for(int j = 0; j < materials.Count; j++)
                {
                    materials[j].SetFloat("_Grow", i);
                }
                yield return new WaitForSeconds(deltaTime);
            }
            boxCollider.enabled = false;
            if(hitEff != null)
            {
                hitEff.SetActive(false);
            }
        }
    }
    
    public override void SetDetectShootOn()
    {
        hitEff.transform.parent = transform;
        hitEff.transform.localPosition = Vector3.zero;
        hitEff.transform.localRotation = Quaternion.identity;
        boxCollider.enabled = true;
        StopAllCoroutines();
        StartCoroutine(EaseMusicEffectInOrOut(true));
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
        StopAllCoroutines();
        StartCoroutine(EaseMusicEffectInOrOut(false));
    }
}
