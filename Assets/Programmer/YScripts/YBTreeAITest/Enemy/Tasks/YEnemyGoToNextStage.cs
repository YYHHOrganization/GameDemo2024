using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{

    public class YEnemyGoToNextStage : YBTEnemyAction
    {
        public SharedInt curStage;
        public override void OnStart()
        {
            curStage.Value++;
        }
        // public override TaskStatus OnUpdate()
        // {
        //     base.OnUpdate();
        // }
    }
}