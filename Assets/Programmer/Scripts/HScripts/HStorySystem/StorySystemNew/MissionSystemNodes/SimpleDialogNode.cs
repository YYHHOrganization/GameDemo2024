using System.Collections;
using System.Collections.Generic;
using OurGame.MissionSystem;
using UnityEngine;
using XNode;

public class SimpleDialogNode : Node 
{
	[Input] public bool forwardMission;
	[Output] public bool success; //成功成功的情况
	[Output] public bool fail; //任务失败的情况
	public bool isStartMission = false;
	public MissionRewardType rewardType = MissionRewardType.Treasure;
	public GameEventType gameEventType = GameEventType.CompleteDialogue;
	private int toId; //跳转节点的ID
	public HashSet<int> toIds = new HashSet<int>(); //跳转节点的ID
	public string filename;
	public string taskName;
	
	// Use this for initialization
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
		dialogSystemMgr.SetUpAndStartDialog();
		MissionRequire<GameMessage> missionRequire = new CompleteDialogRequire(gameEventType, "testDialog09");
		var requires = new MissionRequire<GameMessage>[] { missionRequire };
		MissionReward reward = new TriggerMissionExample.RewardNull();
		var rewards = new MissionReward[] { reward };
		string missionName = taskName;
		var missionProto = new MissionPrototype<GameMessage>(missionName, requires, rewards);
		return missionProto;
	}
}