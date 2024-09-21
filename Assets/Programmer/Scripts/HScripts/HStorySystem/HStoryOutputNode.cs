using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XNode;

public class HStoryOutputNode : Node
{
	[Input] public string inputStory;
	[Input] public string fileName;

	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		base.OnCreateConnection(from, to);
		Debug.Log("Input connected.");
		//当generateSignal设置为true时，输出.csv文件到当前目录下
		if (true)
		{
			string name = GetInputValue<string>("fileName", this.fileName);
			if (name == "")
			{
				name = "DefaultStory";
			}
			//把inputStory的内容写入到文件中
			// 获取输入故事的值
			//string storyContent = GetInputValue<string>("inputStory", this.inputStory);
			List<HStoryLinearNode> linearNodes = new List<HStoryLinearNode>();
			List<HStoryChoiseNode> choiseNodes = new List<HStoryChoiseNode>();
			foreach (var node in graph.nodes)
			{
				Debug.Log(node.name);
				if (node is HStoryLinearNode linearNode)
				{
					linearNodes.Add(linearNode);
					HashSet<int> toIds = linearNode.toIds;
					//翻转toIds
					var reverseResult = toIds.Reverse();
					foreach (int toId in reverseResult)  //把linear node连接的choise node按照顺序加入到列表当中，不容易出现bug
					{
						if(graph.nodes[toId] is HStoryChoiseNode choiseNode)
						{
							choiseNodes.Add(choiseNode);
						}
					}
				}
			}
			
			// 定义文件路径（Assets/Designer/DialogSystem/Outputs/storyName）
			string path = $"Assets/Designer/DialogSystem/Outputs/{name}.csv";
			
			// 将故事内容写入文件
			using (StreamWriter writer = new StreamWriter(path))
			{
				writer.WriteLine("Signal,ID,Character,Title,位置,Content,JumpTo,Effect,Target"); // CSV 文件头
				foreach(HStoryLinearNode linearNode in linearNodes)
				{
					writer.WriteLine(linearNode.GetValue(linearNode.GetOutputPort("result")).ToString());
				}
			
				foreach(HStoryChoiseNode choiseNode in choiseNodes)
				{
					writer.WriteLine(choiseNode.GetValue(choiseNode.GetOutputPort("result")).ToString());
				}
				writer.WriteLine("!,NOTSHOWAGAIN,,,,,,,");
			}

			Debug.Log($"CSV file created at: {path}");
		}
	}
}