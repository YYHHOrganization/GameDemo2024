using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRoomAppendant : MonoBehaviour
{
    public YRouge_RoomBase roomBase;
    // Start is called before the first frame update
    protected void Start()
    {
        roomBase.EnterRoomEvent += EnterRoomHandle;
    }

    protected virtual void EnterRoomHandle()
    {
        
    }

    private void OnDestroy()
    {
        roomBase.EnterRoomEvent -= EnterRoomHandle;
    }
}
