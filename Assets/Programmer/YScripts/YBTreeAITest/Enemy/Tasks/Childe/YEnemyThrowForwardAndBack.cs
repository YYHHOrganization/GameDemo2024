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
    public class YEnemyThrowForwardAndBack : YBTEnemyAction
    {
        public float throwDistance = 10;
        
        public float buildupTime;
        public float throwTime;
        public float retractTime;

        public SharedString animationTriggerName;
        
        public bool needBuildUp;
        public SharedString animationBuildUpAirTriggerName;
        // public string animationBuildUpTriggerName;
        public SharedString animationBuildUpTriggerName;
        
        public bool shakeCameraOnLanding;

        private bool isLanded;
        private bool isAirborner;//是否空中
        
        private Tween buildupTween;
        private List<Tween> throwTweens = new List<Tween>();
        //投掷鱼回收的向外偏移量
        public float  throwOffset = 1f;
        
        public GameObject throwThPrefab;
        
        //投掷个数
        public int throwCount=1;
        
        // public GameObject jumpEffect;
        // public Vector3 jumpEffectOffset;
        public override void OnStart()
        {
            buildupTween = DOVirtual.DelayedCall(buildupTime,StartThrow,false);//意思是在buildUpTime秒后调用StartJump方法
            
            if(needBuildUp)
            {
                animator.SetTrigger(isAirborner?animationBuildUpAirTriggerName.Value:animationBuildUpTriggerName.Value);
            }
            else
            {
                animator.SetTrigger(animationTriggerName.Value);
                animator.SetBool(animationTriggerName.Value,true);
            }
            
        }
        private void StartThrow()
        {
            if(needBuildUp&&!string.IsNullOrEmpty(animationTriggerName.Value))
            {
                animator.SetTrigger(animationTriggerName.Value);
                animator.SetBool(animationTriggerName.Value,true);
            }

            // throwThObject = Object.Instantiate(throwThPrefab, transform);
            // // var direction = (player.transform.position - transform.position).normalized;
            //  rb.velocity = Vector3.zero;
            //  HRogueCameraManager.Instance.ShakeCamera(0.5f, 0.2f);
             
             // throwTween = DOTween.Sequence()
             //     .Append(throwThObject.transform.DOMove(transform.position + transform.forward * throwDistance, throwTime)).SetEase(Ease.OutQuad)
             //     .Append(throwThObject.transform.DOMove(transform.position, retractTime)).SetEase(Ease.InQuad)
             //     .AppendCallback(() =>
             //     {
             //         isLanded = true;
             //         rb.velocity = Vector3.zero;
             //         if (shakeCameraOnLanding)
             //         {
             //             Object.Destroy(throwThObject);
             //         }
             //     });
             
             //投掷多个。同时，且在boss前呈现扇形
             
             // 定义扇形的角度
             
             float angle = 0f;
             for (int i = 1; i <= throwCount; i++)
             {
                GameObject throwThObject = Object.Instantiate(throwThPrefab, transform);
                //比前方forward更往偏移一点
                // Vector3 throwThPosition = transform.position + transform.forward * throwOffset;
                // GameObject throwThObject = Object.Instantiate(throwThPrefab, throwThPosition, Quaternion.identity);

                 // 计算角度
                 
                 if(throwCount>1)
                 {
                     float singleAngle = 60f / (throwCount - 1);
                     angle = (i - 1) * singleAngle - 30;
                 }
                 else
                 {
                     angle = 0;
                 }
                    
                    
                 // 计算投掷方向
                 Vector3 throwDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

                 // 设置刚体的速度为投掷方向乘以速度
                 // rb.velocity = throwDirection * throwSpeed;

                 // 创建一个新的DOTween序列
                 Tween throwTween = DOTween.Sequence()
                     .Append(throwThObject.transform.DOMove(transform.position + throwDirection * throwDistance, throwTime)).SetEase(Ease.OutQuad)
                     .Append(throwThObject.transform.DOMove(transform.position, retractTime)).SetEase(Ease.InQuad)
                     .AppendCallback(() =>
                     {
                         isLanded = true;
                         rb.velocity = Vector3.zero;
                         Object.Destroy(throwThObject);
                     });
                 throwTweens.Add(throwTween);
             }

             HRogueCameraManager.Instance.ShakeCamera(0.5f, 0.2f);
        
            
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
            // throwTween?.Kill();
            foreach (var tween in throwTweens)
            {
                tween?.Kill();
            }
            isLanded = false;
            //保持面向方向不变，不绕着xz方向旋转
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
            animator.SetBool(animationTriggerName.Value,false);
            // Object.Destroy(throwThObject);
        }
    }

}