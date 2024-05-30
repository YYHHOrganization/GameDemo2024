using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.HID;
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

    public Button showHistoryButton;
    public Transform gachaContentTrans;
    public GameObject gachaHistoryPanel;
    public Button closeGachaHistoryButton;
    public Button gachaHistoryNextPageButton;
    public Button gachaHistoryPrePageButton;
    public TMP_Text gachaHistoryPageText;
    # endregion

    private int maxGachaHistoryPage = 0;
    string gachaSaveXmlPath = Application.dataPath + "/Designer/XMLTable/gachaHistory.xml";
    //加载抽卡的xml文件
    XmlDocument xmlDoc = new XmlDocument();

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
        
        showHistoryButton.onClick.AddListener(() =>
        {
            currentPage = 0;
            ShowGachaHistory();
        });
        
        closeGachaHistoryButton.onClick.AddListener(() =>
        {
            gachaHistoryPanel.gameObject.SetActive(false);
        });
        
        gachaHistoryNextPageButton.onClick.AddListener(() =>
        {
            if(currentPage >= maxGachaHistoryPage)
            {
                return;
            }
            currentPage++;
            ShowGachaHistory();
        });
        
        gachaHistoryPrePageButton.onClick.AddListener(() =>
        {
            if(currentPage <= 0)
            {
                return;
            }
            currentPage--;
            ShowGachaHistory();
        });
    }

    private int currentPage = 0;
    private void ShowGachaHistory()
    {
        gachaHistoryPanel.gameObject.SetActive(true);
        gachaHistoryPageText.text = (currentPage + 1).ToString();
        //加载抽卡记录
        xmlDoc.Load(gachaSaveXmlPath);
        List<string> names = new List<string>();
        List<string> stars = new List<string>();
        List<string> timeStrs = new List<string>();
        
        // 获取根节点
        XmlElement root = xmlDoc.DocumentElement;
        XmlNode historyNode = root.SelectSingleNode("GachaHistoryInfo");
        if (historyNode != null)
        {
            XmlNodeList levelsNode = historyNode.SelectNodes("GachaResultNode");
            //Sort by time
            if (levelsNode.Count != 0)
            {
                maxGachaHistoryPage = levelsNode.Count / 10;
                //取currentPage页的数据，一页显示10个
                for (int i = currentPage * 10; i < levelsNode.Count && i < (currentPage + 1) * 10; i++)
                {
                    names.Add(levelsNode[i].Attributes["name"].Value);
                    stars.Add(levelsNode[i].Attributes["star"].Value);
                    timeStrs.Add(levelsNode[i].Attributes["timeStr"].Value);
                }
            }
        }
        //ShowGachaHistoryInUI
        ShowGachaHistoryInUI(names, stars, timeStrs);
    }

    private void ShowGachaHistoryInUI(List<string> names, List<string> stars, List<string> timeStrs)
    {
        for(int i=1;i<gachaContentTrans.childCount;i++)
        {
            if(i > names.Count)  //没有数据了
            {
                break;
            }
            gachaContentTrans.GetChild(i).Find("ItemType").gameObject.GetComponent<TMP_Text>().text = stars[i - 1];
            if (stars[i - 1] == "4星")
            {
                gachaContentTrans.GetChild(i).Find("ItemName").gameObject.GetComponent<TMP_Text>().color = new Color(1, 0.251f, 0.847f, 0.4196f);
            }
            else if (stars[i - 1] == "5星")
            {
                gachaContentTrans.GetChild(i).Find("ItemName").gameObject.GetComponent<TMP_Text>().color = new Color(1,0.741f,0.251f,0.1196f);
            }
            else
            {
                gachaContentTrans.GetChild(i).Find("ItemName").gameObject.GetComponent<TMP_Text>().color = Color.black;
            }
            gachaContentTrans.GetChild(i).Find("ItemName").gameObject.GetComponent<TMP_Text>().text = names[i - 1];
            gachaContentTrans.GetChild(i).Find("GachaTime").gameObject.GetComponent<TMP_Text>().text = timeStrs[i - 1];
        }
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

    private void LoadGachaXmlFileBaseInfo()
    {
        xmlDoc.Load(gachaSaveXmlPath);
        // 获取根节点
        XmlElement root = xmlDoc.DocumentElement;
        // 解析所有的保底情况
        XmlNode baseInfoNode = root.SelectSingleNode("GachaBaseInfo");
        if (baseInfoNode != null)
        {
            XmlNodeList levelsNode = baseInfoNode.SelectNodes("GachaBaseInfoNode");
            if (levelsNode.Count != 0 && levelsNode[0]!=null) //还没有抽卡记录，就创建一个新的节点
            {
                drawCounter5Star = int.Parse(levelsNode[0].Attributes["drawCounter5Star"].Value);
                drawCounter4Star = int.Parse(levelsNode[0].Attributes["drawCounter4Star"].Value);
                isLast5StarCharacterUp = bool.Parse(levelsNode[0].Attributes["isLast5StarCharacterUp"].Value);
                isLast4StarUp = bool.Parse(levelsNode[0].Attributes["isLast4StarUp"].Value);
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
        LoadGachaXmlFileBaseInfo();
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

    private void SaveBaseDataToXmlFile()
    {
        //把已经抽了的抽数保存到Xml文件当中
        XmlElement root = xmlDoc.DocumentElement;
        // 创建新的character节点
        XmlNode historyNode = root.SelectSingleNode("GachaBaseInfo");
        if (historyNode != null)
        {
            XmlNodeList levelsNode = historyNode.SelectNodes("GachaBaseInfoNode");
            if (levelsNode.Count == 0) //还没有抽卡记录，就创建一个新的节点
            {
                XmlElement newGachaBaseNode = xmlDoc.CreateElement("GachaBaseInfoNode");
                newGachaBaseNode.SetAttribute("drawCounter5Star", drawCounter5Star.ToString());
                newGachaBaseNode.SetAttribute("drawCounter4Star", drawCounter4Star.ToString());
                newGachaBaseNode.SetAttribute("isLast5StarCharacterUp", isLast5StarCharacterUp.ToString());
                newGachaBaseNode.SetAttribute("isLast4StarUp", isLast4StarUp.ToString());
        
                // 将新创建的节点添加到levelsNode节点中
                if (historyNode!=null)
                {
                    historyNode.AppendChild(newGachaBaseNode);
                }
            }
            else
            {
                //对这些数值进行更新
                foreach (XmlElement xe in levelsNode) 
                {
                    xe.SetAttribute("drawCounter5Star", drawCounter5Star.ToString());
                    xe.SetAttribute("drawCounter4Star", drawCounter4Star.ToString());
                    xe.SetAttribute("isLast5StarCharacterUp", isLast5StarCharacterUp.ToString());
                    xe.SetAttribute("isLast4StarUp", isLast4StarUp.ToString());
                }
            }
        }
        xmlDoc.Save(gachaSaveXmlPath);
    }

    private void DebugGachaResultAndSave(List<string> result)
    {
        SaveBaseDataToXmlFile();
        //输出抽卡结果
        Debug.Log("====================================");
        XmlElement root = xmlDoc.DocumentElement;
        foreach (var res in result)
        {
            string name = SD_RogueGachaThingCSVFile.Class_Dic[res].Describe;
            int star = SD_RogueGachaThingCSVFile.Class_Dic[res]._RogueGachaItemStar();
            string type = SD_RogueGachaThingCSVFile.Class_Dic[res].RogueGachaItemType;
            string refId = SD_RogueGachaThingCSVFile.Class_Dic[res].RogueGachaItemFindID;
            // if (star == 5)
            // {
            //     Debug.Log("yes!!");
            // }
            // Debug.Log(name + "  星级: " + star);
            // Debug.Log("===============" + drawCounter5Star);
            //1.保存抽卡记录，写入一个文件当中，后面点击历史记录的时候会读取这个文件
            DateTime now = DateTime.Now; // 获取当前时间
            string timeStr = now.ToString("yyyy-MM-dd HH:mm:ss"); // 转为"年-月-日 时:分:秒"的格式
            WriteGachaResultToXMLFile(root, res, name, star, timeStr);
            //2,实时更新抽到的东西
            UpdateInfoToSave(type, refId);
        }
        Debug.Log("====================================");
        xmlDoc.Save(gachaSaveXmlPath);
    }

    private void WriteGachaResultToXMLFile(XmlElement root, string id, string name, int star,string timeStr)
    {
        // 创建新的character节点
        XmlNode historyNode = root.SelectSingleNode("GachaHistoryInfo");
        XmlElement newGachaResNode = xmlDoc.CreateElement("GachaResultNode");
        // 设置新的character节点的属性
        newGachaResNode.SetAttribute("id", id);
        newGachaResNode.SetAttribute("name", name);
        newGachaResNode.SetAttribute("star", star.ToString() + "星");
        newGachaResNode.SetAttribute("timeStr", timeStr);
        // 将新创建的节点添加到levelsNode节点中
        if (historyNode!=null)
        {
            //historyNode.AppendChild(newGachaResNode);
            //写入到最上面
            historyNode.InsertBefore(newGachaResNode, historyNode.FirstChild);
        }
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
