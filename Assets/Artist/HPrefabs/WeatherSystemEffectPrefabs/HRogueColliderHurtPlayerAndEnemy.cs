using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRogueColliderHurtPlayerAndEnemy : MonoBehaviour
{
    public bool isTrigger = true;
    public int damage = 2;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter!!!");
        if (!isTrigger) return;
        if (other.gameObject.CompareTag("Player"))
        {
            HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-damage);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            HRogueEnemyPatrolAI enemyPatrolAI = other.gameObject.GetComponentInParent<HRogueEnemyPatrolAI>();
            if (enemyPatrolAI == null)
            {
                enemyPatrolAI = other.gameObject.GetComponent<HRogueEnemyPatrolAI>();
            }
            if (enemyPatrolAI != null)
            {
                enemyPatrolAI.ChangeHealth(-damage);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
