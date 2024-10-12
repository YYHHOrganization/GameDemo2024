using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OurGame.MissionSystem;
using XNode;

public class TriggerMissionObject : MonoBehaviour
{
    bool isTriggered = false;
    public NodeGraph graph;
    public string missionId;
    private MissionManager<GameMessage> missionManager;
    public bool logProcess=true;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isTriggered) return;
            if (Input.GetKey(KeyCode.F))
            {
                isTriggered = true;
                missionManager = new MissionManager<GameMessage>();
                // 触发任务系统
                if (logProcess)
                {
                    missionManager.AddComponent(new MissionLogger());
                }
                else
                {
                    missionManager.AddComponent(new MissionLogger(true,false,true));
                }
                
                missionManager.AddComponent(new MissionTracker(missionId));
                missionManager.AddComponent(new UIUpdater());
                HLoadScriptManager.Instance.AddOrReplaceMissionManager(missionId, missionManager);
                MissonSystemNodeMgr nodeMgr = gameObject.AddComponent<MissonSystemNodeMgr>();
                nodeMgr.graph = graph;
                nodeMgr.MissionManager = missionManager;
                nodeMgr.StartFirstMission();
                HLoadScriptManager.Instance.AddOrReplaceNodeMgr(missionId, nodeMgr);
            }
            //GameAPI.StartMission(CreateExampleProto());
            
            //HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", "任务开始, 杀死3只小小宝！");
            //SummonEnemyForMission();
        }
    }
    
}
