using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRogueDungeonManager : MonoBehaviour
{
    //用于控制每层的生成，保存每个关卡的生成信息
    //单例
    public static YRogueDungeonManager Instance;
    public YDungeonCreator dungeonCreator;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dungeonCreator = gameObject.GetComponent<YDungeonCreator>();
    }

    private int RogueLevel = 0;
    //getset
    public int GetRogueLevel()
    {
        return RogueLevel;
    }
    
    
    public void EnterNewLevel()
    {
        RogueLevel++;
        
        //生成新的关卡
        // dungeonCreator.CreateDungeon(RogueLevel);
        dungeonCreator.CreateDungeon();
        StartCoroutine(EnterNewLevelCoroutine());//等待加载完 ，后面用别的方法吧
    }
    IEnumerator EnterNewLevelCoroutine()
    {
        yield return new WaitForSeconds(1f);
        //生成新的关卡
        //将角色设置在新的关卡的起始位置，出生房
        YPlayModeController.Instance.SetRogueCharacterPlace();
        dungeonCreator.BakeNavMesh();
        //将加载界面关闭
        YGameRoot.Instance.Pop();
    }
    
}
