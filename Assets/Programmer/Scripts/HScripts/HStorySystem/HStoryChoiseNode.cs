using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class HStoryChoiseNode : Node {

	//可以拉出不同的选项分支
	[Input] public string inputStory;

	// 存储动态输出
	//private List<string> contents = new List<string>();

	//[Output(dynamicPortList = true)] // 设置为动态端口列表
	[TextArea]public string choiseText; // 输出连接
	[TextArea] public string effect; //暂时先用字符串+反射的方法来做
	[Output] public string result;
	private int toId;
	
	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		toId = graph.nodes.IndexOf(to.node);
	}

	// public override void OnRemoveConnection(NodePort port)
	// {
	// 	toId = -1;
	// }

	// Use this for initialization
	protected override void Init() 
	{
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
	{
		int id = port.node.graph.nodes.IndexOf(port.node);
		if (port.fieldName == "result")
		{
			//todo:effect的效果后面再说
			string outputText = "&,"+id+",,,左,"+choiseText+"," + toId+","+effect+",";
			return outputText;
		}

		return "";
	}
}