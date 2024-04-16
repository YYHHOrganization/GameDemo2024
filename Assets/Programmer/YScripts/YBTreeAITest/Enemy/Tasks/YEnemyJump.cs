using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;

namespace Core.AI
{
    
    public class YEnemyJump : YBTEnemyAction
    {
        public float horizontalForce = 5;
        public float jumpForce = 10;
        
        public float buildUpTime;
        public float jumpTime;

        public string animationTriggerName;
        public bool shakeCameraOnLanding;

        private bool isLanded;
        
        private Tween buildupTween;
        private Tween jumpTween;
        public override void OnStart()
        {
            buildupTween = DOVirtual.DelayedCall(buildUpTime,StartJump,false);//意思是在buildUpTime秒后调用StartJump方法
            animator.SetBool(animationTriggerName,true);
        }
        
        private void StartJump()
        {
            // var direction = (player.transform.position - transform.position).normalized;
             
            //转向角色
            if(player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");//不一定是player，也可能是其他的目标，先测试
                if (player==null)
                {
                    
                }
                
                else
                {
                    Debug.Log("player"+player);
                    transform.LookAt(player.transform);
                    //给角色一个向上同时往player方向的力
                    var direction = (player.transform.position - transform.position).normalized;
                    rb.AddForce(direction * horizontalForce, ForceMode.Impulse);
                }
            }
            else
            {
                transform.LookAt(player.transform);
                //给角色一个向上同时往player方向的力
                var direction = (player.transform.position - transform.position).normalized;
                rb.AddForce(direction * horizontalForce, ForceMode.Impulse);
            }
            
            
            
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            jumpTween = DOVirtual.DelayedCall(jumpTime,() =>
            {
                isLanded = true;
                if (shakeCameraOnLanding)
                {
                    HRogueCameraManager.Instance.ShakeCamera(0.5f, 0.2f);
                }
            },false);
            
        }
        
        public override TaskStatus OnUpdate()
        {
            return isLanded? TaskStatus.Success : TaskStatus.Running;
        }
        public override void OnEnd()
        {
            buildupTween?.Kill();
            jumpTween?.Kill();//结束时清除
            isLanded = false;
            animator.SetBool(animationTriggerName,false);
        }
    }

}