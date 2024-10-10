using System.Collections;
using System.Collections.Generic;
using OurGame.MissionSystem;
using UnityEngine;
using XNode;

public class SimpleDialogNode : Node 
{
	[Input] public bool forwardMission;
	// [Output] public bool success; //成功成功的情况
	// [Output] public bool fail; //任务失败的情况
	[Output] public bool branch1;
	[Output] public bool branch2;
	[Output] public bool branch3;
	[Output] public bool branch4;
	[Output] public bool branch5;
	
	public bool isStartMission = false;
	public MissionRewardType rewardType = MissionRewardType.Treasure;
	public GameEventType gameEventType = GameEventType.CompleteDialogue;
	private int toId; //跳转节点的ID
	public HashSet<int> toIds = new HashSet<int>(); //跳转节点的ID
	public NodeGraph m_childGraph;
	public string taskName;
	
	[HideInInspector] public int storyEndingId;
	public string NpcID;
	
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}

	public MissionPrototype<GameMessage> GenerateMission()
	{
		//首先开启对话系统
		HDialogSystemMgr dialogSystemMgr = yPlanningTable.Instance.gameObject.GetComponent<HDialogSystemMgr>();
		dialogSystemMgr.SetUpAndStartDialogWithGraph(m_childGraph,taskName, NpcID, this);
		MissionRequire<GameMessage> missionRequire = new CompleteDialogRequire(gameEventType, taskName);
		var requires = new MissionRequire<GameMessage>[] { missionRequire };
		MissionReward reward = new TriggerMissionExample.RewardNull();
		var rewards = new MissionReward[] { reward };
		string missionName = taskName;
		var missionProto = new MissionPrototype<GameMessage>(missionName, requires, rewards);
		return missionProto;
	}
}