using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

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
    //表示每个选项框对应【选择的选项的id】
    public Dictionary<string,int> selectId=new Dictionary<string, int>();
    public List<string> dropdownNames = new List<string>();
    public List<string> selectNames = new List<string>();
    //再加一个用来存储每个string类型，对应的int类型的id
    public Dictionary<string,int> selectNames2Id=new Dictionary<string, int>();
    //最后一列表示在自己的细分类中的序列号，比如是第几个animation
    public List<int> selectSequenceInSelfClass=new List<int>();
    
    //每一行表示一个表情以及它们在不同角色中对应的每个blendshape的id 存储起来
    //blendshapeNames[0][0]表示第一个表情对应的第一个角色的blendshape的ids
    public List<List<List<int>>> blendshapeIndexs=new List<List<List<int>>>();
    
    //存储目的地 每个目的地是一个vector3
    public List<Vector3> destination=new List<Vector3>();
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
        ReadScreenPlayCSV();
        ReadExpressionCSV();
        ReadDestinationCSV();
        ReadPostProcessingCSV();
        effs =new List<ScriptableRendererFeature>();
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
    
    void ReadScreenPlayCSV()
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
                    int sequenceInSelfClass = int.Parse(values[5]);

                    // 将解析后的数据存入您的数据结构中
                    this.selectId.Add(selectId, 0);
                    this.dropdownNames.Add(dropdownName);
                    this.selectNames.Add(selectId);
                    this.SelectTable.Add(new List<string>(resourcesName.Split(';')));
                    this.UISelectTable.Add(new List<string>(UIName.Split(';')));
                    this.selectNames2Id.Add(selectId, id);
                    this.selectSequenceInSelfClass.Add(sequenceInSelfClass);
                }
            }

            // 在这里可以将数据结构中的数据用于您的逻辑
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
    
    //读取| expression | qiyu  | xina |
    // | ---------- | ----- | ---- |
    // | happy      | 24    | 5;23 |
    // | amazed     | 10;18 | 24   | 存在blendshapeNames中
    void ReadExpressionCSV()
    {
        string filePath = "Assets/Designer/CsvTable/ExpressionCSVFile.csv"; // 替换成您的CSV文件路径

        // 读取CSV文件内容
        string[] fileData = System.IO.File.ReadAllLines(filePath);

        // 解析CSV数据并存储到 blendshapeNames 中
        for (int i = 1; i < fileData.Length; i++) // Start from 1 to skip header row
        {
            string[] rowData = fileData[i].Split(',');
            string expression = rowData[0];
            List<List<int>> expressionData = new List<List<int>>();

            for (int j = 1; j < rowData.Length; j++)
            {
                string[] values = rowData[j].Split(';');
                List<int> intValues = new List<int>();
                
                foreach (string value in values)
                {
                    int intValue;
                    if (int.TryParse(value, out intValue))
                    {
                        intValues.Add(intValue);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse value: " + value);
                    }
                }

                expressionData.Add(intValues);
            }

            blendshapeIndexs.Add(expressionData);
        }
        //debug 测试输出blendshapeNames
        for (int i = 0; i < blendshapeIndexs.Count; i++)
        {
            for (int j = 0; j < blendshapeIndexs[i].Count; j++)
            {
                for (int k = 0; k < blendshapeIndexs[i][j].Count; k++)
                {
                    Debug.Log("blendshapeNames[" + i + "][" + j + "][" + k + "] = " + blendshapeIndexs[i][j][k]);
                }
            }
        }
    }

    //读取后处理相关的CSV文件
    public List<string> postEffectNames;
    public List<string> postEffectFieldNames;
    public List<List<string>> postEffectAttributeNames;
    public List<List<float>> postEffectAttributeValues;
    public List<List<float>> postEffectDefaultValues;
    public List<string> postEffectTypes;
    public List<List<int>> postEffectShouldLerp;
    void ReadPostProcessingCSV()
    {
        string filePath = "Assets/Designer/CsvTable/PostProcessingCSVFile.csv";
        // 读取CSV文件内容
        string[] fileData = System.IO.File.ReadAllLines(filePath);
        for (int i = 1; i < fileData.Length; i++)
        {
            string[] rowData = fileData[i].Split(',');
            //解析csv一行的数据
            string effectName = rowData[0];
            string effectFieldName = rowData[1];
            string attributeNames = rowData[2];
            string attributeValues = rowData[3];
            string shouldLerp = rowData[4];
            string defaultValues = rowData[5];
            string type = rowData[6];
            
            List<float> floatAttributeValues = new List<float>();
            foreach (string value in attributeValues.Split(';'))
            {
                float floatValue;
                if (float.TryParse(value, out floatValue))
                {
                    floatAttributeValues.Add(floatValue);
                }
                else
                {
                    Debug.LogError("Failed to parse value: " + value);
                }
            }
            
            List<float> floatDefaultValues = new List<float>();
            foreach (string value in defaultValues.Split(';'))
            {
                float floatValue;
                if (float.TryParse(value, out floatValue))
                {
                    floatDefaultValues.Add(floatValue);
                }
                else
                {
                    Debug.LogError("Failed to parse value: " + value);
                }
            }
            
            List<int> intShouldLerp = new List<int>();
            foreach (string value in shouldLerp.Split(';'))
            {
                int intValue;
                if (int.TryParse(value, out intValue))
                {
                    intShouldLerp.Add(intValue);
                }
                else
                {
                    Debug.LogError("Failed to parse value: " + value);
                }
            }
            
            postEffectNames.Add(effectName);
            postEffectFieldNames.Add(effectFieldName);
            postEffectAttributeNames.Add(new List<string>(attributeNames.Split(';')));
            postEffectTypes.Add(type);
            postEffectAttributeValues.Add(floatAttributeValues);
            postEffectDefaultValues.Add(floatDefaultValues);
            postEffectShouldLerp.Add(intShouldLerp);
        }
    }

    void ReadDestinationCSV()
    {
        string filePath = "Assets/Designer/CsvTable/DestinationCSVFile.csv"; 

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                if (values.Length == 4) // 确保每行有四个值
                {
                    float x, y, z;
                    if (float.TryParse(values[1], out x) && float.TryParse(values[2], out y) && float.TryParse(values[3], out z))
                    {
                        Vector3 newDestination = new Vector3(x, y, z);
                        destination.Add(newDestination);
                    }
                }
            }
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
    //GetDestination(characterId, selectId);
    public Transform GetDestination(int selectId)
    {
        // Debug.Log("selectIdWhy>>: " + selectId);
        // string path = "Prefabs/YCharacter/" + "xina";
        // GameObject destinationObject = GameObject.Instantiate(Resources.Load<GameObject>(path));
        // destinationObject.name = "Destination+" + selectId + Random.Range(0, 1554);
        // destinationObject.transform.position = destination[selectId];
        // return destinationObject.transform;
        
        //Vector3 destination = this.destination[selectId];
        
        if (selectId==1)
        {
            return GameObject.Find("Restaurant").transform;
        }
        else if (selectId==0)
        {
            return GameObject.Find("FruitsCornor").transform;
        }
        else if (selectId==2)
        {
            return GameObject.Find("Bench").transform;
        }
       return GameObject.Find("Restaurant").transform;
        
        // Transform destinationTransform = new GameObject().transform;
        // destinationTransform.position = destination[selectId];
        // return destinationTransform;
    }
}