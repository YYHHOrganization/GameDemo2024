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
    
    public void BroadcastMessage(string key, GameMessage message)
    {
        if (otherMissionManagers.ContainsKey(key))
        {
            otherMissionManagers[key].SendMessage(message);
        }
    }
}
