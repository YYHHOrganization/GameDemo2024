using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HRogueEnemyPatrolAI : MonoBehaviour
{
    //todo：目标是这里通过一些可配置的参数实现不同的普通怪逻辑
    //比如移动的速度，移动的范围，怪物的移动是勤奋的/懒散的，是否可能发射子弹等等,是否需要寻路系统等
    public LayerMask mlayer;
    [SerializeField] private RogueEnemyTeam mTeam;
    public Animator animator;

    public GameObject mCamera;

    // public YMoveCamera moveCameraScripts;
    //我们希望能够读取目标 但是不能设置目标 比如敌方机器可以读取我们的位置但是不能设置我们的位置等
    public Transform mTarget { get; set; }

    //以下这句话就是返回_team 表示一个匿名函数 其实也是public Team team；的意思 只是以下防止我们去修改我们的_team
    //lambda表达式 传入team 传出mTeam
    public RogueEnemyTeam team => mTeam;
    public HRogueEnemyCommonStateMachine stateMachine => GetComponent<HRogueEnemyCommonStateMachine>();

    public NavMeshAgent mNavMeshAgent;

    public bool isDead;

    public GameObject Mesh;
    public GameObject DisintegrateDissolveVFX;

    public GameObject DieExplosionEff;

    # region 这些暂时全部开放可以调整，后续应该是从策划表当中去读取

    
    public RogueEnemyWanderType wanderType;
    public RogueEnemyChaseType chaseType;
    private string bulletPrefabLink;
    private GameObject bulletPrefab;

    private string chaseBulletPrefabLink;
    private GameObject chaseBulletPrefab;

    private float wanderMaxStopTime = 2f;
    private float wanderMaxMoveTime = 10f;
    public float jumpStopMaxInterval = 2f;
    public float maxJumpHeight = 2f;
    
    [Header("Chase状态下的参数")] 
    public float chaseNoTargetWaitMaxTime = 5f; //角色离开范围多少秒后，怪物会返回Wander状态
    public float chaseMaxAcceleration = 8f;
    
    [Header("Attack状态下的参数")] 
    public float attackRange = 1f;
    public int enemyDamage = 2;
    public float bulletChaseShootInterval = 1f;

    // private int health;
    protected int health;

    # endregion

    private int isWalkingHash;
    private int isAttackingHash;
    private int isDeadHash;
    public int IsAttackingHash => isAttackingHash;
    private Transform shootOrigin;
    protected int maxHealth;

    public Image enemyHealthImage;
    public string enemyID;

    public Class_RogueEnemyCSVFile enemy;

    public Action OnDie;

    private int hitPlayerDamage = -1; // 碰到玩家的伤害
    
    public string curStateName;

    protected virtual void Awake()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isAttackingHash = Animator.StringToHash("isAttacking");
        isDeadHash = Animator.StringToHash("isDead");
        shootOrigin = transform.Find("ShootOrigin");
        ReadTableAndSetAttribute();
    }

    private void ReadTableAndSetAttribute()
    {
        enemy = SD_RogueEnemyCSVFile.Class_Dic[enemyID];
        string wanderType1 = enemy.RogueEnemyWanderType;
        wanderType = (RogueEnemyWanderType)Enum.Parse(typeof(RogueEnemyWanderType), wanderType1);
        string chaseType1 = enemy.RogueEnemyChaseType;
        chaseType = (RogueEnemyChaseType)Enum.Parse(typeof(RogueEnemyChaseType), chaseType1);
        health = enemy._RogueEnemyStartHealth();
        maxHealth = health;

        bulletPrefabLink = enemy.RogueEnemyWanderBulletLink;
        if(bulletPrefabLink!=null && bulletPrefabLink!="null")
            bulletPrefab = Addressables.LoadAssetAsync<GameObject>(bulletPrefabLink).WaitForCompletion();
        chaseBulletPrefabLink = enemy.RogueEnemyChaseBulletPrefab;
        if(chaseBulletPrefabLink!=null && chaseBulletPrefabLink!="null")
            chaseBulletPrefab = Addressables.LoadAssetAsync<GameObject>(chaseBulletPrefabLink).WaitForCompletion();
        InitStateMachine();
    }
    

    private void InitStateMachine()
    {
        var states = new Dictionary<Type, HRogueEnemyBaseState>
        {
            { typeof(HRogueEnemyCommonWanderState), new HRogueEnemyCommonWanderState(this) },
            { typeof(HRogueEnemyCommonChaseState), new HRogueEnemyCommonChaseState(this)},
            {typeof(HRogueEnemyCommonBeFrozenState), new HRogueEnemyCommonBeFrozenState(this)}

        };
        GetComponent<HRogueEnemyCommonStateMachine>().SetStates(states);
    }
    
    public bool CheckIfTargetIsNear(float sensitiveDis)  //判断角色是否离的比较近
    {
        if (Vector3.Distance(mTarget.position, transform.position) <= sensitiveDis)
        {
            return true;
        }
        return false;
    }

    public IEnumerator RandomJump()
    {
        while (true)
        {
            //沿着随机方向跳跃一次，并在跳跃之后随机休息一段时间
            //用AddForce实现简单的跳跃
            Vector3 jumpDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, maxJumpHeight), Random.Range(-1f, 1f));
            GetComponent<Rigidbody>().AddForce(jumpDirection * 6, ForceMode.Impulse);
            transform.forward = new Vector3(jumpDirection.x, 0, jumpDirection.z);
            yield return new WaitForSeconds(jumpStopMaxInterval);
        }
    }

    public void setTarget(Transform tar)
    {
        mTarget = tar;
    }

    public void AttackFunc()
    {
        //todo:已经打到玩家了，玩家扣血

        //wanderType = Enum.Parse(SD_RogueEnemyCSVFile.Class_Dic["70000001"].RogueEnemyWanderType));
        
    }
    

    public void EnemyMoveRandomly()
    {
        // 不走Nev Mesh系统，直接随机游走，这里可以配置怪物的移动速度，移动范围等，当然会和场景碰撞箱自动做碰撞检测
        StartCoroutine(WanderAndStopRandomly());
    }

    public void SetFrozen(float frozenTime)
    {
        enemyIsFrozen = true;
        animator.enabled = false;
        this.frozenTime = frozenTime;
        stateMachine.JustSwitchState(typeof(HRogueEnemyCommonBeFrozenState));
    }

    private bool enemyIsFrozen = false;
    public float frozenTime = 5f;
    public bool EnemyIsFrozen => enemyIsFrozen;
    public IEnumerator FrozenEnemyItself()
    {
        GameObject frozenIce = Addressables.InstantiateAsync("IceOnEnemy", gameObject.transform).WaitForCompletion();
        
        yield return new WaitForSeconds(frozenTime);
        enemyIsFrozen = false;
        animator.enabled = true;
        Destroy(frozenIce);
        //Addressables.Release(frozenIce);
    }


    private bool isMoving = true;

    IEnumerator WanderAndStopRandomly()
    {
        Debug.Log("EnemyMoveRandomly");
        while (true)
        {
            float moveTime = Random.Range(0, wanderMaxMoveTime);
            float moveDistance = enemy._RogueEnemyWanderSpeed() * moveTime;
            int count = 0; //开一个计数器，防止怪物卡死
            while (isMoving)
            {
                Vector3 moveDirection = new Vector3(Random.Range(-1f, 1f)* moveDistance, 0, Random.Range(-1f, 1f)*moveDistance) ;
                Vector3 destination = transform.position + moveDirection;
                //怪物要转向前进的目标
                transform.LookAt(destination);
                animator.SetBool(isWalkingHash, true);
                
                while (transform.position != destination && isMoving)
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * enemy._RogueEnemyWanderSpeed());
                    count++;
                    if (count == 100)
                    {
                        count = 0;
                        isMoving = false;
                    }
                    yield return null;
                }
                isMoving = false;
            }

            if (!isMoving)
            {
                animator.SetBool(isWalkingHash, false);
                float stopTime = Random.Range(0, wanderMaxStopTime);
                yield return new WaitForSeconds(stopTime);
                isMoving = true;
            }
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("RogueRoom"))
        {
            //怪物要转身
            isMoving = false;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            // 玩家碰到要扣血
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(hitPlayerDamage);
        }
    }
    
    public void ShootBulletForward(bool trackPlayer = false, bool isChasing=false)
    {
        /*
         * 1.根据预制体生成子弹(bulletPrefabLink)
         * 2.子弹向前发射：bullet.GetComponent<bulletBase>().ShootForward();
         * 3.开启子弹射击的协程这种写到子弹里面应该就行了
         */
        StartCoroutine(SendBulletOut(trackPlayer, isChasing));
    }
    

    IEnumerator SendBulletOut(bool trackPlayer, bool isChasing)
    {
        while (true)
        {
            if (bulletPrefab)
            {
                GameObject bullet = Instantiate(bulletPrefab, shootOrigin.position, shootOrigin.rotation);
                if (trackPlayer)
                {
                    var bulletScript = bullet.GetComponent<HEnemyBulletMoveBase>();
                    bulletScript.SetTarget(mTarget);
                    SetBulletBaseAttribute(bulletScript);
                }
                if(isChasing)
                    yield return new WaitForSeconds(enemy._RogueEnemyChaseShootInterval());
                else
                    yield return new WaitForSeconds(enemy._RogueEnemyWanderShootInterval());
            }
        }
    }

    private void SetBulletBaseAttribute(HEnemyBulletMoveBase script)
    {
        string bulletAttribute = enemy.EnemyBulletAttribute;
        string[] attributes = bulletAttribute.Split(';');
        float speed = float.Parse(attributes[0]);
        float range = float.Parse(attributes[1]);
        if(curStateName == "wander")
            script.SetBulletAttribute(speed, enemy._EnemyWanderDamage(), range);
        else if(curStateName == "chase")
            script.SetBulletAttribute(speed, enemy._EnemyChaseDamage(), range);
    }

    protected void UpdateEnemyHeathAndShieldUI()
    {
        //enemyHealthImage.fillAmount
        if (enemyHealthImage)
        {
            enemyHealthImage.fillAmount = health * 1.0f / maxHealth;
        }
    }

    public virtual void ChangeHealth(int value)
    {
        health += value;
        UpdateEnemyHeathAndShieldUI();
        if (health <= 0 && !isDead)
        {
            isDead = true;
            SetEnemyDie();
            
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    private void SetEnemyDie()
    {
        Debug.Log("You should really die!!!!");
        StopAllCoroutines();
        animator.SetBool(isDeadHash, true);
        mNavMeshAgent.enabled = false;
        GameObject mesh = Mesh;
        DissolvingControllery dissolving = mesh.GetComponent<DissolvingControllery>();
        dissolving.SetMaterialsPropAndBeginDissolve(mesh,1f);
        HRogueCameraManager.Instance.ShakeCamera(15f, 0.1f);
        DisintegrateDissolveVFX.SetActive(true);
        DieExplosionEff.SetActive(true);
        transform.DOScale(0.01f, 0.8f).SetEase(Ease.InExpo).onComplete = () =>
        {
            Destroy(this.gameObject, 1f);
        };
        
        OnDie?.Invoke();
        
    }
    
    //这里往后来写一些射击子弹的逻辑
    public IEnumerator ShootCircle()
    {
        //每隔一段时间发射一圈环形子弹
        while (true)
        {
            for (int i = 0; i < 360; i += 30)
            {
                Vector3 shootDirection = new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), 0.4f, Mathf.Cos(i * Mathf.Deg2Rad));
                GameObject bullet = Instantiate(chaseBulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, i, 0));
                SetBulletBaseAttribute(bullet.GetComponent<HEnemyBulletMoveBase>());
            }
            yield return new WaitForSeconds(enemy._RogueEnemyChaseShootInterval());
        }
    }

    public IEnumerator ShootCircleInterval()  //也是环状射击子弹，不过一圈的每一颗子弹是间隔发射的
    {
        while (true)
        {
            for (int i = 0; i < 360; i += 15)
            {
                Vector3 shootDirection = new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), 0.4f, Mathf.Cos(i * Mathf.Deg2Rad));
                GameObject bullet = Instantiate(chaseBulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, i, 0));
                SetBulletBaseAttribute(bullet.GetComponent<HEnemyBulletMoveBase>());
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(enemy._RogueEnemyChaseShootInterval());
        }
    }

    public IEnumerator ShootBulletWall()
    {
        while (true)
        {
            //以发射中心为原点，发射一面子弹墙，每隔一段时间发射一次，子弹墙的数量和高度是随机的
            //配置是4 * 4， 5*5 和 6*6
            int bulletCnt = 2;
            int wallSize = 2;
            for (int i = 0; i < bulletCnt; i++)
            {
                for (int j = 0; j < bulletCnt; j++)
                {
                    Vector3 shootDirection = new Vector3(-wallSize / 2 + wallSize / bulletCnt*i,  0.4f + wallSize / bulletCnt*j, 0);
                    GameObject bullet = Instantiate(chaseBulletPrefab, transform.position + shootDirection , Quaternion.identity);
                    SetBulletBaseAttribute(bullet.GetComponent<HEnemyBulletMoveBase>());
                    bullet.GetComponent<HEnemyBulletMoveBase>().SetTarget(mTarget);
                }
            }
            yield return new WaitForSeconds(enemy._RogueEnemyChaseShootInterval());
        }
    }

    public IEnumerator ShootSpecialBulletWithMuzzle()
    {
        // 只负责生成Muzzle，至于Muzzle的逻辑写在Muzzle挂的脚本上
        while (true)
        {
            GameObject muzzleObj = new GameObject("muzzle");
            muzzleObj.transform.position = shootOrigin.position;
            muzzleObj.transform.rotation = shootOrigin.rotation;
            muzzleObj.transform.parent = transform;
            HBulletMuzzleUtility muzzleUtility = muzzleObj.AddComponent<HBulletMuzzleUtility>();
            muzzleUtility.SetInitializeAttribute(chaseBulletPrefab, enemy.RogueEnemyChaseShootFunc, true, curStateName, enemy, mTarget);
            muzzleUtility.Shoot();
            Destroy(muzzleObj, 20f);
            yield return new WaitForSeconds(enemy._RogueEnemyChaseShootInterval());
        }
    }
}

public enum RogueEnemyTeam
{
    Enemy,
    Friend
}

public enum RogueEnemyWanderType
{
    DontMove,  //不移动，固定在原地的怪，等看到玩家再进入chase状态
    MoveRandomlyWithStop, //走走停停，如果一直走不停的话Stop的time就填写0
    ShootBulletForwardWithMove, //向前发射子弹,策划表里需要填写一个子弹的prefab链接
    ShootBulletForwardWithoutMove, //向前发射子弹，但是不移动
    JustToChaseState, //直接进入chase状态,比较激进，不走Wander状态，
    RandomJump, //随机跳跃
}

public enum RogueEnemyChaseType
{
    JustChase, //只是追击
    ChaseAndShootPlayer, //追击并且射击玩家
    AddSthToPlayer, //比如说远程攻击的怪，会降下落雷之类的
    JustGoToAttackState, //直接进入攻击状态，一般来说可能是远程攻击的怪物这种
    ChaseAndShootSpecial, //追击并且射击子弹，但是子弹有特殊的效果，由函数来决定
    DontMove,
    ChaseAndShootWithSpecialMuzzle,
    ShootSpecialMuzzleDontMove,
}

public enum RogueEnemyAttackType
{
    //todo:可能会优化逻辑，比如说有些怪物会有多种攻击方式，比如说近战怪物会有近战攻击和远程攻击，以及这种近战是写到chase里面还是写到Attack里面
    Melee, //近战
    Range, //远程
    Magic, //魔法
    Special, //特殊
}
