using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class YStatueXuyuan :MonoBehaviour
{
    GameObject getUI;
    private bool isPickedUp;
    private TMPro.TMP_Text itemChineseName;
    
    //购买的货币
    public string buyCurrency;
    public string BuyCurrency => buyCurrency; 
    public int HowMuch => howMuch;
    //货币数量 即价格
    public int howMuch;
    
    public TMP_Text PriceTextUI;
    public Transform WishingSuccessItemAppearPlace;
    private void Start()
    {
        getUI = transform.Find("ShowCanvas/Panel").gameObject;
        getUI.gameObject.SetActive(false);
        SetPriceUI();
        isPickedUp = false;
        itemChineseName = getUI.transform.GetChild(1).GetComponent<TMPro.TMP_Text>();
    }

    private string buycurrencyChineseName = "";
    void SetPriceUI()
    {
        //buyCurrency = "20000013";//"信用点";
        //看yPlanningTable.Instance.worldItems是否有这个buyCurrency
        string priceStr = "";
        if (yPlanningTable.Instance.worldItems.ContainsKey(buyCurrency))
        {
            priceStr = yPlanningTable.Instance.worldItems[buyCurrency].chineseName;
        }
        else
        {
            priceStr = yPlanningTable.Instance.rogueItemBases[buyCurrency].itemChineseName;
        }
        buycurrencyChineseName = priceStr;

        SetUIPriceChange();
    }
    void SetUIPriceChange()
    {
        string priceStr = buycurrencyChineseName;
        priceStr = "<size=50%>" + priceStr + "</size>";
        priceStr = "<size=100%>"+ howMuch + "</size>" + priceStr;
        PriceTextUI.text = priceStr;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
            getUI.gameObject.SetActive(true);
        }
    }
    bool isWishing = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isPickedUp && Input.GetKey(KeyCode.F))
            {
                //应该每0.1scheck一下，不然容易点击一下 扣一堆
                if(isWishing) return;
                isWishing = true;
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    isWishing = false;
                });
                
                CheckAndWishing();
            }
        }
    }

    private void CheckAndWishing()
    {
        Debug.Log("Wishing");
        //check 钱够不够
        OnCheckShopItemEnough(buyCurrency, howMuch);
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            getUI.gameObject.SetActive(false);
        }
    }
    
    void OnCheckShopItemEnough(string itemId, int count)
    {
        //如果是商店，那么就要 点击了F直接购买，扣除钱
        //同时应该判断钱是否够
        int countBag = HItemCounter.Instance.CheckCountWithItemId(itemId);
        bool isEnough = countBag >= count;
        if (isEnough)
        {
            Wishing();
            //GetToBagAndShowEffects(); Wishing();
            
        }
        else
        {
            HMessageShowMgr.Instance.ShowMessage("ROGUE_SHOP_NO_MONEY", "对不起货币不足，无法许愿！");
        }
    }
    int wishingBaseRate = 10;
    int wishingRate = 10;
    int wishingRateStep = 10;
    
    private void Wishing()
    {
        //钱-howmuch
        HMessageShowMgr.Instance.ShowMessage("ROGUE_SHOP_NO_MONEY", buycurrencyChineseName + "-" + howMuch);
        //扣钱
        HItemCounter.Instance.RemoveItemInRogueAndWorld(buyCurrency, howMuch);
        //增加价格*2
        howMuch *= 2;
        SetUIPriceChange();
        //许愿
        //许愿成功的基础概率是n,每次许愿都会增加一点概率n+nStep,直到许愿成功,许愿成功后概率重置
        bool isWishingSuccess = Random.Range(0, 100) < wishingRate;
        if (isWishingSuccess)
        {
            //许愿成功
            WishingSuccess();
            wishingRate = wishingBaseRate;
        }
        else
        {
            wishingRate += wishingRateStep;
        }
    }

    private void WishingSuccess()
    {
        HMessageShowMgr.Instance.ShowMessage("ROGUE_SHOP_NO_MONEY", "许愿成功！获得奖励！");
        Debug.Log("许愿成功");
        // isPickedUp = true; 如果许愿到不能许愿了 他会爆炸之类的  或者消散 乘筋斗云离开？
        Vector3 bias = new Vector3(Random.Range(-2f, 2f), 10, Random.Range(0.2f, 4f));
        Vector3 oribias = new Vector3(bias.x, 0.0f, bias.z);
        GameObject wishingItem 
            =HRoguePlayerAttributeAndItemManager.Instance.RollingARandomItem(WishingSuccessItemAppearPlace,bias);
        //这个物品从天而降 从初始位置到wishingsuccessitemappearplace
        wishingItem.transform.DOMove(WishingSuccessItemAppearPlace.position+oribias, 1.5f).SetEase(Ease.OutBounce);
        
    }
}
