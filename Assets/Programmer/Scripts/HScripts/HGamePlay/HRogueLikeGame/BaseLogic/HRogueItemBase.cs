using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class HRogueItemBase : MonoBehaviour
{
    //肉鸽玩法的物品道具基类，其他物品道具继承于这个基类
    private string itemId;
    private RogueItemBaseAttribute rogueItemBaseAttribute;

    private GameObject getUI;

    private bool isPickedUp;

    private TMP_Text itemChineseName;
    private bool canShowName = false;
    
    public void SetItemIDAndShow(string id, RogueItemBaseAttribute rogueItemAttribute)
    {
        itemId = id;
        rogueItemBaseAttribute = rogueItemAttribute;
        getUI = transform.Find("ShowCanvas/Panel").gameObject;
        getUI.gameObject.SetActive(false);
        isPickedUp = false;
        itemChineseName = getUI.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public virtual void GetToBagAndShowEffects()
    {
        if (rogueItemBaseAttribute.rogueItemShowInBag) //需要显示在背包当中
        {
            HItemCounter.Instance.AddItemInRogue(itemId, 1);
        }
        //根据主动道具还是被动道具决定使用效果
        if (rogueItemBaseAttribute.rogueItemKind == "Negative") //被动道具，立刻生效
        {
            string funcName = rogueItemBaseAttribute.rogueItemFunc;
            string funcParams = rogueItemBaseAttribute.rogueItemFuncParams;
            UseNegativeItem(funcName, funcParams);
            HMessageShowMgr.Instance.ShowMessage("ROGUE_USE_NEGATIVE_ITEM", "你使用了被动道具——"+rogueItemBaseAttribute.itemChineseName);
            SetActiveFalseAndDestroy(5f);
        }
        else if (rogueItemBaseAttribute.rogueItemKind == "Positive")
        {
            if (!rogueItemBaseAttribute.rogueItemUSEInScreen)
            {
                SetActiveFalseAndDestroy(0.5f);
            }
            else
            {
                //这种主动道具需要在屏幕上显示，等待玩家使用
                //HRoguePlayerAttributeAndItemManager.Instance.AddScreenPositiveItem(itemId);
                SetActiveFalseAndDestroy(0.5f);
            }
        }
    }
    

    private void SetActiveFalseAndDestroy(float time = 0f)
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject, time);
    }

    protected virtual void UseNegativeItem(string funcName, string funcParams)
    {
        string[] funcs = funcName.Split(':');
        string[] funcParamArray = funcParams.Split(':');
        for(int i=0;i<funcs.Length;i++)
        {
            HRogueItemFuncUtility.Instance.UseNegativeItem(funcs[i], funcParamArray[i]);
        }
    }
    

    public void SetBillboardEffect()
    {
        if (!getUI.GetComponentInParent<HRotateToPlayerCamera>()) return;
        getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
            getUI.gameObject.SetActive(true);
            if (itemChineseName)
            {
                if (rogueItemBaseAttribute.rogueItemNameShowDefault)
                {
                    itemChineseName.text = rogueItemBaseAttribute.itemChineseName;
                }
                else
                {
                    itemChineseName.text = "???";
                }
                
            }
            if (!isPickedUp && Input.GetKey(KeyCode.F))
            {
                GetToBagAndShowEffects();
                isPickedUp = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isPickedUp && Input.GetKey(KeyCode.F))
            {
                GetToBagAndShowEffects();
                isPickedUp = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            getUI.gameObject.SetActive(false);
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
