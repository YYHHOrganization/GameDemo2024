using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OurGame.MissionSystem;

public enum GameEventType
{
    PlayerBehaviourExample,
    PlantCrop,
    EnterRoom,
    CompleteDialogue,
    KillEnemy,
    // more behaviors...
}

/// <summary>游戏消息对象</summary>
public class GameMessage
{
    public readonly GameEventType type;
    public readonly object args;
    public bool hasUsed { get; private set; }

    public GameMessage(GameEventType type, object args = null)
    {
        this.type = type;
        this.args = args;
        this.hasUsed = false;
    }

    /// <summary>使用当前消息</summary>
    public void Use() => 
        hasUsed = true;
}

public class KillEnemyRequire : MissionRequire<GameMessage>
{
    [SerializeField] private GameEventType type;
    [SerializeField] private string args;
    [SerializeField] private int count;

    public class Handle : MissionRequireHandle<GameMessage>
    {
        private readonly KillEnemyRequire require;
        private int count;
        
        public Handle(KillEnemyRequire exampleRequire) : base(exampleRequire)
        {
            require = exampleRequire;
        }
        
        protected override bool UseMessage(GameMessage message)
        {
            return ++count >= require.count;
        }
        
        public override string ToString() =>
            $"{count}/{require.count}";
    }
    
    public KillEnemyRequire(GameEventType type, int count, string args = null)
    {
        this.type = type;
        this.args = args;
        this.count = count;
    }
    
    public override bool CheckMessage(GameMessage message) =>
        message.type == type && message.args?.ToString() == args;
}


public static class GameAPI
{
    /* 理论上来说这个对象应该存储于玩家的存档数据或者云端用户数据中 */
    public readonly static MissionManager<GameMessage> MissionManager = new MissionManager<GameMessage>();

    /// <summary>朝游戏广播一条消息</summary>
    /// <param name="message"></param>
    public static void Broadcast(GameMessage message) =>
        MissionManager.SendMessage(message);

    public static void StartMission(MissionPrototype<GameMessage> missionProto) =>
        MissionManager.StartMission(missionProto);
}