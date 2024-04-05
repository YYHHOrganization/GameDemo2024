using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class YGameSetting : MonoBehaviour
{
    [SerializeField] private float pPatrolAISpeed=2f;
    public static float PatrolAISpeed => Instance.pPatrolAISpeed;

    [SerializeField] private float pAttackRange = 3f;
    public static float AttackRange => Instance.pAttackRange;

    [SerializeField] private float pAggroRadius = 5f;
    public static float AggroRadius => Instance.pAggroRadius;

    [SerializeField] private float pAttackReadyTimer = 0.28f;
    public static float attackReadyTimer => Instance.pAttackReadyTimer;

    [SerializeField] private float pChaseRange = 6f;
    public static float ChaseRange => Instance.pChaseRange;

    //单例模式么
    public static YGameSetting Instance { get; private set; }
    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }
}
