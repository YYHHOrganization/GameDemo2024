using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace Core.AI
{
    public class YBTEnemyConditional : Conditional
    {
        protected Rigidbody rb;
        protected Animator animator;
        protected GameObject player;
        protected YEnemyBT enemyBT;
        public override void OnAwake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            //player = GameObject.FindGameObjectWithTag("Player");//不一定是player，也可能是其他的目标，先测试
            player = YPlayModeController.Instance.curCharacter;
            enemyBT = GetComponent<YEnemyBT>();
        }
    }


}