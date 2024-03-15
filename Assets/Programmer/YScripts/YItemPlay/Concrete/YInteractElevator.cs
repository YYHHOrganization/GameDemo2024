using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YInteractElevator : YIInteractiveGroup
{
    public GameObject elevator;
    //目的
    public Transform target;
    public override void SetResultOn()
    {
        //电梯上升
        Debug.Log("电梯上升");
        ElevatorUp();
    }
    public override void SetResultOff()
    {
        //电梯下降
        
    }
    private void ElevatorUp()
    {
        //电梯上升的逻辑
        // 让电梯上升 直到终点，且速度应该是从0开始加速到最大速度，然后减速到0
        // 电梯上升的逻辑：
        // Vector3 direction = target.position - elevator.transform.position;
        // float distance = direction.magnitude;
        // float speed = 2f; // 设置电梯上升速度
        //StartCoroutine(MoveElevator(direction, distance, speed));
        
        StartMoving();
    }
    
    
    private float currentSpeed = 0f; // 当前速度
    private float distanceToTarget; // 车辆与目标位置的距离
    private bool isMoving = false; // 是否正在移动
    private Vector3 startPosition; // 起始位置
    private float startTime; // 开始时间
    private float maxSpeed = 10f; // 最大速度
    private float acceleration = 8f; // 加速度
    private float deceleration = 8f; // 减速度
    public void StartMoving()
    {
        //Debug.Log("开始运动");
        isMoving = true;
        startTime = Time.time;
        startPosition = transform.localPosition;
        distanceToTarget = Vector3.Distance(startPosition, target.position);
    }
    void Update()
    {
        SetCarMovement();
    }
    public void SetCarMovement()
    {
        if (isMoving)
        {
            distanceToTarget = Vector3.Distance(transform.localPosition,target.position);
            if (distanceToTarget<=0.1f)
            {
                transform.localPosition =target.position;
                isMoving = false;
                return;
            }
            // 计算当前移动时间
            float currentTime = Time.time - startTime;

            // 计算加速段、匀速段和减速段的时间
            float accelerationTime = maxSpeed / acceleration;
            float decelerationTime = maxSpeed / deceleration;
            float uniformTime = (distanceToTarget - accelerationTime*accelerationTime / 2 - decelerationTime*decelerationTime / 2) / maxSpeed;
       
            // 根据时间和距离计算当前速度
            if (currentTime < accelerationTime)
            {
                // 加速段
                currentSpeed = Mathf.Lerp(0f, maxSpeed, currentTime / accelerationTime);
            }
            else if (currentTime < accelerationTime + uniformTime)
            {
                // 匀速段
                currentSpeed = maxSpeed;
            }
            else
            {
                // 减速段
                currentSpeed = Mathf.Lerp(maxSpeed, 0f, (currentTime - accelerationTime - uniformTime) / decelerationTime);
            }
            // Vector3 direction = (GetVatPos(targetIndex) - startPosition).normalized;
            Vector3 direction = (target.position - startPosition).normalized;
            elevator.transform.Translate(direction * currentSpeed * Time.deltaTime);

        }
    }

    
    // private IEnumerator MoveElevator(Vector3 direction, float distance, float speed)
    // {
    //     
    //     float startTime = Time.time;
    //     float duration = distance / speed;
    //     
    //     while (Time.time < startTime + duration)
    //     {
    //         float t = (Time.time - startTime) / duration;
    //         elevator.transform.position += direction * Time.deltaTime * speed;
    //         yield return null;
    //     }
    //     elevator.transform.position = target.position;
    //     
    // }
    //
}
