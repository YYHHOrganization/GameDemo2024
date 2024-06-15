using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YPetWeapon_XingBat : YPetWeapon
{
    Vector3 startAngle = new Vector3(-60,0,0);  // 球棒起始的旋转角度, 可以根据需要调整
    Vector3 middle1Angle = new Vector3(-85,0,0); // 球棒中间的旋转角度, 像挥棒的中段
    Vector3 middleAngle = new Vector3(85,0,0); // 球棒中间的旋转角度, 像挥棒的中段
    Vector3 endAngle = new Vector3(-60,0,0);   // 球棒结束的旋转角度, 可以根据需要调整
    [SerializeField]string EffAddress = "SwordSlash12BatEff";
        
    //Delay时间
    float DelayDuration =  1f;
    //挥舞时间
    float HuiWuDuration = 0.2f;
    //恢复时间
    float RecoverDuration = 2f;
    Sequence swingSequence;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        swingSequence = DOTween.Sequence();
    }

    public override void SetDetectShootOn(Transform AttackEffTrans)
    {
        swingSequence = DOTween.Sequence();
        
        swingSequence.Append(transform.DOLocalRotate(startAngle, 0f));
        swingSequence.Append(transform.DOLocalRotate(middle1Angle, DelayDuration));
        swingSequence.Append(transform.DOLocalRotate(middleAngle, HuiWuDuration).SetEase(Ease.OutQuad)); 
        swingSequence.Append(transform.DOLocalRotate(endAngle, RecoverDuration).SetEase(Ease.InQuad)); 
        //petWeapon.SetDetectShootOn();
        DOVirtual.DelayedCall(DelayDuration,()=>//DelayDuration+HuiWuDuration-0.5f,()=>
        {
            SetColliderOn();
        });
        DOVirtual.DelayedCall(DelayDuration+HuiWuDuration,()=>
        {
            SetColliderOff();
        });
        
        if (EffAddress != "null")
        {
            GameObject effPrefab = Addressables.LoadAssetAsync<GameObject>(EffAddress).WaitForCompletion();
            
            GameObject effIns = Instantiate(effPrefab,AttackEffTrans.position, AttackEffTrans.rotation);    
            effIns.transform.parent = AttackEffTrans;
            //GameObject effIns = Instantiate(effPrefab, AttackEff.transform.position, AttackEff.transform.rotation);
            // effPrefab.transform.position = AttackEff.transform.position;
            // effPrefab.transform.rotation = AttackEff.transform.rotation; 
            // effPrefab.SetActive(true);
            DOVirtual.DelayedCall(DelayDuration+HuiWuDuration+RecoverDuration+3,()=>
            {
                Destroy(effIns);
                swingSequence.Kill();
            });
        }
    }

    void SetColliderOn()
    {
        boxCollider.enabled = true;
    }
    void SetColliderOff()
    {
        boxCollider.enabled = false;
        if(hitEff != null)
        {
            hitEff.SetActive(true);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                hitEff.SetActive(false);
            });  
        }
    }
}
