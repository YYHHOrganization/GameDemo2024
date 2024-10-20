using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OurGame.MissionSystem;
using XNode;

    public class UIUpdater : IMissionSystemComponent<GameMessage>  //这个系统用于更新UI显示
    {
        public void OnMissionStarted(Mission<GameMessage> mission)
        {
            
        }

        public void OnMissionRemoved(Mission<GameMessage> mission, bool isFinished)
        {
            
        }
        
        public void OnMissionStatusChanged(Mission<GameMessage> mission, bool isFinished)
        {
            if (isFinished)
            {
                return;
            }
            //对应状态改变，但是任务还没有完成
        }
    }

    public class MissionLogger : IMissionSystemComponent<GameMessage>
    {
        public void OnMissionStarted(Mission<GameMessage> mission)
        {
            if (!logStart) return;
            Debug.Log($"Mission \"{mission.id}\" started");
            string msg = $"任务 \"{mission.id}\" 开始";
            HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", msg);
        }
        
        public MissionLogger()
        {
            Debug.Log("MissionLogger created");
        }

        bool logStart = true;
        bool logStatusChanged = true;
        bool logEnd = true;
        public MissionLogger(bool startLog, bool statusChangedLog, bool endLog)
        {
            logStart = startLog;
            logStatusChanged = statusChangedLog;
            logEnd = endLog;
        }

        public void OnMissionRemoved(Mission<GameMessage> mission, bool isFinished) { }

        public void OnMissionStatusChanged(Mission<GameMessage> mission, bool isFinished)
        {
            if (isFinished)
            {
                Debug.Log($"Mission \"{mission.id}\" is finished");
                if (!logEnd) return;
                string msg = $"任务 \"{mission.id}\" 已完成。";
                HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", msg);
                return;
            }
            Debug.Log($"Mission \"{mission.id}\" status changed: {mission.HandleStatus[0]}");
            if(!logStatusChanged) return;
            string msg2 = $"任务 \"{mission.id}\" 状态改变: {mission.HandleStatus[0]}";
            HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", msg2);
        }
    }
    
    public class MissionTracker : IMissionSystemComponent<GameMessage>  //任务跟踪器，用来根据任务的节点图来自动安排下一个任务
    {
        private string missionName;
        public MissionTracker(string missionName)
        {
            this.missionName = missionName;
        }
        public void OnMissionStarted(Mission<GameMessage> mission)
        {
            string msg = $"任务 \"{mission.id}\" 开始";
            //HMessageShowMgr.Instance.ShowMessage("LEVEL_IN_MSG_0", msg);
        }

        public void OnMissionRemoved(Mission<GameMessage> mission, bool isFinished) { }

        public void OnMissionStatusChanged(Mission<GameMessage> mission, bool isFinished)
        {
            if (isFinished)
            {
                //jump to next mission
                // if (GameAPI.nodeMgr != null)
                // {
                //     GameAPI.GetToNextMission(true);
                // }
                HLoadScriptManager.Instance.GetNodeMgrToNextMission(missionName, true);
                return;
            }
            //对应状态改变，但是任务还没有完成
        }
    }
