using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    private void ClearPlayerRelativeAssets()
    {
        //清除玩家的相关资源, 比如已经在场景当中的player
        YPlayModeController.Instance.ClearPlayerRelativeAssets();
    }

    private void TriggerPanelToRogueGame()
    {
        //打开选择角色的界面
        ClearPlayerRelativeAssets();
        YLevelManager.JustSetCurrentLevelIndex(3);
        yPlanningTable.Instance.EnterNewLevel(3);
        YGameRoot.Instance.Push(new YChooseCharacterPanel());
    }

    private bool canInteractive = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!canInteractive) return;
            panel.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                HMessageShowMgr.Instance.ShowMessageWithActions("CONFIRM_ENTER_ROGUE_WITH_PORTAL", TriggerPanelToRogueGame, null,null);
                canInteractive = false;
                DOVirtual.DelayedCall(2f, () => { canInteractive = true; });
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            panel.gameObject.SetActive(false);
    }
}
