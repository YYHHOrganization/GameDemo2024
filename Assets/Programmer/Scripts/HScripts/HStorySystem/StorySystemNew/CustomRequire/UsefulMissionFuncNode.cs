using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class UsefulMissionFuncNode : Node {

	[Input] public bool forwardMission;
	[Output] public bool success;

	public MissionSystemFuncEnum funcEnum = MissionSystemFuncEnum.DelayCall;
	public enum MissionSystemFuncEnum
	{
		DelayCall,
		ShowMessage,
	}

	public float args1 = 0;
	public string args2 = "";
	
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}