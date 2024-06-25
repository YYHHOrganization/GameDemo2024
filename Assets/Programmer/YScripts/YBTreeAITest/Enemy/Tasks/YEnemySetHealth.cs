using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AI
{
    
    public class YEnemySetHealth : YBTEnemyAction
    {
        public SharedInt Health;
        public override TaskStatus OnUpdate()
        {
            enemyBT.curHealth = Health.Value;
            enemyBT.UpdateUI(Health.Value);
            return TaskStatus.Success;
        }
    }

}