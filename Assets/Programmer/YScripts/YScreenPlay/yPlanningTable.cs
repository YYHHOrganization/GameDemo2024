using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class yPlanningTable : MonoBehaviour
{
    //静态类
    public static yPlanningTable Instance{get;private set;}
    //获取Eff
    //public List<ScriptableRendererFeature> effs=new List<ScriptableRendererFeature>();  
    // public ScriptableRendererFeature[] effs ;
    public List<ScriptableRendererFeature> effs ;
    //角色list
    public List<string> characterName;

    //public List<string> effName;
    public List<List<string>> SelectTable=new List<List<string>>();
    //UI上的选项名称 
    public List<List<string>> UISelectTable=new List<List<string>>();
    public Dictionary<string,int> selectId=new Dictionary<string, int>();
    public List<string> dropdownNames = new List<string>();
    public List<string> selectNames = new List<string>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        // effs =new ScriptableRendererFeature[2]{null,null};
        ReadCSV();
        effs =new List<ScriptableRendererFeature>();
        
        // SelectTable = new List<List<string>>()
        // {
        //     //顺序是角色 动画 音效 特效 相机 起点 终点
        //     new List<string>() { "qiyu", "xina", },//0
        //     new List<string>(){ "Run","Swagger","Creep"},//小步快跑Run/大摇大摆/阴暗的爬行//1
        //     new List<string>(){ "audio1","audio2","audio3"},//2
        //     new List<string>(){ "FullScreenWrapPassRendererFeature", "RadialBlur", "null"},//3
        //     new List<string>(){ "Look down","Follow"},//4
        //     new List<string>(){ "origin1","origin2","origin3"},
        //     new List<string>(){ "destination1","destination2","destination3"},
        //     new List<string>(){ "2.5Years","3.5Years","3000Years"},//7
        //     new List<string>(){ "Drinking","LookAround",},//8
        // };
        //
        // //UI上的选项名称 
        // UISelectTable = new List<List<string>>()
        // {
        //     //顺序是角色 动画 音效 特效 相机 起点 终点
        //     new List<string>() { "小帅", "小红", },//0
        //     new List<string>(){ "小步快跑过去","大摇大摆走过去","阴暗的爬行过去"},//小步快跑Run/大摇大摆/阴暗的爬行//1
        //     new List<string>(){ "audio1","audio2","audio3"},//2
        //     new List<string>(){ "扭曲特效", "径向模糊特效", "无特效"},//3
        //     new List<string>(){ "俯视镜头","跟随镜头"},//4
        //     new List<string>(){ "origin1","origin2","origin3"},
        //     new List<string>(){ "destination1","destination2","destination3"},
        //     new List<string>(){ "2.5年历史","3.5年历史","3000年历史"},//7
        //     new List<string>(){ "吃毒蘑菇","东张西望",},//8
        // };
        //
        //  selectId = new Dictionary<string, int>() 
        //  {
        //     {"character",0},
        //     {"animation",0},
        //     {"audio",0},
        //     //加上特效和相机
        //     {"effect",0},
        //     {"camera",0},
        //     {"origin",0},
        //     {"destination",0},
        //     //宝藏年份
        //     {"treasure",0},
        //     {"animation2",0},
        //
        // };
        // //改为可扩展的List
        // dropdownNames = new List<string>(){"DropdownCha","DropdownAnimation",
        //     "DropdownAudio","DropdownEffect","DropdownCamera","DropdownOrigin","DropdownDestination","DropdownTreasure","DropdownAnimationFront"};
        //selectNames = new List<string>(){"character","animation","audio","effect","camera","origin","destination","treasure","animation2"};
    }
    
    
    
    //定义一个eff 用于存放特效 其中有每个特效的名称和id
    private void Start()
    {
        preLoadEffectFullScreen();
        SetAllEffRendererFeatureOff();
    }
    
    //不同id更改的动画轨道index
    public Dictionary<int,int> animationId2TimelineIndex = new Dictionary<int, int>()
    {
        {1,1},
        {8,0},
    };
    
    void ReadCSV()
    {
        //读取csv文件
        //string path = Application.dataPath + "/Resources/SelectTable.csv";
        //string path = "Assets/Resources/SelectTable.csv";
        //StreamReader sr = new StreamReader(path);
        //string str = sr.ReadToEnd();
        //Debug.Log(str);
        //sr.Close();
        
        string filePath = "Assets/Designer/CsvTable/ScreenPlayCSVFile.csv"; // 替换成您的CSV文件路径

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    // 解析CSV行，并将数据存入相应的数据结构中
                    int id = int.Parse(values[0]);
                    string selectId = values[1];
                    string dropdownName = values[2];
                    string resourcesName = values[3];
                    string UIName = values[4];

                    // 将解析后的数据存入您的数据结构中
                    this.selectId.Add(selectId, 0);
                    this.dropdownNames.Add(dropdownName);
                    this.selectNames.Add(selectId);
                    this.SelectTable.Add(new List<string>(resourcesName.Split(';')));
                    this.UISelectTable.Add(new List<string>(UIName.Split(';')));
                }
            }

            // 在这里可以将数据结构中的数据用于您的逻辑
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
    public ScriptableRendererFeature GetEffRendererFeature(int index)
    {
        if(SelectTable[3][index]=="null")
        {
            return null;
        }
        return effs[index];
    }
    //把其他的特效停下setActive(false) 
    public void SetAllEffRendererFeatureOff()
    {
        Debug.Log("InSetAll : "+ effs.Count);
        for (int i = 0; i < effs.Count; i++)
        {
            if (effs[i] == null)
            {
                continue;
            }
            effs[i].SetActive(false);
        }
    }
    //预加载特效资源
    public void preLoadEffectFullScreen()
    {
        ////加载资源 ScriptableRendererFeature
        //加载特效资源
        List<ScriptableRendererFeature> srfList;
        ScriptableRendererFeature unitRendererFeature=null;
        UniversalRenderPipelineAsset _pipelineAssetCurrent = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;  // 通过GraphicsSettings获取当前的配置
        _pipelineAssetCurrent = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;  // 通过QualitySettings获取当前的配置
        //_pipelineAssetCurrent = QualitySettings.GetRenderPipelineAssetAt(QualitySettings.GetQualityLevel()) as UniversalRenderPipelineAsset;  // 通过QualitySettings获取不同等级的配置

        // 也可以通过QualitySettings.names遍历所有配置

        srfList = _pipelineAssetCurrent.scriptableRenderer.GetType().
                GetProperty("rendererFeatures",
                    BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(_pipelineAssetCurrent.scriptableRenderer, null)
            as List<ScriptableRendererFeature>;

        //此处我们取寻找List中所有的特效是否有我们需要的特效 SelectTable[3]中除了==null之外的所有 是否存在于srfList中  
        //如果存在 我们就把他们赋值给effs 并且这个特效在effs中的索引就是他在SelectTable[3]中的索引
        // for(int i = 0; i < srfList.Count; i++)
        // {
        //     if (srfList[i].name == SelectTable[3][0])
        //     {
        //         effs[0] = srfList[i];
        //         // unitRendererFeature.SetActive(false);
        //     }
        //     if (srfList[i].name == SelectTable[3][1])
        //     {
        //         effs[1] = srfList[i];
        //         // unitRendererFeature.SetActive(false);
        //     }
        // }
        
        // 创建一个字典用于存储特效名称和对应的特效对象
        Dictionary<string, ScriptableRendererFeature> effsDict = new Dictionary<string, ScriptableRendererFeature>();

        foreach (ScriptableRendererFeature srf in srfList)
        {
            if (!string.IsNullOrEmpty(srf.name) && SelectTable[3].Contains(srf.name))
            {
                effsDict[srf.name] = srf;
            }
        }

        // 将特效对象按照在SelectTable[3]中的顺序存入effs列表
        for (int i = 0; i < SelectTable[3].Count; i++)
        {
            if (!string.IsNullOrEmpty(SelectTable[3][i]) && effsDict.ContainsKey(SelectTable[3][i]))
            {
                effs.Add(effsDict[SelectTable[3][i]]);
            }
            else
            {
                effs.Add(null);
            }
        }
        
        
    }
}