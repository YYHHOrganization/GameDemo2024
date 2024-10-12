using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OurGame.MissionSystem;

public class HLoadScriptManager : MonoBehaviour
{
    //单例
    public static HLoadScriptManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public Dictionary<string, MissionManager<GameMessage>> otherMissionManagers = new Dictionary<string, MissionManager<GameMessage>>();
    public Dictionary<string, MissonSystemNodeMgr> nodeMgrs = new Dictionary<string, MissonSystemNodeMgr>();
    public bool isInTutorial = false;
    public void AddOrReplaceMissionManager(string key, MissionManager<GameMessage> missionManager)
    {
        if (otherMissionManagers.ContainsKey(key))
        {
            otherMissionManagers[key] = missionManager;
        }
        else
        {
            otherMissionManagers.Add(key, missionManager);
        }
    }
    
    public void AddOrReplaceNodeMgr(string key, MissonSystemNodeMgr nodeMgr)
    {
        if (nodeMgrs.ContainsKey(key))
        {
            nodeMgrs[key] = nodeMgr;
        }
        else
        {
            nodeMgrs.Add(key, nodeMgr);
        }
    }
    
    public void BroadcastMessage(string key, GameMessage message)
    {
        if (otherMissionManagers.ContainsKey(key))
        {
            otherMissionManagers[key].SendMessage(message);
        }
    }
    
    public void BroadcastMessageToAll(GameMessage message)
    {
        foreach (var missionManager in otherMissionManagers)
        {
            missionManager.Value.SendMessage(message);
        }
    }
    
    public void GetNodeMgrToNextMission(string key, bool isFinished)
    {
        if (nodeMgrs.ContainsKey(key))
        {
            nodeMgrs[key].GetToNextMission(isFinished);
        }
    }
}
