using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class YEnemyInitBoss : YBTEnemyAction
    {
        public string bossName;
        
        public override void OnAwake()
        {
            base.OnAwake();
            //player = GameObject.FindGameObjectWithTag("Player");//不一定是player，也可能是其他的目标，先测试
            // player = YPlayModeController.Instance.curCharacter;
            // Debug.Log("player"+player);
            // shootOrigin = transform.Find("ShootOrigin");
            // enemyBT = GetComponent<YEnemyBT>();
            // enemyBT.bossName = bossName;
            
            
        }
        // Start is called before the first frame update
        public override void OnStart()
        {
            base.OnStart();
            //出现也给timeline 给boss一个运镜
            //message显示
            
            //boss初始位置是天上
            enemyBT.transform.localPosition = enemyBT.transform.localPosition+new Vector3(0, 5, 0);
            
            enemyBT.bossPanel.SetActive(false);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

    }
}