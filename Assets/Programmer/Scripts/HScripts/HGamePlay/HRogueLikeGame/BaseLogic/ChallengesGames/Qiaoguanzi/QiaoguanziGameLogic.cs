using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using OurGame.MissionSystem;

public class QiaoguanziGameLogic : MonoBehaviour
{
    private int count = 10;
    
    private List<string> brokenItems = new List<string>();
    private List<GameObject> brokenItemObjects = new List<GameObject>();
    private MissionManager<GameMessage> MissionManager;

    private void InitializeGameModel()
    {
        GameObject currentRoom = YRogue_RoomAndItemManager.Instance.currentRoom;
        brokenItems.Add("YRogueBlueWhitewareCouldBroken-1-1");
        brokenItems.Add("YRogueWhitewareCouldBroken-1-1_ItemData");
        GameObject brokenItem1 = Addressables.LoadAssetAsync<GameObject>(brokenItems[0])
            .WaitForCompletion();
        GameObject brokenItem2 = Addressables.LoadAssetAsync<GameObject>(brokenItems[1])
            .WaitForCompletion();
        brokenItemObjects.Add(brokenItem1);
        brokenItemObjects.Add(brokenItem2);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, brokenItems.Count);
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
            GameObject item = Instantiate(brokenItemObjects[randomIndex], currentRoom.transform.position + offset, Quaternion.identity, currentRoom.transform);
        }
    }

    MissionPrototype<GameMessage> CreateExampleProto()
    {
        //  在20s内敲碎10个罐子
        var missionRequire = new BrokenSomethingRequire(GameEventType.BreakItem, count);
        var requires = new MissionRequire<GameMessage>[] { missionRequire };
        var reward = new TriggerMissionExample.RewardXingqiong();
        var rewards = new MissionReward[] { reward };
        var missionProto = new MissionPrototype<GameMessage>("限时敲罐子", requires, rewards);
        return missionProto;
    }

    private void MissionFailed()
    {
        HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", "任务失败");
        MissionManager.RemoveMission("限时敲罐子");
    }

    private void StartMission()
    {
        //开启一个任务：敲罐子
        MissionManager = new MissionManager<GameMessage>();
        var missionProto = CreateExampleProto();
        HLoadScriptManager.Instance.AddOrReplaceMissionManager("QiaoguanziGame", MissionManager);
        MissionManager.AddComponent(new MissionLogger());
        MissionManager.StartMission(missionProto);
        HMessageShowMgr.Instance.ShowTickMessage("限时敲罐子任务开始，15s内敲碎10个罐子", 15,MissionFailed);
    }
    
    private void Start()
    {
        //以房间中心为原点，生成30个物体，每个物体是随机的，但都在房间的范围内
        InitializeGameModel();
        StartMission();
    }
}
