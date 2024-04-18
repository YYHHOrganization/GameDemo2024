using System.Collections;
using System.Collections.Generic;
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
    }

    // Start is called before the first frame update
    public override void ChangeHealth(int value)
    {
        Debug.Log("ChangeHealth");
        health += value;
        Debug.Log(gameObject.name + health);
        UpdateUI();

    }

    public void UpdateUI()
    {
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
}
