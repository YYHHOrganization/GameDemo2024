using System.Collections;
using System.Collections.Generic;
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

    private void FindFirstMisson()
    {
        if (firstMissionNode==null)
        {
            firstMissionNode = graph.nodes.Find(node => node is SimpleMissonNode && (node as SimpleMissonNode).isStartMission);
            currentNode = firstMissionNode;
        }
    }
    
    private void SummonEnemyForMission(int count)
    {
        YSpecialMapTutorial ySpecialMapTutorial = FindObjectOfType<YSpecialMapTutorial>();
        if (ySpecialMapTutorial)
        {
            ySpecialMapTutorial.SummonEnemyForMission(count);
        }
    }

    private MissionPrototype<GameMessage> GenerateMission(Node firstMissionNode)
    {
        SimpleMissonNode simpleMissonNode = firstMissionNode as SimpleMissonNode;
        UsefulMissionFuncNode funcNode = firstMissionNode as UsefulMissionFuncNode;
        
        if (simpleMissonNode != null)
        {
            string args = simpleMissonNode.args;
            GameEventType gameEventType = simpleMissonNode.gameEventType;
            int count = simpleMissonNode.count;
            MissionRequire<GameMessage> missionRequire;
            switch (gameEventType)
            {
                case GameEventType.KillEnemy:
                    missionRequire = new KillEnemyRequire(gameEventType, count, args);
                    SummonEnemyForMission(count);
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
                default:
                    missionRequire = new KillEnemyRequire(gameEventType, count, args);
                    break;
            }
            
            var requires = new MissionRequire<GameMessage>[] { missionRequire };
            MissionReward reward;
            if(simpleMissonNode.rewardType == SimpleMissonNode.RewardType.Treasure)
            {
                reward = new TriggerMissionExample.RewardXingqiong();
            }
            else
            {
                reward = new TriggerMissionExample.RewardXingqiong();
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
            GameAPI.StartMission(firstMission);
        }
    }

    public void GetToNextMission(bool isSuccess)
    {
        if (isSuccess)
        {
            //找currentNode对应success的output节点连接的地方
            NodePort successPort = currentNode.GetOutputPort("success");
            if (successPort.IsConnected)
            {
                NodePort connectedPort = successPort.Connection;
                int toId = graph.nodes.IndexOf(connectedPort.node);
                currentNode = graph.nodes[toId];
                MissionPrototype<GameMessage> nextMission = GenerateMission(currentNode);
                if (nextMission == null) return;
                GameAPI.StartMission(nextMission);
            }
        }
        else
        {
            //对应任务失败
        }
    }
}
