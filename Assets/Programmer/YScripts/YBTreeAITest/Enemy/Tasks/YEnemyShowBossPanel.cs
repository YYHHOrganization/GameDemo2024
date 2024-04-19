using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Core.AI
{
    public class YEnemyShowBossPanel : YBTEnemyAction
    {
        public GameObject bossPanel;
        public bool isShow;
        private GameObject imageKuang;
        GameObject BossName;
        public override void OnStart()
        {
            bossPanel = enemyBT.bossPanel;
            imageKuang = bossPanel.transform.Find("imageKuang").gameObject;
            BossName = bossPanel.transform.Find("BossName").gameObject;
            BossName.SetActive(false);
            Show();
        }

        private void Show()
        {
            bossPanel.SetActive(isShow);
            imageKuang.transform.DOScale( new Vector3(1.0f, 1.0f, 1.0f), 0.3f);
            DOVirtual.DelayedCall(0.3f, () =>
            {
                BossName.SetActive(true);
                BossName.GetComponent<CanvasGroup>().DOFade(1.0f, 1f);
            });
            
            // bossPanel.transform.DOScale(new Vector3(0.9f, 0.9f, 1.0f), 0.2f).From(true);
            // bossPanel.GetComponent<CanvasGroup>().DOFade(1.0f, 0.2f);


        }
    }
}
