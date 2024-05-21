using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
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
    # endregion
    
    private void AddUIListeners()
    {
        getOneCardButton.onClick.AddListener(() =>
        {
            DrawCard(1);
        });
        getTenCardButton.onClick.AddListener(() =>
        {
            DrawCard(10);
        });
    }
    
    private void ReadGachaBaseInfo()
    {
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
    
    private void Start()
    {
        ReadGachaBaseInfo();
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
                if (drawChance <= 0.065f || !isLast4StarUp) //抽到UP角色
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

    private void ShowGachaResultInPanel(List<string> result)
    {
        skipGachaAnimationButton.onClick.RemoveAllListeners();
        StopAllCoroutines();
        //输出抽卡结果
        Debug.Log("====================================");
        foreach (var res in result)
        {
            string name = SD_RogueGachaThingCSVFile.Class_Dic[res].Describe;
            int star = SD_RogueGachaThingCSVFile.Class_Dic[res]._RogueGachaItemStar();
            if (star == 5)
            {
                Debug.Log("yes!!");
            }
            Debug.Log(name + "  星级: " + star);
            Debug.Log("===============");
        }

        Debug.Log("====================================");
    }

}
