using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Core.AI
{
    public class YEmemyIsHealthUnder : YBTEnemyConditional
    {
        public SharedInt healthThreshold;
        public override TaskStatus OnUpdate()
        {
            if (enemyBT.curHealth <= healthThreshold.Value)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
    
}