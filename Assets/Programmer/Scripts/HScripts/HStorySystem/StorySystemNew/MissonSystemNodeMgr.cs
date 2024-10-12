using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using DG.Tweening;
using OurGame.MissionSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using XNode;

public class MissonSystemNodeMgr : MonoBehaviour
{
    public NodeGraph graph;
    private Node firstMissionNode;
    public Node currentNode;
    public MissionManager<GameMessage> MissionManager = new MissionManager<GameMessage>();
    private void FindFirstMisson()
    {
        if (firstMissionNode==null)
        {
            //firstMissionNode = graph.nodes.Find(node => node is SimpleMissonNode && (node as SimpleMissonNode).isStartMission);
            firstMissionNode = graph.nodes.Find(node=>node is MissionStartNode);
            NodePort successPort = firstMissionNode.GetOutputPort("success");
            List<NodePort> connectedPort = successPort.GetConnections();
            int toId = graph.nodes.IndexOf(connectedPort[0].node);
            currentNode = graph.nodes[toId];
            firstMissionNode = currentNode;
        }
    }
    
    private void SummonEnemyForMission(int count)
    {
        YSpecialMapTutorial ySpecialMapTutorial = FindObjectOfType<YSpecialMapTutorial>();
        if (ySpecialMapTutorial)
        {
            ySpecialMapTutorial.SummonEnemyForMission(count + 5);
        }
    }

    private MissionPrototype<GameMessage> GenerateMission(Node firstMissionNode)
    {
        SimpleMissonNode simpleMissonNode = firstMissionNode as SimpleMissonNode;
        UsefulMissionFuncNode funcNode = firstMissionNode as UsefulMissionFuncNode;
        SimpleDialogNode simpleDialogNode = firstMissionNode as SimpleDialogNode;
        if (simpleDialogNode != null)
        {
            return simpleDialogNode.GenerateMission();
        }
        else if (simpleMissonNode != null)
        {
            //todo:直接调用simpleNode里面的方法生成任务
            //simpleMissonNode.GenerateMission();
            string args = simpleMissonNode.args;
            GameEventType gameEventType = simpleMissonNode.gameEventType;
            int count = simpleMissonNode.count;
            MissionRequire<GameMessage> missionRequire;
            switch (gameEventType)
            {
                case GameEventType.KillEnemy:
                    missionRequire = new KillEnemyRequire(gameEventType, count, args);
                    if (HLoadScriptManager.Instance.isInTutorial)
                    {
                        SummonEnemyForMission(count);
                    }
                    break;
                case GameEventType.GotoSomewhere:
                    // 在args对应的位置(类似于225.6;3.03;-225.03，需要解析)生成一个Trigger，挂载一个脚本，OnTriggerEnter的时候触发新任务
                    GameObject trigger = Addressables.InstantiateAsync("TriggerToGoTo").WaitForCompletion();
                    Vector3 pos = new Vector3();
                    string[] posStr = args.Split(';');
                    pos.x = float.Parse(posStr[0]);
                    pos.y = float.Parse(posStr[1]);
                    pos.z = float.Parse(posStr[2]);
                    trigger.transform.position = pos;
                    trigger.gameObject.AddComponent<TriggerAndSendMsg>();
                    missionRequire = new GotoSomewhereRequire(gameEventType, "true");
                    break;
                case GameEventType.CompleteDialogue:  //完成对话对应的任务
                    missionRequire = new KillEnemyRequire(gameEventType, count, args);
                    break;
                case GameEventType.ShowTutorial:
                    missionRequire = new ReadTutorialRequire(gameEventType, args);
                    HMessageShowMgr.Instance.ShowMessage(args);
                    break;
                default:
                    missionRequire = new KillEnemyRequire(gameEventType, count, args);
                    break;
            }
            
            var requires = new MissionRequire<GameMessage>[] { missionRequire };
            MissionReward reward;
            switch (simpleMissonNode.rewardType)
            {
                case MissionRewardType.Treasure:
                    reward = new TriggerMissionExample.RewardXingqiong();
                    break;
                case MissionRewardType.None:
                    reward = new TriggerMissionExample.RewardNull();
                    break;
                default:
                    reward = new TriggerMissionExample.RewardNull();
                    break;
            }
            var rewards = new MissionReward[] { reward };
            string missionName = simpleMissonNode.missionName;
            var missionProto = new MissionPrototype<GameMessage>(missionName, requires, rewards);
            return missionProto;
        }
        else if(funcNode != null)
        {
            UsefulMissionFuncNode.MissionSystemFuncEnum funcEnum = funcNode.funcEnum;
            switch (funcEnum)
            {
                case UsefulMissionFuncNode.MissionSystemFuncEnum.DelayCall:
                    float args1 = funcNode.args1;
                    Debug.Log("DelayCall" + args1);
                    DOVirtual.DelayedCall(args1, () =>
                    {
                        GetToNextMission(true);
                    });
                    break;
                case UsefulMissionFuncNode.MissionSystemFuncEnum.ShowMessage:
                    int a1 = (int)funcNode.args1;  //2;TutorialName
                    string tutorialName = funcNode.args2;
                    if (a1 == 2 )
                    {
                        HMessageShowMgr.Instance.ShowMessage(tutorialName);
                    }
                    GetToNextMission(true);
                    break;
            }
        }

        return null;
    }
    
    public void StartFirstMission()
    {
        FindFirstMisson();
        if (firstMissionNode != null)
        {
            MissionPrototype<GameMessage> firstMission = GenerateMission(firstMissionNode);
            if (firstMission == null) return;
            //GameAPI.StartMission(firstMission);
            MissionManager.StartMission(firstMission);
        }
    }

    public void GetToNextMission(bool isSuccess)
    {
        if (isSuccess)
        {
            //找currentNode对应success的output节点连接的地方
            NodePort successPort = currentNode.GetOutputPort("success");
            if (successPort!=null && successPort.IsConnected)
            {
                List<NodePort> connectedPort = successPort.GetConnections();
                int toId = graph.nodes.IndexOf(connectedPort[0].node);
                currentNode = graph.nodes[toId];
                MissionPrototype<GameMessage> nextMission = GenerateMission(currentNode);
                if (nextMission == null) return;
                //GameAPI.StartMission(nextMission);
                MissionManager.StartMission(nextMission);
            }
            
            //额外的情况，不是success，有剧情的choose分支
            NodePort branchPort = currentNode.GetOutputPort("branch1");
            if (branchPort != null)
            {
                SimpleDialogNode simpleDialogNode = currentNode as SimpleDialogNode;
                int storyEndingId = simpleDialogNode.storyEndingId;
                NodePort toPort = currentNode.GetOutputPort("branch" + storyEndingId);
                if (toPort != null)
                {
                    List<NodePort> connectedPort = toPort.GetConnections();
                    int toId = graph.nodes.IndexOf(connectedPort[0].node);
                    currentNode = graph.nodes[toId];
                    MissionPrototype<GameMessage> nextMission = GenerateMission(currentNode);
                    if (nextMission == null) return;
                    //GameAPI.StartMission(nextMission);
                    MissionManager.StartMission(nextMission);
                }
            }
        }
        else
        {
            //对应任务失败
        }
    }
}
