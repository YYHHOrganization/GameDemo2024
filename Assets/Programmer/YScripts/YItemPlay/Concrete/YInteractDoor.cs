using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YInteractDoor : YIInteractiveGroup
{
    public GameObject doorLeft;
    public GameObject doorRight;
    
    public Transform targetLeft;
    public Transform targetRight;
    
    //存储门初始位置
    public Vector3 doorLeftOrigin;
    public Vector3 doorRightOrigin;
    //目的
   
    public float maxSpeed = 6f; // 最大速度
    public float acceleration = 2f; // 加速度
    public float deceleration = 2f; // 减速度
    
    //如果已经移动到上面了，就不再移动，再点击就是下降？ 或者两个按钮 一个上升一个下降
    bool isUp = false;
    
    YMyUtilityClass.SmoothMover smoothMoverLeft;
    YMyUtilityClass.SmoothMover smoothMoverRight;
    //write start
    public override void Start()
    {
        base.Start();
        smoothMoverLeft = doorLeft.AddComponent<YMyUtilityClass.SmoothMover>();
        smoothMoverRight = doorRight.AddComponent<YMyUtilityClass.SmoothMover>();
        // doorLeftOrigin = doorLeft.transform.position;
        // doorRightOrigin = doorRight.transform.position;
        doorLeftOrigin = doorLeft.transform.localPosition;
        doorRightOrigin = doorRight.transform.localPosition;
        
        
    }
    public override void SetResultOn()
    {
        Debug.Log("门开启");
        // smoothMoverLeft.setSmoothMover(doorLeft.transform, doorLeft.transform.position, targetLeft.position, maxSpeed, acceleration, deceleration);
        smoothMoverLeft.setSmoothMover(doorLeft.transform, doorLeft.transform.localPosition, targetLeft.localPosition, maxSpeed, acceleration, deceleration);
        smoothMoverLeft.StartMoving();
        // smoothMoverRight.setSmoothMover(doorRight.transform, doorRight.transform.position, targetRight.position, maxSpeed, acceleration, deceleration);
        smoothMoverRight.setSmoothMover(doorRight.transform, doorRight.transform.localPosition, targetRight.localPosition, maxSpeed, acceleration, deceleration);
        smoothMoverRight.StartMoving();
        //YMyUtilityClass.SmoothMove(elevator.transform, elevator.transform.position, target.position, maxSpeed, acceleration, deceleration);
        
        
    }
    public override void SetResultOff()
    {
        Debug.Log("门关闭");
        // smoothMoverLeft.setSmoothMover(doorLeft.transform, doorLeft.transform.position, doorLeftOrigin, maxSpeed, acceleration, deceleration);
        // smoothMoverLeft.StartMoving();
        // smoothMoverRight.setSmoothMover(doorRight.transform, doorRight.transform.position, doorRightOrigin, maxSpeed, acceleration, deceleration);
        // smoothMoverRight.StartMoving();
        
        smoothMoverLeft.setSmoothMover(doorLeft.transform, doorLeft.transform.localPosition, doorLeftOrigin, maxSpeed, acceleration, deceleration);
        smoothMoverLeft.StartMoving();
        smoothMoverRight.setSmoothMover(doorRight.transform, doorRight.transform.localPosition, doorRightOrigin, maxSpeed, acceleration, deceleration);
        smoothMoverRight.StartMoving();
        
    }
}
