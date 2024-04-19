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
    public class YEnemyCreateWaterBall : YBTEnemyAction
    {
        public int waterBallCount = 5;
        public override void OnStart()
        {
            enemyBT.waterBallController.BossCreateWaterBall(waterBallCount);
        }
    }
}
