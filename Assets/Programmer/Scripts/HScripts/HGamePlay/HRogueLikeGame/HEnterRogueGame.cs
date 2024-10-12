using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HEnterRogueGame : MonoBehaviour
{
    private Transform showCanvas;
    private Transform panel;

    private void Start()
    {
        showCanvas = transform.parent.Find("ShowCanvas");
        panel = showCanvas.Find("Panel");
        panel.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            panel.gameObject.SetActive(true);
        }
    }

    private void TriggerPanelToRogueGame()
    {
        //打开选择角色的界面
        YLevelManager.JustSetCurrentLevelIndex(3);
        yPlanningTable.Instance.EnterNewLevel(3);
        YGameRoot.Instance.Push(new YChooseCharacterPanel());
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            panel.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                HMessageShowMgr.Instance.ShowMessageWithActions("CONFIRM_ENTER_ROGUE_WITH_PORTAL", TriggerPanelToRogueGame, null,null);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            panel.gameObject.SetActive(false);
    }
}
