﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class MissionStartNode : Node
{
	[Output] public bool success; //成功成功的情况
	// Use this for initialization
	protected override void Init() {
		base.Init();
		
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}