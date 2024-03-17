using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YMyUtilityClass
{
    public static void SmoothMove(Transform objectTransform, Vector3 start, Vector3 end, float maxSpeed, float acceleration, float deceleration)
    {
        //SmoothMover smoothMover = new SmoothMover(objectTransform, start, end, maxSpeed, acceleration, deceleration);
        SmoothMover smoothMover = objectTransform.gameObject.AddComponent<SmoothMover>();
        
        smoothMover.setSmoothMover(objectTransform, start, end, maxSpeed, acceleration, deceleration);
        smoothMover.StartMoving();
    }

    public class SmoothMover:MonoBehaviour
    {
        private Transform objectTransform;
        private Vector3 start;
        private Vector3 end;
        private float maxSpeed;
        private float acceleration;
        private float deceleration;
        private float startTime;
        private float distanceToTarget;
        private bool isMoving;
        
        private float distanceOrigin;
        private float accelerationTime;
        private float decelerationTime;
        private float uniformTime;
        
        private float currentSpeed = 0f; // 当前速度

        public SmoothMover(Transform objectTransform, Vector3 start, Vector3 end, float maxSpeed, float acceleration, float deceleration)
        {
            this.objectTransform = objectTransform;
            this.start = start;
            this.end = end;
            this.maxSpeed = maxSpeed;
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            
            
        }
        public void setSmoothMover(Transform objectTransform, Vector3 start, Vector3 end, float maxSpeed, float acceleration, float deceleration)
        {
            this.objectTransform = objectTransform;
            this.start = start;
            this.end = end;
            this.maxSpeed = maxSpeed;
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            
            
        }

        public void StartMoving()
        {
            startTime = Time.time;
            distanceToTarget = Vector3.Distance(start, end);
            isMoving = true;
            
            distanceOrigin = distanceToTarget;
            accelerationTime = maxSpeed / acceleration;
            decelerationTime = maxSpeed / deceleration;
            uniformTime = (distanceOrigin - 0.5f * accelerationTime * accelerationTime * acceleration -
                           (maxSpeed * decelerationTime -
                            0.5f * decelerationTime * decelerationTime * deceleration)) / maxSpeed;

        }

        public void Update()
        {
            if (isMoving)
            {
                //实时更新距离目标的位置
                // distanceToTarget = Vector3.Distance(objectTransform.position,end);
                distanceToTarget = Vector3.Distance(objectTransform.localPosition,end);
                //Debug.Log("distanceToTarget"+distanceToTarget);
                if (distanceToTarget<=0.1f)
                {
                    // objectTransform.position = end;
                    objectTransform.localPosition = end;
                    isMoving = false;
                    //Debug.Log("到达目的地");
                    return;
                }
                // 计算当前移动时间
                float currentTime = Time.time - startTime;

                // 计算加速段、匀速段和减速段的时间
            
                // 根据时间和距离计算当前速度
                if (currentTime < accelerationTime)
                {
                    //Debug.Log("加速段");
                    // 加速段
                    currentSpeed = acceleration * currentTime;
                }
                else if (currentTime < accelerationTime + uniformTime)
                {
                    //Debug.Log("匀速段");
                    // 匀速段
                    currentSpeed = maxSpeed;
                }
                else
                {
                    // 减速段
                    currentSpeed = maxSpeed - deceleration * (currentTime - accelerationTime - uniformTime);
                    //Debug.Log("减速段"+currentSpeed);
                }
                Vector3 direction = (end - start).normalized;
                //objectTransform.Translate(direction * currentSpeed * Time.deltaTime);
                // objectTransform.position += direction * currentSpeed * Time.deltaTime;
                objectTransform.localPosition += direction * currentSpeed * Time.deltaTime;

            }
        }
    }
}
