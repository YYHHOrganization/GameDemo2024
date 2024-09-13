using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRogueToturialEnterItem :YRogue_TriggerGame
{
    protected GameObject getUIHold;
    private bool hasEnter = false;
    protected void Start()
    {
        base.Start();
        getUIHold = transform.Find("ShowCanvasHold/Panel").gameObject;
        getUIHold.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
        getUIHold.gameObject.SetActive(true);
    }
    protected override void StartInteract()
    {
        if (hasEnter) return;
        if (!shouldAliveAfterGame)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
        
        Debug.Log("YRogueTutorialEnterItem");
        //GameObject tutorialScene = Addressables.LoadAssetAsync<GameObject>("TutorialScenePrefab").WaitForCompletion();
        //GameObject tutorialSceneInstance = Instantiate(tutorialScene);
        //寻找脚本YSpecialMapTutorial
        //YSpecialMapTutorial ySpecialMapTutorial = tutorialSceneInstance.gameObject.GetComponent<YSpecialMapTutorial>();
        YSpecialMapTutorial ySpecialMapTutorial = FindObjectOfType<YSpecialMapTutorial>();
        if (ySpecialMapTutorial)
        {
            hasEnter = true;
            RutPainter painter = ySpecialMapTutorial.gameObject.GetComponentInChildren<RutPainter>();
            if (painter)
            {
                painter.Initialize();
            }
            //存储原来的位置  方便后续传送
            Transform playertransform = YPlayModeController.Instance.curCharacter.transform;
            YRogueDungeonManager.Instance.SetTransferPlace(playertransform);
            Debug.Log(playertransform.position+"存储原来的位置");
            
            Transform playerBornPlace = ySpecialMapTutorial.playerBornPlace;
            
            DOVirtual.DelayedCall(0.5f, () =>
            {
                //将玩家传送到指定位置
                //YPlayModeController.Instance.SetRogueCharacterPlace(playerBornPlace.position);
                YPlayModeController.Instance.SetRogueCharacterPlaceWithNoCatcake(playerBornPlace.position);
                ySpecialMapTutorial.playerEnterTutorial();
            });
            // ROGUE_TUTORIAL_SCENE_LOAD
            HMessageShowMgr.Instance.ShowMessage("ROGUE_TUTORIAL_SCENE_LOAD" );
            gameObject.SetActive(false);
        }
        
        
    }
}
