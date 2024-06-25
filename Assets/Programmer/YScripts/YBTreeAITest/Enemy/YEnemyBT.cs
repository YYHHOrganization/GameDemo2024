using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YEnemyBT : HRogueEnemyPatrolAI
{
    public int curHealth
    {
        get { return health; }
        set { health = value; }
    }
    

    public GameObject bloodFrozeEff;
    //skinmesjrender
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public WaterBallControll waterBallController;
    public WaterBender waterBenderController;
    public GameObject bossPanel;

    public Action OnGettingHit;
    protected override void Awake()
    {
        health = 20;
        animator = gameObject.GetComponentInChildren<Animator>();
        // mNavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // isWalkingHash = Animator.StringToHash("isWalking");
        // isAttackingHash = Animator.StringToHash("isAttacking");
        // isDeadHash = Animator.StringToHash("isDead");
        // shootOrigin = transform.Find("ShootOrigin");
        maxHealth = health;
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        LoadAllRefPrefabs();
    }

    // Start is called before the first frame update
    public override void ChangeHealth(int value)
    {
        Debug.Log("ChangeHealth");
        health += value;
        Debug.Log(gameObject.name + health);
        UpdateUI();

        if (value < 0)
        {
            OnGettingHit?.Invoke();
        }
    }

    public void UpdateUI()
    {
        UpdateEnemyHeathAndShieldUI();
    }
    public void UpdateUI(int maxHealth)
    {
        base.maxHealth = maxHealth;
        UpdateEnemyHeathAndShieldUI();
    }
    
    public void bloodFrozeEffect(bool isFroze)
    {
        if (isFroze)
        {
            bloodFrozeEff.SetActive(false);
        }
        else
        {
            bloodFrozeEff.SetActive(true);
        }
    }

    public void ExplodeCore()
    {
        waterBallController.DestroyWaterBall();
    }
    
    // public void WaterBendAttack()
    // {
    //     waterBenderController.Attack();
    // }
    public void SetEnemyDie()
    {
        Debug.Log("You should really die!!!!");
        StopAllCoroutines();
        
        // DisintegrateDissolveVFX.SetActive(true);
        // DieExplosionEff.SetActive(true);
        
        // OnDie?.Invoke();
        OnDie?.Invoke(gameObject);
        
        enemyUICanvas.gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    // private void OnCollisionEnter(Collision other)
    {
       if (other.gameObject.CompareTag("Player"))
        {
            // 玩家碰到要扣血
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(hitPlayerDamage);
        }
    }

    

}
