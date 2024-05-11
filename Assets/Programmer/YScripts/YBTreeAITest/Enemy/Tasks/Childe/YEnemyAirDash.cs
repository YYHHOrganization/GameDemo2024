
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;

namespace Core.AI
{
    public class YEnemyAirDash : YBTEnemyAction
    {
        public float dashForce = 5;
        public float jumpForce = 10;
        
        public float buildUpTime;
        public float jumpTime;
        public float dashTime;

        public string animationTriggerName;
        
        public bool needBuildUp;
        public string animationBuildUpTriggerName;
        
        public bool shakeCameraOnLanding;

        private bool isLanded;
        
        private Tween buildupTween;
        private Tween jumpTween;
        private Tween dashTween;
        
        // public GameObject jumpEffect;
        // public Vector3 jumpEffectOffset;
        public override void OnStart()
        {
            //先跳起来
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            //空中准备和停顿一下 再开始dash
            jumpTween = DOVirtual.DelayedCall(jumpTime,StartBuildUp,false);//意思是在buildUpTime秒后调用StartJump方法
            
            if(needBuildUp)
            {
                animator.SetBool(animationBuildUpTriggerName,true);
                animator.SetTrigger(animationBuildUpTriggerName);
            }
            else
            {
                animator.SetTrigger(animationTriggerName);
                animator.SetBool(animationTriggerName,true);
            }
        }
        void StartBuildUp()
        {
            rb.useGravity = false;
            buildupTween = DOVirtual.DelayedCall(buildUpTime,StartDash,false);//意思是在buildUpTime秒后调用StartJump方法
        }
        private void StartDash()
        {
            rb.useGravity = true;
            
            if(needBuildUp&&!string.IsNullOrEmpty(animationTriggerName))
            {
                animator.SetTrigger(animationTriggerName);
                animator.SetBool(animationTriggerName,true);
            }
            // var direction = (player.transform.position - transform.position).normalized;
             rb.velocity = Vector3.zero;
            //转向角色
            if(player == null)
            {
                // player = GameObject.FindGameObjectWithTag("Player");//不一定是player，也可能是其他的目标，先测试
                player = YPlayModeController.Instance.curCharacter;
                if (player==null)
                {
                    
                }
                
                else
                {
                    Debug.Log("player"+player);
                    //看向角色，只绕着y轴
                    transform.LookAt(player.transform);
                    transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
                    //transform.LookAt(player.transform);
                    
                    //给角色一个向上同时往player方向的力
                    var direction = (player.transform.position - transform.position).normalized;
                    rb.AddForce(direction * dashForce, ForceMode.Impulse);
                }
            }
            else
            {
                transform.LookAt(player.transform);
                transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
                //给角色一个向上同时往player方向的力
                var direction = (player.transform.position - transform.position).normalized;
                rb.AddForce(direction * dashForce, ForceMode.Impulse);
            }
            
            
            dashTween = DOVirtual.DelayedCall(dashTime,() =>
            {
                isLanded = true;
                rb.velocity = Vector3.zero;
                if (shakeCameraOnLanding)
                {
                    HRogueCameraManager.Instance.ShakeCamera(0.5f, 0.2f);
                }
            },false);
            
        }
        
        public override TaskStatus OnUpdate()
        {
            //test
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
            return isLanded? TaskStatus.Success : TaskStatus.Running;
        }
        public override void OnEnd()
        {
            buildupTween?.Kill();
            jumpTween?.Kill();//结束时清除
            dashTween?.Kill();
            isLanded = false;
            //保持面向方向不变，不绕着xz方向旋转
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
            animator.SetBool(animationTriggerName,false);
            rb.useGravity = true;
        }
    }

}