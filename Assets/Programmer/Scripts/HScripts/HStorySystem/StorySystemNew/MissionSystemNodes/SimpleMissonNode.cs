using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SimpleMissonNode : Node
{
	/*
	 * 最简单的任务节点，类似线性的那种任务，有一个require和对应的reward，先写第一版
	 */
	// Use this for initialization
	//[Input] public string rewardTreasureId;
	[Input] public bool forwardMission;
	[Output] public bool success; //成功成功的情况
	[Output] public bool fail; //任务失败的情况
	
	public MissionRewardType rewardType = MissionRewardType.Treasure;
	public MissionMessageType messageType = MissionMessageType.GameMessage;
	public GameEventType gameEventType = GameEventType.KillEnemy;
	public int count = -1;
	
	public bool isStartMission = false;
	private int toId; //跳转节点的ID
	public HashSet<int> toIds = new HashSet<int>(); //跳转节点的ID

	public string missionName;

	public string args; // 参数，用分号隔开
	protected override void Init() {
		base.Init();
		
	}
	
	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		toId = graph.nodes.IndexOf(to.node);
		toIds.Add(toId);  //4 5
	}

	public override void OnRemoveConnection(NodePort port)
	{
		// 断开连接时，需要找到并移除对应的 toId
		if (port.IsOutput) // 如果当前端口是输出端口
		{
			foreach (var connectedPort in port.GetConnections())
			{
				int toId = graph.nodes.IndexOf(connectedPort.node);
				toIds.Remove(toId); // 从 toIds 中移除断开的目标节点 ID
			}
		}
		else if (port.IsInput) // 如果当前端口是输入端口
		{
			int toId = graph.nodes.IndexOf(port.node);
			toIds.Remove(toId); // 从 toIds 中移除断开的目标节点 ID
		}
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
	
}