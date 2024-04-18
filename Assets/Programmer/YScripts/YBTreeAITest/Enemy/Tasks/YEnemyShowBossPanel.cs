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
        public override void OnStart()
        {
            bossPanel = enemyBT.bossPanel;
            Show();
        }

        private void Show()
        {
            bossPanel.SetActive(isShow);
            
        }
    }
}
