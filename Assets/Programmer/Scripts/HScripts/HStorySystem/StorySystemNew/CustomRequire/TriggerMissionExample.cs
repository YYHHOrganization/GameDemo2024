using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OurGame.MissionSystem;
using XNode;

public class TriggerMissionExample : MonoBehaviour
{
    bool isTriggered = false;
    private int killCount = 3;
    public NodeGraph graph;
    
    public class RewardXingqiong : MissionReward
    {
        public override void ApplyReward()
        {
            Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
            // 奖励对应数量的星琼
            HOpenWorldTreasureManager.Instance.InstantiateATreasureAndSetInfoWithTypeId("10000011", player.position + new Vector3(0.2f,-0.1f, 0.2f), YGameRoot.Instance.gameObject.transform);
        }
    }
    
    public class RewardNull : MissionReward
    {
        public override void ApplyReward()
        {
            // do nothing
        }
    }
    
    
    MissionPrototype<GameMessage> CreateExampleProto()
    {
        /* 创建案例任务原型 */
        
        /* 任务需求是执行三次PlayerBehaviourExample */
        var missionRequire = new KillEnemyRequire(GameEventType.KillEnemy, killCount);
        var requires = new MissionRequire<GameMessage>[] { missionRequire };
        var reward = new RewardXingqiong();
        var rewards = new MissionReward[] { reward };
        var missionProto = new MissionPrototype<GameMessage>("杀死那只小小宝", requires, rewards);
        
        return missionProto;
    }

    private void SummonEnemyForMission()
    {
        YSpecialMapTutorial ySpecialMapTutorial = FindObjectOfType<YSpecialMapTutorial>();
        if (ySpecialMapTutorial)
        {
            ySpecialMapTutorial.SummonEnemyForMission(killCount);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isTriggered) return;
            isTriggered = true;
            // 触发任务系统
            GameAPI.MissionManager.AddComponent(new MissionLogger());
            GameAPI.MissionManager.AddComponent(new MissionTracker());
            MissonSystemNodeMgr nodeMgr = gameObject.AddComponent<MissonSystemNodeMgr>();
            GameAPI.nodeMgr = nodeMgr;
            nodeMgr.graph = graph;
            nodeMgr.StartFirstMission();
            //GameAPI.StartMission(CreateExampleProto());
            
            //HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", "任务开始, 杀死3只小小宝！");
            //SummonEnemyForMission();
        }
    }
}
