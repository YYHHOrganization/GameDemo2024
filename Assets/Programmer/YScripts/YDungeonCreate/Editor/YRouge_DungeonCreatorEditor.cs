using System.Collections;
using System.Collections.Generic;
using DG.DOTweenEditor;
using DG.Tweening;
using UnityEngine;
using UnityEditor;

// 使用CustomEditor属性，
// 告诉Unity这个类是用来自定义YDungeonCreator类在Inspector面板的显示方式的
[CustomEditor(typeof(YDungeonCreator))]
public class YRouge_DungeonCreatorEditor : Editor
{
    // 重写OnInspectorGUI方法，这个方法会在Inspector面板中绘制UI
    public override void OnInspectorGUI()
    {
        // 调用基类的OnInspectorGUI方法，会绘制出YDungeonCreator类的所有公开属性
        base.OnInspectorGUI();
        // 获取当前被选中的YDungeonCreator对象
        YDungeonCreator dungeonCreator = (YDungeonCreator)target;
        // 在Inspector面板中添加一个按钮，按钮的文本是"Create New Dungeon"
        if (GUILayout.Button("Create New Dungeon"))
        {
            // 当按钮被点击时，调用dungeonCreator的CreateDungeon方法
            dungeonCreator.CreateDungeon();
        }
        if (GUILayout.Button("Create New Dungeon And Icon"))
        {
            dungeonCreator.CreateDungeon(0, true);
            // //Delay 1s
            // System.Threading.Thread.Sleep(5000);
            // dungeonCreator.TestGenerateAllLittleMapMask();
        }
    }   
   
}
