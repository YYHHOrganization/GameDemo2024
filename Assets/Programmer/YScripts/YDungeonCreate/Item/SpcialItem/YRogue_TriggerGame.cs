using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRogue_TriggerGame : MonoBehaviour
{
    GameObject getUI;
    // public string gameName;
    public string GameLinkPrefab;
    public string MessageLink;//转场的时候显示的message
    public bool shouldAliveAfterGame = true;
    private void Start()
    {
        getUI = transform.Find("ShowCanvas/Panel").gameObject;
        getUI.gameObject.SetActive(false);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            getUI.gameObject.SetActive(true);
            getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
        }
    }
    bool isInteract = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.Q))
            {
                //应该每0.1scheck一下，不然容易点击一下 扣一堆
                if(isInteract) return;
                isInteract = true;
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    isInteract = false;
                });
                
                StartInteract();
            }
        }
    }

    protected virtual void StartInteract()
    {
        //黑屏 默认弹出message
        HMessageShowMgr.Instance.ShowMessage(MessageLink);
        getUI.gameObject.SetActive(false);
        if (shouldAliveAfterGame)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
        DOVirtual.DelayedCall(1.5f, () =>
        {
            LoadGame();
        });
    }
    void LoadGame()
    {
        // GameLinkPrefab
        GameObject gamePre = Addressables.InstantiateAsync(GameLinkPrefab, transform).WaitForCompletion();
        gamePre.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            getUI.gameObject.SetActive(false);
        }
    }

}
