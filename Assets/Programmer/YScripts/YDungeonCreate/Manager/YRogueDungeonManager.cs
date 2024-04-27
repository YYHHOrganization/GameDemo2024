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
    private int RogueLevel = -1;

    public bool flagGameBegin;//游戏是否开始,防止蜘蛛直接开始追角色了
    public Vector3 RogueDungeonOriginPos = new Vector3(-400, 0, -400);
    public Vector2Int RogueDungeonOriginPos2 = new Vector2Int(-400, -400);
    
    Transform RogueBornPlace;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dungeonCreator = gameObject.GetComponent<YDungeonCreator>();
        flagGameBegin = true;
    }

    

    //getset
    public int GetRogueLevel()
    {
        return RogueLevel;
    }
    
    
    public void EnterNewLevel()
    {
        flagGameBegin = false;
        RogueLevel++;
        //角色先飞到天上休息下？
        YPlayModeController.Instance.SetRogueCharacterPlaceFarAway();

        //生成新的关卡
        // dungeonCreator.CreateDungeon(RogueLevel);

        // if (RogueLevel == 0)
        //     dungeonCreator.CreateDungeon(1);
        // else
        //     dungeonCreator.CreateDungeon();
        
        dungeonCreator.CreateDungeon();
        
        StartCoroutine(EnterNewLevelCoroutine());//等待加载完 ，后面用别的方法吧
    }
    public float loadFakeTime = 5f;
    IEnumerator EnterNewLevelCoroutine()
    {
        yield return new WaitForSeconds(loadFakeTime);
        flagGameBegin = true;
        //生成新的关卡
        //将角色设置在新的关卡的起始位置，出生房
        // YPlayModeController.Instance.SetRogueCharacterPlace();
        YPlayModeController.Instance.SetRogueCharacterPlace(RogueBornPlace);
        
        dungeonCreator.BakeNavMesh();
        //将加载界面关闭
        YGameRoot.Instance.Pop();
    }

    //再角色生成的时候调用 让游戏开始 可以判断蜘蛛是否开始追角色
    public void SetflagGameBegin(bool b)
    {
        flagGameBegin = b;
    }
    
    
    //存储当前所有房间的script
    public List<YRouge_RoomBase> roomBaseLists;
    public List<YRouge_RoomBase> GetRoomBaseList()
    {
        return dungeonCreator.GetRoomBaseList();
    }

    public void SetRogueBornPlace(Transform transformPosition)
    {
        RogueBornPlace = transformPosition;
    }
    
    
}
