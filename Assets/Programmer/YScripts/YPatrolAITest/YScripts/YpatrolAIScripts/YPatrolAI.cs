using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YPatrolAI : MonoBehaviour
{
    public LayerMask mlayer;
    [SerializeField] private Team mTeam;
    //ֻ��һ������
    [SerializeField] private GameObject laserVisual;
    public Animator animator;

    public GameObject mCamera;
    // public YMoveCamera moveCameraScripts;
    //����ϣ���ܹ���ȡĿ�� ���ǲ�������Ŀ�� ����з��������Զ�ȡ���ǵ�λ�õ��ǲ����������ǵ�λ�õ�
    public Transform mTarget { get; private set; }

    //������仰���Ƿ���_team ��ʾһ���������� ��ʵҲ��public Team team������˼ ֻ�����·�ֹ����ȥ�޸����ǵ�_team
    //lambda���ʽ ����team ����mTeam
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
        //UI���� �����������
        // YUIManager uiManager = YUIManager.getInstance() as YUIManager;
        // uiManager.flashScreen();
        // uiManager.shakeScreen();
        
        //moveCameraScripts.cameraShakeFunc();
        //litjson.cameraShakeFunc();
        //todo:�Ѿ�������ˣ���ҿ�Ѫ
    }
    //���и�beattack ��Ū��һ����ɱ��
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