using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;

namespace Core.AI
{
    public class YEnemyGossamerStorm : YBTEnemyAction
    {
        public float airborneChance = 0.5f;//空中概率
        public float jumpForce = 10;
        public float jumpTime;
        
        public float buildupTime;
        public float stormTime;
        public float jumpDownTime = 0.2f;

        public SharedString animationTriggerName;
        
        public bool needBuildUp;
        public SharedString animationBuildUpAirTriggerName;
        // public string animationBuildUpTriggerName;
        public SharedString animationBuildUpTriggerName;
        
        public bool shakeCameraOnLanding;

        private bool isLanded;
        private bool isAirborner;//是否空中
        
        private Tween buildupTween;
        private Tween jumpTween;
        private Tween stormTween;
        Tween jumpDownTween;

        private GameObject stormObject;
        public GameObject stormPrefab;
        
        // public GameObject jumpEffect;
        // public Vector3 jumpEffectOffset;
        public override void OnStart()
        {
            isAirborner = Random.value < airborneChance;
            if(needBuildUp)
            {
                animator.SetTrigger(isAirborner?animationBuildUpAirTriggerName.Value:animationBuildUpTriggerName.Value);
            }
            else
            {
                animator.SetTrigger(animationTriggerName.Value);
                animator.SetBool(animationTriggerName.Value,true);
            }

            if (isAirborner)
            {
                //先跳起来
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                //空中准备和停顿一下 再开始dash
                jumpTween = DOVirtual.DelayedCall(jumpTime,StartBuildUp,false);//意思是在buildUpTime秒后调用StartJump方法
            }
            else
            {
                StartBuildUp();
            }
            
        }
        void StartBuildUp()
        {
            rb.useGravity = false;
            buildupTween = DOVirtual.DelayedCall(buildupTime,StartStorm,false);//意思是在buildUpTime秒后调用StartJump方法
        }

        private void StartStorm()
        {
            if(needBuildUp&&!string.IsNullOrEmpty(animationTriggerName.Value))
            {
                animator.SetTrigger(animationTriggerName.Value);
                animator.SetBool(animationTriggerName.Value,true);
            }

            stormObject = Object.Instantiate(stormPrefab, transform);
            var direction = (player.transform.position - transform.position).normalized;
            rb.velocity = Vector3.zero;
            HRogueCameraManager.Instance.ShakeCamera(0.5f, 0.2f);
            
            stormTween = DOVirtual.DelayedCall(stormTime,() =>
            {
                isLanded = true;
                rb.velocity = Vector3.zero;
                if (shakeCameraOnLanding)
                {
                    
                    rb.useGravity = true;
                    Object.Destroy(stormObject);
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
            stormTween?.Kill();
            isLanded = false;
            //保持面向方向不变，不绕着xz方向旋转
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
            
            animator.SetBool(animationTriggerName.Value,false);
            rb.useGravity = true;
            Object.Destroy(stormObject);

            
        }
    }

}