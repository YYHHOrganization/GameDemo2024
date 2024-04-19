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
    public class YEnemySpellWaterBall : YBTEnemyAction
    {
        public override void OnStart()
        {
            Vector3 pos = player.transform.position;
            enemyBT.waterBallController.ThrowWaterBall(pos);
        }
    }
}

