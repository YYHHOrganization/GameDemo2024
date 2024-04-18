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
    public class YEnemyBenderAttack : YBTEnemyAction
    {
        public override void OnStart()
        {
            enemyBT.waterBenderController.Attack(player.transform.position);
        }
    }
}