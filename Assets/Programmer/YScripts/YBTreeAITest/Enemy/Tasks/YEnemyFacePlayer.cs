using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;

namespace Core.AI
{
    public class YEnemyFacePlayer : YBTEnemyAction
    {
        protected override void OnUpdateEnemy()
        {
            transform.LookAt(player.transform);
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
        }
    }

}