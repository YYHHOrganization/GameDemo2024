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
    
    public YPetStateMachine mPetStateMachine => GetComponent<YPetStateMachine>();
    [SerializeField]Transform shootOrigin;

    public bool shouldCheckEnemyCount;//如果进入了战斗或者boss房，需要check敌人数量，敌人都死了就不需要了
    public PetAttackType attackType;
    private string attackType1;

    [SerializeField]GameObject AttackEff;
    [SerializeField]Transform AttackEffTrans;
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
            AttackEffTrans
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
}

public enum PetAttackType
{
    MeleeAttack,
    RangedAttack
}