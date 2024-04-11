using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YWanderState : YBaseState
{
    private Vector3? mDestination;
    public YPatrolAI partrolAI;
    public float stopDis=1f;

    public Quaternion desiredRotation;
    public float turnSpeed=1f;
    public float rayDis=15f;
    private Vector3 mDirection;
    //private readonly LayerMask mlayerMask = LayerMask.NameToLayer("YWalls");
    private readonly LayerMask mlayerMask;
    private LayerMask wallLayer;
    //构造函数 同时:base(yPatrolAI.gameObject) 是必要的 给父类传输形参yPatrolAI.gameObject
    public YWanderState(YPatrolAI yPatrolAI) : base(yPatrolAI.gameObject)
    {
        partrolAI = yPatrolAI;
        wallLayer=partrolAI.mlayer;
        mlayerMask = partrolAI.mlayer;
    }
    public override Type Tick()
    {      
        //var chaseTarget = CheckForAggro();
        Transform chaseTarget=null;
        if (YPlayModeController.Instance.curCharacter)
        {
            chaseTarget = YPlayModeController.Instance.curCharacter.transform;
        }
     
        if(chaseTarget)
        {
            partrolAI.setTarget(chaseTarget);
            partrolAI.animator.SetInteger("AnimState", 1);
            if (partrolAI.SpotLightWander) partrolAI.SpotLightWander.SetActive(false);
            if (partrolAI.SpotLightChase) partrolAI.SpotLightChase.SetActive(true);
            return typeof(YChaseState);
        }
        if(mDestination.HasValue==false||
            Vector3.Distance(transform.position,mDestination.Value)<=stopDis)
        {
            FindRandomDis();
        }

        //ab之间做差值
        transform.rotation = Quaternion.Slerp(transform.rotation,desiredRotation,Time.deltaTime*turnSpeed);

        //如果角色面前有墙壁 旋转一下 争取不撞墙
        if(IsForwardBlock())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, 0.2f);
        }
        else
        {
            transform.Translate(Vector3.forward*Time.deltaTime*YGameSetting.PatrolAISpeed);
        }
        Debug.DrawRay(transform.position,mDirection*rayDis,Color.red);

        //如果前进的方向上有墙壁
        while(IsPathBlock())
        {
            FindRandomDis();
           // Debug.Log("Wall!!");
        }
        if (partrolAI.isDead == true)
        {
            return typeof(YDieState);
        }
        return null;
    }
    //每次转换回来的时候 重新找一个目的地mDestination
    public override void OnStateEnter()
    {
        // FindRandomDis();
        // partrolAI.mNavMeshAgent.enabled = false;
        
        // partrolAI.animator.SetInteger("AnimState", 0);
        // if (partrolAI.SpotLightWander) partrolAI.SpotLightWander.SetActive(true);
        // if (partrolAI.SpotLightChase) partrolAI.SpotLightChase.SetActive(false);
    }
    private bool IsPathBlock()
    {
        Ray ray = new Ray(transform.position, mDirection);

        // Debug.Log(LayerMask.LayerToName(mlayerMask));

        //当球体扫描与任何碰撞器相交时为true，否则为false。0.5f球体半径
        return Physics.SphereCast(ray, 0.5f, rayDis, mlayerMask);
    }

    private bool IsForwardBlock()
    {
        //Debug.DrawRay(transform.position,  transform.forward * rayDis, Color.blue);
        //射线检测
        Ray ray = new Ray(transform.position,transform.forward);
        //当球体扫描与任何碰撞器相交时为true，否则为false。0.5f球体半径
        //return Physics.SphereCast(ray,0.5f,rayDis,mlayerMask);
        bool res= Physics.SphereCast(ray, 0.5f, rayDis, mlayerMask);
        return res;
    }

    //private bool IsPathBlocked3()
    //{
    //    Debug.DrawRay(transform.position, mDirection * rayDis, Color.blue);
    //    Ray ray = new Ray(transform.position, mDirection);
    //    var hitSomething = Physics.RaycastAll(ray, rayDis, partrolAI.mlayer);
    //    return hitSomething.Any();
    //}
    
    // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    private void FindRandomDis()
    {
        //随机
        Vector3 testPos = (transform.position + (transform.forward * 4f)) +
            (new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f, UnityEngine.Random.Range(-4.5f, 4.5f)));
        //目的
        mDestination = new Vector3(testPos.x, 1f, testPos.z);
        
        //debug绘制mDestination这个点 或者初始化一个cube放在这
        // cube.transform.position = mDestination.Value;
        
        
        //方向  (.Value)
        mDirection = Vector3.Normalize(mDestination.Value - transform.position);
        mDirection = new Vector3(mDirection.x,0f,mDirection.z);

        //旋转
        desiredRotation = Quaternion.LookRotation(mDirection);

        //Debug.Log("Got Direction");
    }

    //寻找目标
    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    //寻找目标
    private Transform CheckForAggro()
    {
        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        int lineCount = 1;
        // stepAngle = Quaternion.AngleAxis(120/lineCount, Vector3.up);
        stepAngle = Quaternion.AngleAxis(60, Vector3.up);
        for (var i = 0; i < lineCount; i++)
        {
            if (Physics.Raycast(pos, direction, out hit,YGameSetting.AggroRadius))
            {
                //var otherPartrolAI = hit.collider.GetComponent<YPatrolAI>();
                // var aTarget = hit.collider.GetComponent<YPlayerMovement>();
                var aTargetLayer = hit.collider.gameObject;

                //if (aTarget != null && aTarget.team != gameObject.GetComponent<YPatrolAI>().team)
                // if (aTarget != null)
                // {
                //     Debug.DrawRay(pos, direction * hit.distance, Color.red);
                //     return aTarget.transform;
                // }
                // else if (aTargetLayer.layer.ToString() == "Yplayer")
                // {
                //     Debug.DrawRay(pos, direction * hit.distance, Color.blue);
                //     return aTargetLayer.transform;
                // }
                if(aTargetLayer.tag=="Player")
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.blue);
                    return aTargetLayer.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * YGameSetting.AggroRadius, Color.white);
            }
            direction = stepAngle * direction;
        }

        return null;
    }
}

