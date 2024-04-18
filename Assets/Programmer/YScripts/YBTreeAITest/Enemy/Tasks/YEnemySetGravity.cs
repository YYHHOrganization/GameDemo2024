using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class YEnemySetGravity : YBTEnemyAction
    {
        public bool isGravity = true;
        public override void OnStart()
        {
            base.OnStart();
            if (isGravity)
            {
                rb.useGravity = true;
            }
            else
            {
                rb.useGravity = false;
            }
        }
    }

}
    
