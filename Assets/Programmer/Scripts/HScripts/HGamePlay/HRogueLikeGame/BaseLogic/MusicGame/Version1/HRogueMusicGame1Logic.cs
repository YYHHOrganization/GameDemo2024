using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class HRogueMusicGame1Logic : MonoBehaviour
{
    public GameObject insGO;
    public Transform floorUpOrigin;
    public Transform floorDownOrigin;
    public Transform floorUp;
    public Transform floorDown;
    private int gameMode = 1; //1模式是最简单的音游模式，后面再来一个纯跑酷的模式吧，有三条命，被炸弹炸掉就减一条命

    private HRogueCharacterInMusicGameVer1 characterScript;
    public Transform bgImage1;
    public Transform bgImage2;

    public GameObject badEnemyInsGo; //障碍物/扣分物体
    private double totalSongSeconds = 0;
    private List<string> songList = new List<string>();
    private Koreography thisSong;

    public Transform creatureRoot; //角色或者其他生成生物的根节点
    
    void ChooseASong()
    {
        songList.Add("ZhongmuSong");
        songList.Add("MojingSong");
        songList.Add("AixiMusic");
        int randomIndex = 2;
        thisSong = Addressables.LoadAssetAsync<Koreography>(songList[randomIndex]).WaitForCompletion();
        Koreographer.Instance.LoadKoreography(thisSong);
    }
    
    void Start()
    {
        //RegisterForEvents(string eventID, KoreographyEventCallback callback)
        //第一个参数是我们上面命名的Track Event ID 例如我的是 TestEventID
        ChooseASong();
        Koreographer.Instance.RegisterForEvents("TestEventID", FireEventDebugLog);
        Koreographer.Instance.RegisterForEvents("EventForEffects", FireEventEffects);
        LoadGame();
    }
    
    private void FireEventEffects(KoreographyEvent koreoEvent)
    {
        if(koreoEvent.HasIntPayload())
        {
            int value = koreoEvent.GetIntValue();
            SetGameEffectWithValue(value);
        }
    }

    private void ResetEveryEffects()
    {
        ScriptableRendererFeature feature1 =
            HPostProcessingFilters.Instance.GetRenderFeature("FullScreenDoubleBonus");
        feature1.SetActive(false);
        ScriptableRendererFeature feature2 =
            HPostProcessingFilters.Instance.GetRenderFeature("FullScreenInvincible");
        feature2.SetActive(false);
        characterScript.SetScoreMultiplier(1);
        characterScript.SetVincible(false);
    }

    IEnumerator RotateTwoCharacters()
    {
        for (int i = 0; i < 4; i++)
        {
            Transform child1 = creatureRoot.GetChild(0);
            child1.DOLocalRotate(new Vector3(0, 90, -20), 0.8f).OnComplete(() =>
            {
                child1.DOLocalRotate(new Vector3(0, 90, 20), 0.8f);
            });
            Transform child2 = creatureRoot.GetChild(1);
            child2.DOLocalRotate(new Vector3(0, 90, -20), 0.8f).OnComplete(() =>
            {
                child2.DOLocalRotate(new Vector3(0, 90, 20), 0.8f);
            });
            yield return new WaitForSeconds(1.7f);
        }
    }

    public void GiveOutTwoCharactersForCheer()
    {
        //生成两个荧妹，加油助威
        //dotween, creatureRoot先往左移动5个单位，然后creatureRoot下的节点左右摆动旋转四次，然后再向右移动5个单位
        creatureRoot.transform.DOLocalMoveZ(1.5f, 1f).OnComplete(
            () =>
            {
                StartCoroutine(RotateTwoCharacters());
                DOVirtual.DelayedCall(7f, () =>
                {
                    creatureRoot.transform.DOLocalMoveZ(2.5f, 1f).OnComplete(() =>
                    {
                        for(int i = 0; i < creatureRoot.childCount; i++)
                        {
                            int j = i;
                            Transform child = creatureRoot.GetChild(j);
                            child.DOLocalRotate(new Vector3(0, 90, 0), 0.1f);
                        }
                    });
                });
            }
            );
    }

    private void SetGameEffectWithValue(int value)
    {
        string name = "FullScreenDoubleBonus";
        if (value == 0)
        {
            ResetEveryEffects();
            return;
        } 
        else if (value == 3)
        {
            GiveOutTwoCharactersForCheer();
            return;
        }

        if (value == 1)
        {
            name = "FullScreenDoubleBonus";
            characterScript.SetScoreMultiplier(2);
            HMessageShowMgr.Instance.ShowMessage("ROGUE_MUSICGAME_SCORE_TWO");
        }
        else if (value == 2)
        {
            name = "FullScreenInvincible";
            characterScript.SetVincible(true);
            HMessageShowMgr.Instance.ShowMessage("ROGUE_MUSICGAME_INVINCIBLE");
        }
        ScriptableRendererFeature feature =
            HPostProcessingFilters.Instance.GetRenderFeature(name);
        feature.SetActive(true);
    }

    private void LoadGame()
    {
        SetExitPanelFalse();
        YPlayModeController.Instance.LockPlayerInput(true);
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        Invoke("SetGamePanelActive", 2.5f);
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
    }

    private void SetGamePanelActive()
    {
        characterScript = GetComponentInChildren<HRogueCharacterInMusicGameVer1>();
        characterScript.InstantiateGameScorePanel(this);
    }

    public bool testGameStart = false;
    private bool startMovingPlatform = false;

    //private int fireIndex = 0;
    private int goodTotalCount = 0;  //生成好的道具的总数
    
    private void SummonRandomItem()
    {
        int randomKind = Random.Range(0, 100);
        bool isBad = false;
        GameObject summonObj;
        if (randomKind < 50)  //是好的道具
        {
            summonObj = insGO;
            goodTotalCount++;
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
        //obj.gameObject.name = "Item" + fireIndex;
        //fireIndex++;
        obj.gameObject.tag = isBad? "Enemy":"Player";
        Destroy(obj, 10f);
    }

    private void SummonItemsWithPayloadValue(int value)
    {
        if (value == 0)
        {
            SummonRandomItem();
            return;
        }
        bool isBad = false;
        GameObject summonObj;
        GameObject obj;
        //1：上轨道好，2：上轨道坏；3：下轨道好，4:下轨道坏，5：上轨道长条，6：下轨道长条
        if (value == 2 || value == 4)
        {
            summonObj = badEnemyInsGo;
            isBad = true;
        }
        else
        {
            summonObj = insGO;
            goodTotalCount++;
            isBad = false;
        }

        if (value == 1 || value == 2)
        {
            obj = Instantiate(summonObj, floorUpOrigin.position, Quaternion.identity, floorUp);
            obj.gameObject.tag = isBad ? "Enemy" : "Player";
            Destroy(obj, 10f);
        }
        else if (value == 3 || value == 4)
        {
            obj = Instantiate(summonObj, floorDownOrigin.position, Quaternion.identity, floorDown);
            obj.gameObject.tag = isBad ? "Enemy" : "Player";
            Destroy(obj, 10f);
        }
        else if (value == 5) //上轨道长条
        {
            obj = Instantiate(summonObj, floorUpOrigin.position, Quaternion.identity, floorUp);
            obj.gameObject.tag = "CouldHit";
            Destroy(obj, 10f);
        }
        else if (value == 6) //下轨道长条
        {
            obj = Instantiate(summonObj, floorDownOrigin.position, Quaternion.identity, floorDown);
            obj.gameObject.tag = "CouldHit";
            Destroy(obj, 10f);
        }
        
}
    
    void FireEventDebugLog(KoreographyEvent koreoEvent)
    {
        if(koreoEvent.HasIntPayload())
        {
            int value = koreoEvent.GetIntValue();
            Debug.Log("Fired Event with value: " + value);
            //根据Value值来给出内容
            SummonItemsWithPayloadValue(value);
        }
        else
        {
            SummonRandomItem();
        }
        
    }

    private void MoveTwoPlatforms()
    {
        floorUp.position += Vector3.left * 0.15f;
        floorDown.position += Vector3.left * 0.15f;
    }
    

    public void StartGameLogic()
    {
        GetComponent<SimpleMusicPlayer>().LoadSong(thisSong);
        GetComponent<SimpleMusicPlayer>().Play();
        totalSongSeconds = Koreographer.Instance.GetMusicSecondsLength();
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

    private float gameAccruacy = 0;
    private void SetThisGameOver()
    {
        //计算一下玩家的准度，然后显示出来
        int pickUpCount = characterScript.GoodPickupCount;
        gameAccruacy = (float)pickUpCount / goodTotalCount;
        string overrideContent = "您的准确率为： " + gameAccruacy.ToString("P");
        characterScript.ShowGameOverEffect();
        HMessageShowMgr.Instance.ShowMessageWithActions("ROGUE_MUSICGAME_Grade", EndGame, EndGame,EndGame, null, overrideContent);
    }

    private void EndGame()
    {
        //todo:退出游戏界面，依据分数给出宝箱
        HRoguePlayerAttributeAndItemManager.Instance.SetAttributePanel(true);
        HCameraLayoutManager.Instance.SetLittleMapCamera(true);
        characterScript.DestroyGameScorePanel();
        GiveOutTreasure(gameAccruacy);
        YPlayModeController.Instance.LockPlayerInput(false);
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        HAudioManager.Instance.Play("StartRogueAudio", HAudioManager.Instance.gameObject);
        Destroy(gameObject, 1f);
    }
    
    private void GiveOutTreasure(float accuracy)
    {
        Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        Vector3 treasurePos = player.position;
        string chestID;
        //根据awaardGrade生成奖励
        if (accuracy<=0.5f)
        {
            return;
        }
        else if (accuracy<=0.7f)
        {
            chestID = "10000013";
        }
        else if (accuracy<=0.9f)
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
