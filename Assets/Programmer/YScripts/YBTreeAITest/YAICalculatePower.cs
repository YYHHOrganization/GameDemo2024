using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class YAICalculatePower : Action
{
    public SharedInt power;//sharedInt这个变量是共享的，可以在不同的行为树中共享?inspector中可以看到
    
    public override TaskStatus OnUpdate()
    {
//计算power
        power.Value = 100;
        return TaskStatus.Success;
    }
}
