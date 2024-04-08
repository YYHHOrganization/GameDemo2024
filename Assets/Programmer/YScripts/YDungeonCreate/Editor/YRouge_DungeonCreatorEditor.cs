using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(YDungeonCreator))]
public class YRouge_DungeonCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        YDungeonCreator dungeonCreator = (YDungeonCreator)target;
        if (GUILayout.Button("Create New Dungeon"))
        {
            dungeonCreator.CreateDungeon();
        }
    }   
}
