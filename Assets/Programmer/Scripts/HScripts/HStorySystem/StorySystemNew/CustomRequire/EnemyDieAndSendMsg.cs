using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieAndSendMsg : MonoBehaviour
{
    private void OnDestroy()
    {
        //Debug.Log("KILL !!!");
        GameAPI.Broadcast(new GameMessage(GameEventType.KillEnemy));
    }
}
