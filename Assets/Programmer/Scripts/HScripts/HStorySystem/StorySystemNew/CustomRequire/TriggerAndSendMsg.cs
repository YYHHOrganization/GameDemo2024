using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAndSendMsg : MonoBehaviour
{
    bool hasEntered = false;
    private void OnTriggerEnter(Collider other)
    {
        if(hasEntered) return;
        if (other.CompareTag("Player"))
        {
            //GameAPI.Broadcast(new GameMessage(GameEventType.GotoSomewhere, "true"));
            HLoadScriptManager.Instance.BroadcastMessageToAll(new GameMessage(GameEventType.GotoSomewhere, "true"));
            hasEntered = true;
            Destroy(this.gameObject, 3f);
        }
            
    }
}
