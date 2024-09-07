using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRogueToturialEnterItem :YRogue_TriggerGame
{
    protected GameObject getUIHold;
    protected void Start()
    {
        base.Start();
        getUIHold = transform.Find("ShowCanvasHold/Panel").gameObject;
        getUIHold.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
        getUIHold.gameObject.SetActive(true);
    }
    protected override void StartInteract()
    {
        if (!shouldAliveAfterGame)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
        
        Debug.Log("YRogueTutorialEnterItem");
        //寻找脚本YSpecialMapTutorial
        YSpecialMapTutorial ySpecialMapTutorial = FindObjectOfType<YSpecialMapTutorial>();
        if (ySpecialMapTutorial)
        {
            //存储原来的位置  方便后续传送
            Transform playertransform = YPlayModeController.Instance.curCharacter.transform;
            YRogueDungeonManager.Instance.SetTransferPlace(playertransform);
            Debug.Log(playertransform.position+"存储原来的位置");
            
            Transform playerBornPlace = ySpecialMapTutorial.playerBornPlace;
            //将玩家传送到指定位置
            //YPlayModeController.Instance.SetRogueCharacterPlace(playerBornPlace.position);
            YPlayModeController.Instance.SetRogueCharacterPlaceWithNoCatcake(playerBornPlace.position);
            ySpecialMapTutorial.playerEnterTutorial();
        }
        
        gameObject.SetActive(false);
    }
}
