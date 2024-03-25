using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YInteractElevator : YIInteractiveGroup
{
    public GameObject elevator;
    //目的
    public Transform target;
    public float maxSpeed = 6f; // 最大速度
    public float acceleration = 2f; // 加速度
    public float deceleration = 2f; // 减速度
    
    //如果已经移动到上面了，就不再移动，再点击就是下降？ 或者两个按钮 一个上升一个下降
    bool isUp = false;
    
    YMyUtilityClass.SmoothMover smoothMover;
    //write start
    public override void Start()
    {
        base.Start();
        smoothMover = elevator.AddComponent<YMyUtilityClass.SmoothMover>();
    }
    YMyUtilityClass.SmoothMover smoothMoverPlayer;
    public override void SetResultOn()
    {
        //电梯上升
        Debug.Log("电梯上升");
        HAudioManager.Instance.Play("ElevatorUpAudio", smoothMover.gameObject);

        smoothMover.setSmoothMover(elevator.transform, elevator.transform.localPosition, target.localPosition, maxSpeed, acceleration, deceleration);
        smoothMover.StartMoving();
        
        //test player上升
        
        if(player==null)
            return;
        player.transform.parent = gameObject.transform;
        smoothMoverPlayer = player.AddComponent<YMyUtilityClass.SmoothMover>();
        smoothMoverPlayer.setSmoothMover(player.transform, player.transform.localPosition, target.localPosition, 
            maxSpeed, acceleration, deceleration,true);
        smoothMoverPlayer.StartMoving();
        
    }

    GameObject player;
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("什么东西进入电梯");
    //     if (other.CompareTag("Player"))
    //     {
    //         Debug.Log("角色进入电梯");
    //         player = other.gameObject;
    //         //player.transform.position = elevator.transform.position;
    //         player.transform.parent = elevator.transform;
    //     }
    // }
    
    public void PlayerEnterElevator(GameObject player)
    {
        Debug.Log("角色进入电梯*********");
        this.player = player;
        player.transform.parent = gameObject.transform;
    }
   public void PlayerExitElevator()
    {
        //移除父节点
        player.transform.parent = null;
        //移除组件
        Destroy(smoothMoverPlayer);
        //移除对player的引用
        player = null;
    }

    public override void SetResultOff()
    {
        //电梯下降
    }
    private float currentSpeed = 0f; // 当前速度
    private float distanceToTarget; // 与目标位置的距离--实时更新
    private bool isMoving = false; // 是否正在移动
    private Vector3 startPosition; // 起始位置
    Vector3 targetPosition; // 目标位置
    private float startTime; // 开始时间
    
    
    private float distanceOrigin;

    private float accelerationTime;
    private float decelerationTime;
    private float uniformTime;
    public void StartMoving(GameObject moveObj, Transform target, float maxSpeed, float acceleration, float deceleration)
    {
        //Debug.Log("开始运动");
        isMoving = true;
        startTime = Time.time;

        startPosition = moveObj.transform.position;

        targetPosition = target.position;
        distanceToTarget = Vector3.Distance(startPosition, targetPosition);
        
        distanceOrigin = distanceToTarget;
        
        accelerationTime = maxSpeed / acceleration;
        decelerationTime = maxSpeed / deceleration;
        uniformTime = (distanceOrigin - 0.5f * accelerationTime * accelerationTime * acceleration -
                             (maxSpeed * decelerationTime -
                              0.5f * decelerationTime * decelerationTime * deceleration)) / maxSpeed;
        
    }
    
    void Update()
    {
        //sSetElevatorMovement();
    }
    public void SetElevatorMovement()
    {
        if (isMoving)
        {
            //实时更新距离目标的位置
            distanceToTarget = Vector3.Distance(elevator.transform.position,targetPosition);
            //Debug.Log("distanceToTarget"+distanceToTarget);
            if (distanceToTarget<=0.1f)
            {
                elevator.transform.position =targetPosition;
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
            Vector3 direction = (targetPosition - startPosition).normalized;
            elevator.transform.Translate(direction * currentSpeed * Time.deltaTime);

        }
    }
    
}
