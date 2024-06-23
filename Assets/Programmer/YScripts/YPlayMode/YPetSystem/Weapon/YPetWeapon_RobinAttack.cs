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
    private GameObject musicEffects;

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
            boxCollider.enabled = true;
            musicEffects.SetActive(true);
            if (hitEff != null)
            {
                //与parent解绑，否则会跟随parent的位置
                //hitEff.transform.parent = null;
                hitEff.SetActive(true);
            }
            DOVirtual.DelayedCall(duration, () =>
            {
                SetDetectShootOff();
            }); 
        }
        else
        {
            startValue = 0.9f;
            endValue = 2.0f;
            deltaTime = 0.02f;
            musicEffects.SetActive(false);
            boxCollider.enabled = false;
            for (float i = startValue; i <= endValue; i += deltaTime)
            {
                for(int j = 0; j < materials.Count; j++)
                {
                    materials[j].SetFloat("_Grow", i);
                }
                yield return new WaitForSeconds(deltaTime);
            }
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
        if (!musicEffects)
        {
            musicEffects = hitEff.transform.Find("RobinBullet/MusicEffect").gameObject;
        }
        StopAllCoroutines();
        StartCoroutine(EaseMusicEffectInOrOut(true));
    }
    public override void SetDetectShootOff()
    {
        StopAllCoroutines();
        StartCoroutine(EaseMusicEffectInOrOut(false));
    }
}
