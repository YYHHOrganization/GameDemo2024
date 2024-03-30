using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Serialization;
using TMPro;

public class YLevelMain : MonoBehaviour 
{
    //单例
    public static YLevelMain Instance;
    void Awake()
    {
        Instance = this;
    }
    //关卡列表
    private List<YLevel> m_levels;

    void Start () 
    {
        
        //人为解锁第二个关卡
        //在实际游戏中玩家需要满足一定条件方可解锁关卡
        //此处仅作为演示
        //YLevelManager.SetLevels ("level1", true);

    }
    //动态生成关卡
    public void InitLevelPanel(GameObject parent,YLevelPanel panel)
    {
        //获取关卡
        m_levels = YLevelManager.LoadLevels(panel);
        //动态生成关卡
        foreach (YLevel l in m_levels) 
        {
            GameObject prefab=(GameObject)Instantiate((Resources.Load("Prefabs/UI/singleUnit/YLevelPanelUnit") as GameObject));
            //数据绑定
            DataBind(prefab,l);
            //设置父物体
            //prefab.transform.SetParent(GameObject.Find("UIRoot/Background/LevelPanel").transform);
            prefab.transform.SetParent(parent.transform);
            prefab.transform.localPosition=new Vector3(0,0,0);
            prefab.transform.localScale=new Vector3(1,1,1);
            //将关卡信息传给关卡
            YLevelEvent yLevelEvent=prefab.GetComponent<YLevelEvent>();
            yLevelEvent.level=l;
            //AddListener
            prefab.GetComponent<Button>().onClick.AddListener(yLevelEvent.OnClick);
            prefab.name="Level";
        }

    }

    /// <summary>
    /// 数据绑定
    /// </summary>
    void DataBind(GameObject go,YLevel level)
    {
        //为关卡绑定关卡名称
        //go.transform.Find("LevelName").GetComponent<Text>().text=level.Name;
        
        //为关卡绑定关卡图片
        Texture2D tex2D;
        if(level.UnLock)
        {
            go.GetComponentInChildren<TMP_Text>().text = level.UIName;
            tex2D=Resources.Load("Prefabs/UI/singleUnit/YLevel/nolocked") as Texture2D;
        }
        else
        {
            tex2D=Resources.Load("Prefabs/UI/singleUnit/YLevel/LockedLevelUI") as Texture2D;
        }
        Sprite sprite=Sprite.Create(tex2D,new Rect(0,0,tex2D.width,tex2D.height),new Vector2(0.5F,0.5F));
        go.transform.GetComponent<Image>().sprite=sprite;
    }
    
    
    //test 测试按下v，直接进入输了界面 测试！！
    void Update()
    {
        
    }
    
    
}