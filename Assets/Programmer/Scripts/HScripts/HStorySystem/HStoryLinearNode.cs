using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class HStoryLinearNode : Node 
{
	
	[Input] public string inputStory;
	[TextArea] public string CharacterName; // 角色名称
	[TextArea] public string Title; // 头衔
	[TextArea] public string Content; // 剧情文本内容
	private int toId; //跳转节点的ID
	public HashSet<int> toIds = new HashSet<int>(); //跳转节点的ID
	[Input] public bool isEnd; // 是否是结束节点

	[Output] public string result; // 输出连接
	public int storyEndingId;
	public MissionSystemDialogAnimation dialogAnimation;

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

	public override object GetValue(NodePort port) 
	{
		int id = port.node.graph.nodes.IndexOf(port.node);
		// 获取上一个节点的输出作为 inputStory
		string previousOutput = GetInputValue<string>("inputStory", "");
		//线性的直接输出一行文案的内容即可
		if (port.fieldName == "result" && !isEnd)
		{
			string outputText = "#,"+id+","+CharacterName+","+Title+",左,"+Content+"," + toId+",,";
			return outputText;
		}
		else
		{
			string outputText = "END,"+id+","+CharacterName+","+Title+",左,"+Content+"," + toId+",,";
			return outputText;
		}

		return "";
	}
}