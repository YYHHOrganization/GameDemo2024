using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YPatrolAI : MonoBehaviour
{
    public LayerMask mlayer;
    [SerializeField] private Team mTeam;
    //只是一个光束
    [SerializeField] private GameObject laserVisual;
    public Animator animator;

    public GameObject mCamera;
    // public YMoveCamera moveCameraScripts;
    //我们希望能够读取目标 但是不能设置目标 比如敌方机器可以读取我们的位置但是不能设置我们的位置等
    public Transform mTarget { get; private set; }

    //以下这句话就是返回_team 表示一个匿名函数 其实也是public Team team；的意思 只是以下防止我们去修改我们的_team
    //lambda表达式 传入team 传出mTeam
    public Team team => mTeam;
    public YStateMachine stateMachine => GetComponent<YStateMachine>();
    
    public NavMeshAgent mNavMeshAgent;

    public GameObject SpotLightWander;
    public GameObject SpotLightChase;

    public bool isDead;

    public GameObject Mesh;
    public GameObject DisintegrateDissolveVFX;
    
    public GameObject DieExplosionEff;
    
    public Action OnDie;
    private void Awake()
    {
        InitStateMachine();
        animator = gameObject.GetComponentInChildren<Animator>();
        mNavMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        //animator = gameObject.GetComponentInChildren<Animator>();
        // moveCameraScripts = mCamera.GetComponent<YMoveCamera>();
    }
    private void InitStateMachine()
    {
        var states = new Dictionary<Type, YBaseState>
        {
            {typeof(YWanderState),new YWanderState(this) },
            {typeof(YChaseState),new YChaseState(this) },
            {typeof(YAttackState),new YAttackState(this) },
            {typeof(YDieState),new YDieState(this)}

        };
        GetComponent<YStateMachine>().SetStates(states);
    }
    
    public void setTarget(Transform tar)
    {
        mTarget = tar;
    }

    public void AttackFunc()
    {
        //UI部分 如果攻击到了
        // YUIManager uiManager = YUIManager.getInstance() as YUIManager;
        // uiManager.flashScreen();
        // uiManager.shakeScreen();
        
        //moveCameraScripts.cameraShakeFunc();
        //litjson.cameraShakeFunc();
        //todo:已经打到玩家了，玩家扣血
    }
    //还有个beattack 先弄个一击必杀吧
    public void die()
    {
        isDead = true;
        OnDie?.Invoke();
        Destroy(gameObject, 5f);
    }
}
public enum Team
{
    Enemy,
    Friend
}