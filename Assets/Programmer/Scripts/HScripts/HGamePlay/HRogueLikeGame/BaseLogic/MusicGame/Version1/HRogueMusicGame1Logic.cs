using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class HRogueMusicGame1Logic : MonoBehaviour
{
    public GameObject insGO;
    public Transform floorUpOrigin;
    public Transform floorDownOrigin;
    public Transform floorUp;
    public Transform floorDown;
    private int gameMode = 1; //1模式是最简单的收集物品的模式，后面再引入其他的比如躲避障碍物的模式

    private HRogueCharacterInMusicGameVer1 characterScript;
    public Transform bgImage1;
    public Transform bgImage2;

    public GameObject badEnemyInsGo; //障碍物/扣分物体
    private double totalSongSeconds = 0;
    void Start()
    {
        //RegisterForEvents(string eventID, KoreographyEventCallback callback)
        //第一个参数是我们上面命名的Track Event ID 例如我的是 TestEventID
        Koreographer.Instance.RegisterForEvents("TestEventID", FireEventDebugLog);
        //totalSongSeconds = Koreographer.Instance.GetMusicSecondsLength();
        totalSongSeconds = 20f;
        LoadGame();
    }

    private void LoadGame()
    {
        SetExitPanelFalse();
        YPlayModeController.Instance.LockPlayerInput(true);
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        characterScript = GetComponentInChildren<HRogueCharacterInMusicGameVer1>();
        characterScript.InstantiateGameScorePanel(this);
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
    }

    public bool testGameStart = false;
    private bool startMovingPlatform = false;
    
    void FireEventDebugLog(KoreographyEvent koreoEvent)
    {
        // if(koreoEvent.HasIntPayload())
        // {
        //     Debug.Log("IntPayload: " + koreoEvent.GetIntValue());
        // }
        int randomKind = Random.Range(0, 100);
        bool isBad = false;
        GameObject summonObj;
        if (randomKind < 50)  //是好的道具
        {
            summonObj = insGO;
            isBad = false;
        }
        else
        {
            summonObj = badEnemyInsGo;
            isBad = true;
        }
        
        int randomNumber = Random.Range(0, 100);
        GameObject obj;
        if (randomNumber < 50)
        {
            obj = Instantiate(summonObj, floorUpOrigin.position, Quaternion.identity, floorUp);
        }
        else
        {
            obj = Instantiate(summonObj, floorDownOrigin.position, Quaternion.identity, floorDown);
        }
        
        obj.gameObject.tag = isBad? "Enemy":"Player";
        Destroy(obj, 10f);
    }

    private void MoveTwoPlatforms()
    {
        floorUp.position += Vector3.left * 0.15f;
        floorDown.position += Vector3.left * 0.15f;
    }

    private void Update()
    {
        // if (testGameStart)
        // {
        //     StartGameLogic();
        // }
    }

    public void StartGameLogic()
    {
        GetComponent<SimpleMusicPlayer>().Play();
        HAudioManager.Instance.StopAllAudio();
        startMovingPlatform = true;
        testGameStart = false;
        HMessageShowMgr.Instance.ShowTickMessage("歌曲倒计时：", (int)totalSongSeconds, SetThisGameOver);
        Debug.Log("tO1111!!!!!!" + totalSongSeconds);
    }
    
    private void SetExitPanelFalse()
    {
        //主要是角色的属性面板要保持关闭，还有小地图
        HRoguePlayerAttributeAndItemManager.Instance.SetAttributePanel(false);
        HCameraLayoutManager.Instance.SetLittleMapCamera(false);
    }

    private void SetThisGameOver()
    {
        //todo:显示最终的分数和成绩，二次确认窗口
        HMessageShowMgr.Instance.ShowMessageWithActions("ROGUE_MUSICGAME_Grade", EndGame, EndGame,EndGame);
    }

    private void EndGame()
    {
        //todo:退出游戏界面，依据分数给出宝箱
        HRoguePlayerAttributeAndItemManager.Instance.SetAttributePanel(true);
        HCameraLayoutManager.Instance.SetLittleMapCamera(true);
        int score = characterScript.DestroyGameScorePanel();
        GiveOutTreasure(score);
        YPlayModeController.Instance.LockPlayerInput(false);
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        HAudioManager.Instance.Play("ResumeAllAudio", HAudioManager.Instance.gameObject);
        Destroy(gameObject, 1f);
    }
    
    private void GiveOutTreasure(int score)
    {
        Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        Vector3 treasurePos = player.position;
        string chestID;
        //根据awaardGrade生成奖励
        if (score<=-1)
        {
            return;
        }
        else if (score<=3)
        {
            chestID = "10000013";
        }
        else if (score<=5)
        {
            chestID = "10000011";
        }
        else
        {
            chestID = "10000012";
        }
        
        HOpenWorldTreasureManager.Instance.InstantiateATreasureAndSetInfoWithTypeId(chestID, treasurePos, transform.parent);
        
        
    }
    private void FixedUpdate()
    {
        if (startMovingPlatform)
        {
            MoveTwoPlatforms();
            CycleBackgroundImage();
        }
    }
    
    //背景循环播放的动画
    bool currentCheckbg1 = false;
    private void CycleBackgroundImage()
    {
        if (currentCheckbg1)
        {
            if (bgImage1.localPosition.z <= 8f)
            {
                bgImage2.localPosition = new Vector3(-0.9f, 1.03f, 28.3f);
                currentCheckbg1 = false;
            }
        }
        else
        {
            if (bgImage2.localPosition.z <= 8f)
            {
                bgImage1.localPosition = new Vector3(-0.9f, 1.03f, 28.3f);
                currentCheckbg1 = true;
            }
        }
        bgImage1.position += Vector3.left * 0.03f;
        bgImage2.position += Vector3.left * 0.03f;
    }
    
}
