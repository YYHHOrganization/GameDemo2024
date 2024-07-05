using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class YEnemyUICanvas : MonoBehaviour
{
    [Header("红色血条")]
    public Image enemyHealthImage;
    [Header("中间过程的黄色血条 为了视觉效果")]
    public Image enemyMidTempHealthImage;//中间过程的黄色血条 为了视觉效果

    [SerializeField]string AddressablePath = "YEnemyXieTiaoEff";
    [SerializeField]Transform VFXEffPos0;
    [SerializeField]Transform VFXEffPos1;

    private GameObject eff0;
    private void Start()
    {
        //位置设置为pos，是相对的，并挂在这个canvas下
        eff0 = Addressables.LoadAssetAsync<GameObject>(AddressablePath).WaitForCompletion() ;
    }

    public void UpdateEnemyHeathUI(int health, int maxHealth)
    {
        //enemyHealthImage.fillAmount
        float finalFillAmount = health * 1.0f / maxHealth;
        if (enemyHealthImage)
        {
            enemyHealthImage.fillAmount = finalFillAmount;
        }
        //这个血条是为了视觉效果，当怪物受到伤害的时候，会先将目前血条和最终血条之间的差值用黄色血条表示，然后再慢慢变成最终血条
        if (enemyMidTempHealthImage)
        {
            enemyMidTempHealthImage.DOFillAmount(finalFillAmount, 0.5f);
        }
        
        //生成的粒子位置是-0.8-0.8
        UpdateEnemyHealthParticle(finalFillAmount );
    }
    void UpdateEnemyHealthParticle(float finalFillAmount)
    {
        //哎 这里最好用对象池吧 后面改一下
        //生成的粒子位置是-0.8-0.8,根据finalFillAmount来决定生成的粒子位置比例
        Vector3 pos = Vector3.Lerp(VFXEffPos0.position, VFXEffPos1.position, finalFillAmount);
        if (eff0 == null)
        {
            eff0 = Addressables.LoadAssetAsync<GameObject>(AddressablePath).WaitForCompletion();
        }
        GameObject eff = Instantiate(eff0);
        eff.transform.parent = transform;
        eff.transform.position = pos;
        Destroy(eff, 1.0f);
    }
   
}
