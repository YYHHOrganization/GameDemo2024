
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;

namespace Core.AI
{
    public class YEnemyDash : YBTEnemyAction
    {
        
        public float buildUpTime;
        public float jumpTime;

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
            
            buildupTween = DOVirtual.DelayedCall(buildUpTime,StartJump,false);//意思是在buildUpTime秒后调用StartJump方法
            
            if(needBuildUp)
            {
                animator.SetBool(animationBuildUpTriggerName,true);
            }
            else
            {
                animator.SetTrigger(animationTriggerName);
                animator.SetBool(animationTriggerName,true);
            }
        }
        
        private void StartJump()
        {
            if(needBuildUp&&!string.IsNullOrEmpty(animationTriggerName))
            {
                animator.SetTrigger(animationTriggerName);
                animator.SetBool(animationTriggerName,true);
            }
            
            transform.LookAt(player.transform);
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
            //给角色一个向上同时往player方向的力
            var direction = (player.transform.position - transform.position).normalized;
            //不再使用刚体，而是直接往角色身上冲，使用dotween模拟
            
            //慢到快的setease
            dashTween = transform.DOMove(player.transform.position, jumpTime).SetEase(Ease.OutQuad);
            
            jumpTween = DOVirtual.DelayedCall(jumpTime,() =>
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
        }
    }

}