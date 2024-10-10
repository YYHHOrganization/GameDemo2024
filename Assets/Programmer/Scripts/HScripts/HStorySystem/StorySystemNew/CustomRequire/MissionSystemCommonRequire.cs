using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OurGame.MissionSystem;
public class GotoSomewhereRequire : MissionRequire<GameMessage>
{
    [SerializeField] private GameEventType type;
    [SerializeField] private string args;
    public class Handle : MissionRequireHandle<GameMessage>
    {
        private readonly GotoSomewhereRequire require;
        public bool isArrived;
        
        public Handle(GotoSomewhereRequire exampleRequire) : base(exampleRequire)
        {
            require = exampleRequire;
        }
        
        protected override bool UseMessage(GameMessage message)
        {
            return true; //说明已经到达目的地了，return true即可
        }

        public override string ToString() =>
            isArrived.ToString();
    }
    
    public GotoSomewhereRequire(GameEventType type, string args)
    {
        this.type = type;
        this.args = args;
    }
    
    public override bool CheckMessage(GameMessage message) =>
        message.type == type && message.args?.ToString() == args;
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

public class CompleteDialogRequire : MissionRequire<GameMessage>
{
    [SerializeField] private GameEventType type;
    [SerializeField] private string args;

    public class Handle : MissionRequireHandle<GameMessage>
    {
        private readonly CompleteDialogRequire require;
        
        public Handle(CompleteDialogRequire exampleRequire) : base(exampleRequire)
        {
            require = exampleRequire;
        }
        
        protected override bool UseMessage(GameMessage message)
        {
            //return ++count >= require.count;
            return true;
        }
        
    }
    
    public CompleteDialogRequire(GameEventType type, string args = null)
    {
        this.type = type;
        this.args = args;
    }
    
    public override bool CheckMessage(GameMessage message) =>
        message.type == type && message.args?.ToString() == args;
}

public class BrokenSomethingRequire : MissionRequire<GameMessage>
{
    [SerializeField] private GameEventType type;
    [SerializeField] private string args;
    [SerializeField] private int count;

    public class Handle : MissionRequireHandle<GameMessage>
    {
        private readonly BrokenSomethingRequire require;
        private int count;
        
        public Handle(BrokenSomethingRequire exampleRequire) : base(exampleRequire)
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
    public BrokenSomethingRequire(GameEventType type, int count, string args = null)
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
    public static MissonSystemNodeMgr nodeMgr;
    /// <summary>朝游戏广播一条消息</summary>
    /// <param name="message"></param>
    public static void Broadcast(GameMessage message) =>
        MissionManager.SendMessage(message);

    public static void StartMission(MissionPrototype<GameMessage> missionProto) =>
        MissionManager.StartMission(missionProto);
    
    public static void GetToNextMission(bool isSuccess)=>nodeMgr.GetToNextMission(isSuccess);
}