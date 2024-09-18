using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OurGame.MissionSystem;

public class TriggerMissionExample : MonoBehaviour
{
    bool isTriggered = false;
    private int killCount = 3;
    public class MissionLogger : IMissionSystemComponent<GameMessage>
    {
        public void OnMissionStarted(Mission<GameMessage> mission)
        {
            Debug.Log($"Mission \"{mission.id}\" started");
            string msg = $"任务 \"{mission.id}\" 开始";
            HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", msg);
        }

        public void OnMissionRemoved(Mission<GameMessage> mission, bool isFinished) { }

        public void OnMissionStatusChanged(Mission<GameMessage> mission, bool isFinished)
        {
            if (isFinished)
            {
                Debug.Log($"Mission \"{mission.id}\" is finished");
                string msg = $"任务 \"{mission.id}\" 已完成，奖励贵重宝箱。";
                HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", msg);
                return;
            }
            Debug.Log($"Mission \"{mission.id}\" status changed: {mission.HandleStatus[0]}");
            string msg2 = $"任务 \"{mission.id}\" 状态改变: {mission.HandleStatus[0]}";
            HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", msg2);
        }
    }

    public class RewardXingqiong : MissionReward
    {
        public override void ApplyReward()
        {
            Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
            // 奖励对应数量的星琼
            HOpenWorldTreasureManager.Instance.InstantiateATreasureAndSetInfoWithTypeId("10000011", player.position + new Vector3(0.2f,-0.1f, 0.2f), YGameRoot.Instance.gameObject.transform);
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
            // 触发任务系统
            GameAPI.MissionManager.AddComponent(new MissionLogger());
            GameAPI.StartMission(CreateExampleProto());
            isTriggered = true;
            HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", "任务开始, 杀死3只小小宝！");
            SummonEnemyForMission();
        }
    }
}
