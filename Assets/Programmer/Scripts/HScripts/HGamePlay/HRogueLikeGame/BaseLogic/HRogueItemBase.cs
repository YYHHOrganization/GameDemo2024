using System;
using System.Collections;
using System.Collections.Generic;
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
        getUI = GetComponentInChildren<Canvas>().gameObject;
        getUI.gameObject.SetActive(false);
        isPickedUp = false;
        itemChineseName = getUI.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }

    protected virtual void GetToBagAndShowEffects()
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
        }
        else if (rogueItemBaseAttribute.rogueItemKind == "Positive")
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject, 0.5f);
        }
    }

    protected virtual void UseNegativeItem(string funcName, string funcParams)
    {
        //利用反射调用函数funcName，传递参数funcParams
        System.Reflection.MethodInfo method = this.GetType().GetMethod(funcName);
        method.Invoke(this, new object[] {funcParams});
    }
    
    public void AddAttributeValue(string funcParams)
    {
        Debug.Log("AddAttributeValue");
        //根据funcParams的内容，将对应的属性值加上对应的数值
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        float attributeValue = float.Parse(paramList[1]);
        HRougeAttributeManager.Instance.AddAttributeValue(attributeName, attributeValue);
        this.gameObject.SetActive(false);
        Destroy(this.gameObject, 0.5f);
    }

    public void AddHeartOrShield(string funcParams)
    {
        //根据funcParams的内容，决定加血/加护盾，或者是加血量上限/加护盾上限
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        int attributeValue = int.Parse(paramList[1]);
        if (HRougeAttributeManager.Instance.AddHeartOrShield(attributeName, attributeValue))
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject, 0.5f);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            getUI.GetComponentInChildren<HRotateToPlayerCamera>().enabled = true;
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
