using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public abstract class YPetBase : MonoBehaviour
{
    [SerializeField]string petID;
    Class_RoguePetCSVFile pet;
    public float followSpeed;
    public NavMeshAgent mNavMeshAgent;
    public Transform curCharacterTrans;
    public Transform ChaseTarget;//追逐的 ，目前应该是只用作enemy
    public Animator animator;
    float attackDistance;
    public float AttackDistance => attackDistance;
    
    
    public YPetStateMachine mPetStateMachine => GetComponent<YPetStateMachine>();
    [SerializeField]Transform shootOrigin;

    public bool shouldCheckEnemyCount;//如果进入了战斗或者boss房，需要check敌人数量，敌人都死了就不需要了
    public PetAttackType attackType;
    private string attackType1;
    public FollowTypeInBattle followTypeInBattle;

    [SerializeField]GameObject AttackEff;
    [SerializeField]Transform AttackEffTrans;
    [SerializeField] private GameObject weapon;
    protected virtual void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        curCharacterTrans = YPlayModeController.Instance.curCharacter.transform;
        InitStateMachine();
        YTriggerEvents.OnEnterRoomType += EnterNewRoom;
        
        //初始化发射器
        ReadTableAndSetAttribute();
        InitMuzzle();
    }
    
    GameObject bulletPrefab = null;
    private void ReadTableAndSetAttribute()
    {
        pet = SD_RoguePetCSVFile.Class_Dic[petID];
        
        string bulletPrefabLink = pet.WanderBulletLink;
        attackType1 = pet.AttackType;
        attackType = (PetAttackType)Enum.Parse(typeof(PetAttackType), attackType1);

        attackDistance = pet._PetAttackSensitiveDis();
        
        followTypeInBattle = (FollowTypeInBattle)Enum.Parse(typeof(FollowTypeInBattle), pet.FollowTypeInBattle);
        
        if(bulletPrefabLink!=null && bulletPrefabLink!="null")
            bulletPrefab = Addressables.LoadAssetAsync<GameObject>(bulletPrefabLink).WaitForCompletion();
    }
    YFriendBulletMuzzleUtility muzzleUtility;
    private void InitMuzzle()
    {
        GameObject muzzleObj = new GameObject("muzzle");
        muzzleObj.transform.position = shootOrigin.position;
        muzzleObj.transform.rotation = shootOrigin.rotation;
        Debug.Log("muzzleObj.transform.parent = transform;"+transform);
        muzzleObj.transform.parent = transform;
        Debug.Log("muzzleObj.transform.parent = transform;"+transform);
        
        muzzleUtility = muzzleObj.AddComponent<YFriendBulletMuzzleUtility>();
        muzzleUtility.SetInitializeAttribute(
            bulletPrefab,
            pet.ChaseShootFunc,
            attackType1, 
            pet, 
            null,
            AttackEff,
            AttackEffTrans,
            weapon
            );
            //chaseBulletPrefab, enemy.RogueEnemyChaseShootFunc, true, curStateName, enemy, mTarget);
    }
    public virtual void MuzzleShoot()
    {
        muzzleUtility.ShootSpecialBullet();
    }
    public virtual void MuzzleStopShoot()
    {
        muzzleUtility.ShootOff();
    }

    private void EnterNewRoom(object sender, YTriggerEnterRoomTypeEventArgs e)
    {
        //判断是什么房间。状态转换条件：进入战斗房/进入boss房（目前好像就这两个房间有怪），
        //进入商店房/进入普通房间（目前好像就这两个房间没有怪）
        if (e.roomType == RoomType.BattleRoom || e.roomType == RoomType.BossRoom)
        {
            shouldCheckEnemyCount = true;
            //mPetStateMachine.JustSwitchState(typeof(YPetAttackState));
        }
        
    }

    protected virtual void InitStateMachine()
    {
        var states = new Dictionary<Type, YPetBaseState>
        {
            { typeof(YPetFollowState), new YPetFollowState(this) },
            { typeof(YPetAttackState), new YPetAttackState(this) },
        };
        GetComponent<YPetStateMachine>().SetStates(states);
    }
    
    //如果进入房间，那么这里就会有一个监听，查看房间中的敌人数量，同时，如果敌人数量为0，那么就会切换到跟随状态
    //如果返回true，那么就说明房间有怪物
    public bool CheckEnemyCount()
    {
        if (shouldCheckEnemyCount == false)
        {
            return false;
        }
        List<GameObject> enemies =
            YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>().Enemies;
        if (enemies == null || enemies.Count == 0)
        {
            shouldCheckEnemyCount = false;
            return false;
        }
        return true;
    }
    
    //如果有敌人，那么就返回随机一个敌人
    public GameObject CheckAndGetEnemy()
    {
        if (shouldCheckEnemyCount == false)
        {
            return null;
        }
        List<GameObject> enemies =
            YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>().Enemies;
        if (enemies == null || enemies.Count == 0)
        {
            shouldCheckEnemyCount = false;
            return null;
        }
        else
        {
            return enemies[UnityEngine.Random.Range(0, enemies.Count)];
        }
        return null;
    }
    
    //返回是否距离角色足够近
    [SerializeField]
    private float closeEnoughDistance = 10;
    public bool IsCloseEnoughToCharacter()
    {
        return Vector3.Distance(transform.position, curCharacterTrans.position) < closeEnoughDistance;
    }

    public virtual void EnterChaseState()
    {
        
    }
    public virtual void EnterFollowState()
    {
        
    }

    public bool EnemyInAttackRange()
    {
        // if (ChaseTarget == null)
        // {
        //     return false;
        // }
        // return Vector3.Distance(ChaseTarget.position, transform.position) < attackDistance;
        // https://blog.csdn.net/qq2512667/article/details/83281203
        
        return CheckPosition(ChaseTarget.position, 60, attackDistance);
    }
    bool CheckPosition(Vector3 targetPos,float angle,float radius)
    {
 
        bool isCheck = false;
 
        //Mathf.Deg2Rad 角度转弧度
        var cosAngle = Mathf.Cos(Mathf.Deg2Rad * angle * 0.5f); //以一位单位，取得Cos角度
        Vector3 circleCenter = transform.position;
        Vector3 disV = targetPos - circleCenter;//从圆心到目标的方向
        float dis2 = disV.sqrMagnitude; // 得到 长度平方和
        if (dis2<radius*radius) // 视距内 
        {
            disV.y = 0.0f;
            disV = disV.normalized; //向量除以它的长度   向量归一化   对向量  
            //开平方 得到向量长度    
            //归一化后，即是 单位向量了。
            //用当前物体 向前方向，和从圆心到目标的单位方向 做 点积；
            float cos = Vector3.Dot(transform.forward, disV);//求点积
            //这样的结果就得到了cos角度*1
            if (cos-cosAngle>=0)
            {
                return true; //在视野内
            }
        }
        return false;
 
    }
}

public enum PetAttackType
{
    MeleeAttack,
    RangedAttack,
    ChaseBeforeMeleeAttack
}

//宠物战场跟随类型（如果是Close，那么战场中不会距离主人很远，否则会自己打不关注人在哪）
public enum FollowTypeInBattle
{
    Close,
    notConcern
}