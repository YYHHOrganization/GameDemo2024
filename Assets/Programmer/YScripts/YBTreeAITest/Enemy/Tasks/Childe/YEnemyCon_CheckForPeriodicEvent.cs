using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Core.AI;
using UnityEngine;


public class YEnemyCon_CheckForPeriodicEvent : YBTEnemyConditional
{
    public float interval = 3f;//意思是每隔3秒执行一次么，就是让他先走，每3s会执行一次fight？
    public SharedFloat PeriodicTimer;//这个是用来记录时间的
    public override TaskStatus OnUpdate()
    {
        PeriodicTimer.Value += Time.deltaTime;
        if (PeriodicTimer.Value > interval)
        {
            // PeriodicTimer.Value = 0; 后面需配备一个setfloat节点 fight之后会重置这个值
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}