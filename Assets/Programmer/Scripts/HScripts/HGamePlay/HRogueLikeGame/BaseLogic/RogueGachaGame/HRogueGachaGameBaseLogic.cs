using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;

public class HRogueGachaGameBaseLogic : MonoBehaviour
{
    //模拟原神抽卡的代码
    private List<string> UpStar4Character = new List<string>();
    private string UpStar5Character = "AAAA";  //正在Up的五星ID，只读取第一个
    private List<string> Star5Characters = new List<string>();  //常驻池的五星角色
    private int drawCounter5Star = 0;
    private int drawCounter4Star = 0;
    private bool isLast5StarCharacterUp = true;
    private bool isLast4StarUp = true;
    private List<string> Star3Things = new List<string>();
    private List<string> Changzhu4StarThings = new List<string>();
    
    # region for UIs
    public Button getOneCardButton;
    public Button getTenCardButton;
    public GameObject showGachaAnimationPanel;
    public Button skipGachaAnimationButton;
    public GameObject showGachaResultPanel;
    public GameObject aSingleCatcakeShowPrefab;  //单抽展示的预制体
    
    public TMP_Text xingqiongNumText;
    public TMP_Text xinyongdianNumText;

    public GameObject gachaFiveStarUpPanel;
    public Transform Gacha4ItemLocation;
    # endregion


    private bool CouldGachaOrNot(int number)
    {
        //判断一下星穹的数量
        int xingqingCnt = HItemCounter.Instance.CheckCountWithItemId("20000012");
        if (xingqingCnt < number * 160)
        {
            return false;
        }

        return true;
    }

    private void UseXingqiongForGacha(int gachaCnt)
    {
        HItemCounter.Instance.RemoveItem("20000012",160 * gachaCnt);
        //显示星琼数和信用点数
        xingqiongNumText.text = HItemCounter.Instance.CheckCountWithItemId("20000012").ToString();
        xinyongdianNumText.text = HItemCounter.Instance.CheckCountWithItemId("20000013").ToString();
    }
    
    private void AddUIListeners()
    {
        getOneCardButton.onClick.AddListener(() =>
        {
            if (!CouldGachaOrNot(1))
            {
                HMessageShowMgr.Instance.ShowMessage("ROGUE_GACHA_NOENOUGH_XINGQIONG");
                return;
            }

            UseXingqiongForGacha(1);
            DrawCard(1);
        });
        getTenCardButton.onClick.AddListener(() =>
        {
            if (!CouldGachaOrNot(10))
            {
                HMessageShowMgr.Instance.ShowMessage("ROGUE_GACHA_NOENOUGH_XINGQIONG");
                return;
            }
            UseXingqiongForGacha(10);
            DrawCard(10);
        });
    }
    
    private void ReadGachaBaseInfo()
    {
        //显示星琼数和信用点数
        xingqiongNumText.text = HItemCounter.Instance.CheckCountWithItemId("20000012").ToString();
        xinyongdianNumText.text = HItemCounter.Instance.CheckCountWithItemId("20000013").ToString();
        //读取抽卡基本信息，包括UP角色，五星角色，四星角色等，以及抽卡记录
        var dictionary = SD_RogueGachaThingCSVFile.Class_Dic;
        foreach (var item in dictionary)
        {
            //不是当期有的东西，直接跳过
            if (item.Value.IsCurrentGachaItem == "0") continue;
            //是五星
            if (item.Value._RogueGachaItemStar() == 5)
            {
                if (item.Value.IsChangzhuThing == "1")  //是常驻五星
                {
                    Star5Characters.Add(item.Key);
                }
                else  //是Up五星
                {
                    UpStar5Character = item.Key;
                }
                
            }
            //是4星物品
            else if (item.Value._RogueGachaItemStar() == 4)
            {
                //4星Up角色
                if (item.Value.IsCurrentGachaUpItem == "1")
                {
                    UpStar4Character.Add(item.Key);
                }
                //4星常驻角色
                else if(item.Value.IsChangzhuThing == "1")
                {
                    Changzhu4StarThings.Add(item.Key);
                }
                
            }
            //是3星物品
            else if(item.Value._RogueGachaItemStar() == 3)
            {
                Star3Things.Add(item.Key);
            }
        }
    }
    
    private void UpdateUIInGachaPanel()
    {
        //更新抽卡界面的UI
        Sprite fiveStarUpSprite = Addressables.LoadAssetAsync<Sprite>(SD_RogueGachaThingCSVFile.Class_Dic[UpStar5Character].ItemLink).WaitForCompletion();
        gachaFiveStarUpPanel.GetComponent<Image>().sprite = fiveStarUpSprite;
        //更新四星Up角色
        for (int i = 0; i < Gacha4ItemLocation.childCount; i++)
        {
            string id = UpStar4Character[i];
            string link = SD_RogueGachaThingCSVFile.Class_Dic[id].ItemLink;
            Sprite sprite = Addressables.LoadAssetAsync<Sprite>(link).WaitForCompletion();
            Gacha4ItemLocation.GetChild(i).GetComponentInChildren<Image>().sprite = sprite;
        }
    }
    
    private void Start()
    {
        ReadGachaBaseInfo();
        UpdateUIInGachaPanel();
        AddUIListeners();
    }

    //抽卡的核心逻辑，num为抽卡次数
    public void DrawCard(int num)
    {
        List<string> result = new List<string>();
        bool has5StarItem = false;
        bool has4StarItem = false;
        for(int i = 0;i < num;i++)
        {
            Random.InitState((int)DateTime.Now.Ticks + Random.Range(0, 1000));
            float drawChance = Random.value;
            //抽到五星角色
            if (drawChance <= 0.016f || drawCounter5Star == 89)  
            {
                drawCounter5Star = 0;
                //抽到UP角色
                if (drawChance <= 0.008f || !isLast5StarCharacterUp)
                {
                    result.Add(UpStar5Character);
                    isLast5StarCharacterUp = true;
                }
                else
                {
                    //随机抽到五星角色
                    int randomIndex = Random.Range(0, Star5Characters.Count);
                    result.Add(Star5Characters[randomIndex]);
                    isLast5StarCharacterUp = false;
                }

                has5StarItem = true;
                drawCounter5Star = 0;
            }
            //抽到四星角色或武器
            else if(drawChance <= 0.13f|| drawCounter4Star == 9)
            {
                drawCounter4Star = 0;
                if (drawChance <= 0.065f || !isLast4StarUp || Changzhu4StarThings.Count == 0) //抽到UP角色,或者常驻池没有东西，也从up池抽吧
                {
                    int randomIndex = Random.Range(0, UpStar4Character.Count);
                    //Debug.Log("randomIndex: " + randomIndex + "  UpStar4Character.Count: " + UpStar4Character.Count);
                    result.Add(UpStar4Character[randomIndex]);
                    isLast4StarUp = true;
                }
                else
                {
                    int randomIndex = Random.Range(0, Changzhu4StarThings.Count);
                    result.Add(Changzhu4StarThings[randomIndex]);
                    isLast4StarUp = false;
                }

                has4StarItem = true;
                drawCounter5Star++;
                drawCounter4Star = 0;
            }
            //其它皆为三星武器
            else
            {
                drawCounter4Star++; 
                drawCounter5Star++;
                int randomIndex = Random.Range(0, Star3Things.Count);
                result.Add(Star3Things[randomIndex]);
            }
        }
        
        ShowResult(result, has5StarItem, has4StarItem);
    }
    
    private void ShowResult(List<string> result, bool hasFiveStar, bool hasFourStar)
    {
        if (hasFiveStar)
        {
            ShowAnimation(3, result);
        }
        else if (hasFourStar)
        {
            ShowAnimation(2, result);
        }
        else
        {
            ShowAnimation(1, result);
        }
    }

    private void ShowAnimation(int type, List<string> result)
    {
        string animationPath = "HonkaiGachaAnimation" + type;
        //播放动画，对应时长之后调出抽卡结果
        VideoClip clip = Addressables.LoadAssetAsync<VideoClip>(animationPath).WaitForCompletion();
        float clipLength = (float)clip.length;
        //播放动画
        showGachaAnimationPanel.GetComponent<VideoPlayer>().clip = clip;
        showGachaAnimationPanel.SetActive(true);
        showGachaAnimationPanel.GetComponent<VideoPlayer>().Play();
        
        //为跳过动画按钮绑定监听事件
        skipGachaAnimationButton.onClick.RemoveAllListeners();
        skipGachaAnimationButton.onClick.AddListener(() =>
        {
            showGachaAnimationPanel.SetActive(false);
            ShowGachaResultInPanel(result);
        });

        StartCoroutine(ShowGachaAnimation(clipLength, result));
    }

    IEnumerator ShowGachaAnimation(float clipLength, List<string> result)
    {
        yield return new WaitForSeconds(clipLength);
        showGachaAnimationPanel.SetActive(false);
        ShowGachaResultInPanel(result);
    }

    private bool playerLeftMouseDown = false;
    private bool couldDetectOtherInput = false;
    private List<string> currentResult = new List<string>();
    private int currentShowResultIndex = 0;
    private void Update()
    {
        if(couldDetectOtherInput && Input.GetMouseButtonDown(0))
        {
            couldDetectOtherInput = false;
            playerLeftMouseDown = true;
            Destroy(currentShowedGachaThing);
            currentShowedGachaThing = null;
            if(currentShowResultIndex >= currentResult.Count)
            {
                ShowAGachaResultWithId(-1);
                couldDetectOtherInput = false;
                return;
            }
            ShowAGachaResultWithId(currentShowResultIndex);
            currentShowResultIndex++;
        }
    }

    private void ShowGachaResultInPanel(List<string> result)
    {
        currentShowResultIndex = 0;
        skipGachaAnimationButton.onClick.RemoveAllListeners();
        StopAllCoroutines();
        showGachaResultPanel.SetActive(true);
        DOVirtual.DelayedCall(3f, () =>
        {
            couldDetectOtherInput = true;
        });
        
        currentResult = result;
        DebugGachaResultAndSave(result);
        
        ShowAGachaResultWithId(currentShowResultIndex);
        currentShowResultIndex++;
    }

    private void DebugGachaResultAndSave(List<string> result)
    {
        //输出抽卡结果
        Debug.Log("====================================");
        foreach (var res in result)
        {
            string name = SD_RogueGachaThingCSVFile.Class_Dic[res].Describe;
            int star = SD_RogueGachaThingCSVFile.Class_Dic[res]._RogueGachaItemStar();
            string type = SD_RogueGachaThingCSVFile.Class_Dic[res].RogueGachaItemType;
            string refId = SD_RogueGachaThingCSVFile.Class_Dic[res].RogueGachaItemFindID;
            if (star == 5)
            {
                Debug.Log("yes!!");
            }
            Debug.Log(name + "  星级: " + star);
            Debug.Log("===============" + drawCounter5Star);
            //todo:1.保存抽卡记录，写入一个文件当中，后面点击历史记录的时候会读取这个文件
        
            //todo:2,实时更新抽到的东西
            UpdateInfoToSave(type, refId);
        }
        Debug.Log("====================================");
        

    }

    private void UpdateInfoToSave(string type, string id)
    {
        switch (type)
        {
            case "ItemRogueCharacter":
                YCharacterInfoManager.SetStatusByID(id, true);
                break;
            case "ItemRogueMaomaogao":

                break;
        }
    }
    
    private GameObject currentShowedGachaThing = null;

    private void ShowAGachaResultWithId(int index)
    {
        if (index == -1)
        {
            //退出界面,todo:其实应该显示一下所有的结果，这个后面再做
            showGachaResultPanel.SetActive(false);
            return;
        }
        //展示某张卡在屏幕上
        string id = currentResult[index];
        string name = SD_RogueGachaThingCSVFile.Class_Dic[id].Describe;
        int star = SD_RogueGachaThingCSVFile.Class_Dic[id]._RogueGachaItemStar();
        string uiIconLink = SD_RogueGachaThingCSVFile.Class_Dic[id].ItemLink;
        currentShowedGachaThing = Instantiate(aSingleCatcakeShowPrefab, showGachaResultPanel.transform);
        Transform gachaThingUIShow = currentShowedGachaThing.transform.Find("CatcakeUISpace");
        GameObject catcakeIcon = gachaThingUIShow.Find("CatcakeIcon").gameObject;
        Transform gachaThingDescription = currentShowedGachaThing.transform.Find("UIDescriptionPart/DescriptionPanel");
        gachaThingUIShow.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetEase(Ease.Flash);
        GameObject gachaThingImage = gachaThingDescription.Find("UIImage").gameObject;
        Transform gachaThingName = gachaThingDescription.Find("Name");
        Transform gachaThingStar = gachaThingDescription.Find("Stars");
        gachaThingName.GetComponent<TMP_Text>().text = name;
        for(int i = 0;i < star;i++)
        {
            gachaThingStar.GetChild(i).gameObject.SetActive(true);
        }
        
        DOVirtual.DelayedCall(0.3f, () =>
        {
            gachaThingDescription.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
            gachaThingDescription.DOLocalMoveX(-420f, 0.2f).SetEase(Ease.InCirc);
            couldDetectOtherInput = true;
        });
        
        gachaThingImage.GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>(uiIconLink).WaitForCompletion();
        catcakeIcon.GetComponent<RawImage>().texture = gachaThingImage.GetComponent<Image>().sprite.texture;
    }
    
}
