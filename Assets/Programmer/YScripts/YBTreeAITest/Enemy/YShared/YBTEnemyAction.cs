using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class YBTEnemyAction : Action
    {
        protected Rigidbody rb;
        protected Animator animator;
        protected GameObject player;
        protected Transform shootOrigin;
        protected YEnemyBT enemyBT;
        public override void OnAwake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            //player = GameObject.FindGameObjectWithTag("Player");//不一定是player，也可能是其他的目标，先测试
            player = YPlayModeController.Instance.curCharacter;
            Debug.Log("player"+player);
            
            shootOrigin = transform.Find("ShootOrigin");
            enemyBT = GetComponent<YEnemyBT>();
        }
        public override TaskStatus OnUpdate()
        {
            if(player == null)
            {
                //player = GameObject.FindGameObjectWithTag("Player");//不一定是player，也可能是其他的目标，先测试
                player = YPlayModeController.Instance.curCharacter;
                if (player==null)
                {
                    return TaskStatus.Success;
                }
                
                else
                {
                    Debug.Log("player"+player);
                    OnUpdateEnemy();
                    return TaskStatus.Success;
                }
            }
            else
            {
                OnUpdateEnemy();
                return TaskStatus.Success;
            }

        }

        protected virtual void OnUpdateEnemy()
        {
            
        }
    }

}