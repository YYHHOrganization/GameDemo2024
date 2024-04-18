using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Core.AI
{
    public class YEnemyDestroyBoss : YBTEnemyAction
    {
        public float bleedTime = 1.0f;//流血时间
        public GameObject DieEffect;
        public GameObject explodeEffect;
        
        private bool isDestroy = false;
        public override void OnStart()
        {
            Debug.Log("Destroy Boss");
            //出现也给timeline 给boss一个运镜
            //message显示
            if (DieEffect != null)
            {
                GameObject dieEffect = Object.Instantiate(DieEffect, transform.position, Quaternion.identity);
                Object.Destroy(dieEffect, 1.0f);
            }
            DOVirtual.DelayedCall(bleedTime,(() =>
            {
                if (explodeEffect != null)
                {
                    GameObject explode = Object.Instantiate(explodeEffect, transform.position, Quaternion.identity);
                    Object.Destroy(explode, 1.0f);
                }
                HRogueCameraManager.Instance.ShakeCamera(0.5f, 0.5f);
                isDestroy = true;
                Object.Destroy(gameObject);
            }));
            
            //血条消失
            enemyBT.curHealth = 0;
            
        }

        public override TaskStatus OnUpdate()
        {
            return isDestroy? TaskStatus.Success : TaskStatus.Running;
        }
    }
}