using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
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
    
    //表情
    public List<string> expressionNameList = new List<string>();
    public List<string> expressionUINameList = new List<string>();
    public List<List<int>> characterExpressionIndices = new List<List<int>>();


    private List<string> animationNoMoveList = new List<string>();
    List<string> animationNoMoveListInUI = new List<string>();
    
    
    List<string> animationMoveList = new List<string>();
    List<string> animationMoveListInUI =new List<string>();
    
    //地点
    public List<GameObject> DestinationGoList = new List<GameObject>();
    public List<string> DestinationList = new List<string>();
    public List<string> DestinationUINameList = new List<string>();

    //特效
    //读取后处理相关的CSV文件
    public List<string> postEffectNames = new List<string>();
    public List<string> postEffectUIName = new List<string>();
    
    public List<string> postEffectFieldNames = new List<string>();
    public List<List<string>> postEffectAttributeNames = new List<List<string>>();
    public List<List<float>> postEffectAttributeValues = new List<List<float>>();
    public List<List<float>> postEffectDefaultValues = new List<List<float>>();
    public List<string> postEffectTypes = new List<string>();
    public List<List<int>> postEffectShouldLerp = new List<List<int>>();
    //存储特效的结构体
    public struct PostEffectStruct
    {
        public string effectName;
        public string effectUIName;
        public string effectFieldName;
        public List<string> attributeNames;
        public List<float> attributeValues;
        public List<float> defaultValues;
        public string type;
        public List<int> shouldLerp;
    }
    
    //相机
    public List<string> cameraNamesList = new List<string>();
    public List<string> cameraUINameList = new List<string>();
    
    //一个相机结构体或者类？
    public struct CameraStruct
    {
        public string cameraName;
        public string cameraUIName;
        //是否是follow
        public bool isFollow;
        //是否是lookat
        public bool isLookAt;
    }
    //存储相机的结构体
    public List<CameraStruct> cameraStructs = new List<CameraStruct>();
    
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
        //ReadDestinationCSV();
        ReadPostProcessingCSV();
        ReadAnimationCSV("Assets/Designer/CsvTable/AnimationCSVFile.csv",
            animationNoMoveList,animationNoMoveListInUI,animationMoveList,animationMoveListInUI);
        ReadDestinationCSV("Assets/Designer/CsvTable/DestinationCSVFile.csv",
            DestinationList,DestinationUINameList);
        preKeepDestination();
        ReadCameraCSV("Assets/Designer/CsvTable/CameraCSVFile.csv",
            cameraNamesList,cameraUINameList,cameraStructs);
        ReadOpenWorldThings();
        effs =new List<ScriptableRendererFeature>();
    }
    
    //定义一个eff 用于存放特效 其中有每个特效的名称和id
    private void Start()
    {
        preLoadEffectFullScreen();
        SetAllEffRendererFeatureOff();

        GetAllBlendShapeUsed();
    }
    
    //不同id更改的动画轨道index
    public Dictionary<int,int> animationId2TimelineIndex = new Dictionary<int, int>()
    {
        {1,1},
        {8,0},
    };

    void ReadOpenWorldThings()
    {
        string worldItemLink = "Assets/Designer/CsvTable/ItemSystem/WorldItemCSVFile.csv";
        ReadItemsInDesignerTable(worldItemLink);
        //todo:大世界暂时就一个宝箱，后面再研究读取其他东西
        string treasureLayout = "Assets/Designer/CsvTable/ItemSystem/WorldTreasureLayoutCSVFile.csv";
        string treasure = "Assets/Designer/CsvTable/ItemSystem/WorldTreasureCSVFile.csv";
        HOpenWorldTreasureManager.Instance.ReadCSVFile(treasureLayout, treasure);
    }

    public Dictionary<string, HOpenWorldItemStruct> worldItems = new Dictionary<string, HOpenWorldItemStruct>();
    void ReadItemsInDesignerTable(string link)
    {
        //从index=3开始读取
        string[] fileData = File.ReadAllLines(link);
        for (int i = 3; i < fileData.Length; i++)
        {
            string[] rowData = fileData[i].Split(',');
            string itemName = rowData[0];
            HOpenWorldItemStruct item = new HOpenWorldItemStruct();
            item.id = int.Parse(rowData[0]);
            item.itemName = rowData[3];
            item.chineseName = rowData[2];
            item.itemType = (ItemType)Enum.Parse(typeof(ItemType), rowData[1]);
            worldItems.Add(itemName, item);
        }
    }
    
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
                    string selectName = values[1];
                    string dropdownName = values[2];
                    string resourcesName = values[3];
                    string UIName = values[4];
                    int sequenceInSelfClass = int.Parse(values[5]);

                    // 将解析后的数据存入您的数据结构中
                    this.selectId.Add(selectName, 0);
                    this.dropdownNames.Add(dropdownName);
                    this.selectNames.Add(selectName);
                    this.SelectTable.Add(new List<string>(resourcesName.Split(';')));
                    this.UISelectTable.Add(new List<string>(UIName.Split(';')));
                    this.selectNames2Id.Add(selectName, id);
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
            string expressionUIName = rowData[1];
            expressionNameList.Add(expression);
            expressionUINameList.Add(expressionUIName);
            
            List<List<int>> expressionData = new List<List<int>>();

            for (int j = 2; j < rowData.Length; j++)
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
        // for (int i = 0; i < blendshapeIndexs.Count; i++)
        // {
        //     for (int j = 0; j < blendshapeIndexs[i].Count; j++)
        //     {
        //         for (int k = 0; k < blendshapeIndexs[i][j].Count; k++)
        //         {
        //             Debug.Log("blendshapeNames[" + i + "][" + j + "][" + k + "] = " + blendshapeIndexs[i][j][k]);
        //         }
        //     }
        // }
        UpdateTableList("blendshape",expressionNameList,expressionUINameList);
    }

    
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
            
            string effectUIName = rowData[1];
            
            string effectFieldName = rowData[2];
            string attributeNames = rowData[3];
            string attributeValues = rowData[4];
            string shouldLerp = rowData[5];
            string defaultValues = rowData[6];
            string type = rowData[7];
            
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
            postEffectUIName.Add(effectUIName);
            
            postEffectAttributeNames.Add(new List<string>(attributeNames.Split(';')));
            postEffectTypes.Add(type);
            postEffectAttributeValues.Add(floatAttributeValues);
            postEffectDefaultValues.Add(floatDefaultValues);
            postEffectShouldLerp.Add(intShouldLerp);
        }
        UpdateTableList("effect",postEffectNames,postEffectUIName);
    }

    /// <summary>
    /// 现在弃用了 现在采用直接场景中的物体来代替
    /// </summary>
    void ReadDestinationCSV()
    {
        string filePath = "Assets/Designer/CsvTable/DestinationCSVFileOld.csv"; 

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

    void ReadDestinationCSV(string filePath,
        List<string> destinationList, List<string> destinationListInUI
        )
    {
        using (var reader = new StreamReader(filePath))
        {
            bool header = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (header) // 跳过表头
                {
                    header = false;
                    continue;
                }
                var values = line.Split(',');
                string destinationName = values[0];
                string uiName = values[1];

                destinationList.Add(destinationName);
                destinationListInUI.Add(uiName);
            }
        }
        //UpdateDestinationList();
        UpdateTableList("destination",destinationList,destinationListInUI);
    }
    public void ReadAnimationCSV(string filePath, 
        List<string> noMoveList, List<string> noMoveListInUI, 
        List<string> moveList, List<string> moveListInUI)
    {
        using (var reader = new StreamReader(filePath))
        {
            bool header = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (header) // 跳过表头
                {
                    header = false;
                    continue;
                }
                var values = line.Split(',');
                string animationName = values[0];
                string moveType = values[1];
                string uiName = values[2];

                if (moveType == "NoMove")
                {
                    noMoveList.Add(animationName);
                    noMoveListInUI.Add(uiName);
                }
                else if (moveType == "Move")
                {
                    moveList.Add(animationName);
                    moveListInUI.Add(uiName);
                }
            }
        }
    }
    
    void ReadCameraCSV(string filePath, 
        List<string> cameraList, List<string> cameraListInUI,List<CameraStruct> cameraStructs)

    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            bool header = true;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (header) // 跳过表头
                {
                    header = false;
                    continue;
                }
                string[] values = line.Split(',');

                if (values.Length == 4) // 确保每行有三个值
                {
                    CameraStruct newCamera = new CameraStruct();
                    string cameraName = values[0];
                    string cameraUIName = values[1];
                    newCamera.cameraName = cameraName;
                    newCamera.cameraUIName = cameraUIName;
                    // newCamera.cameraName = values[0];
                    // newCamera.cameraUIName = values[1];
                    newCamera.isFollow = values[2] == "1";//如果是1就是true,如果是0就是false
                    newCamera.isLookAt = values[3] == "1";
                    cameraStructs.Add(newCamera);
                    
                    cameraList.Add(cameraName);
                    cameraListInUI.Add(cameraUIName);
                }
            }
        }
        //UpdateCameraList();
        UpdateTableList("camera",cameraList,cameraListInUI);
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
    
   
    private void preKeepDestination()
    {
        //获取目的地
        //遍历destination[selectId["destination1"]]; 并存储于一个list中
        //然后在timeline中的每个destination中的位置都是这个list中的位置
        
        // for (int i = 0; i < SelectTable[selectNames2Id["destination1"]].Count; i++)
        // {
        //     int id = selectNames2Id["destination1"];
        //     string destinationName = SelectTable[id][i];
        //     // Debug.Log("destinationName: " + destinationName);
        //     
        //     DestinationGoList.Add(GameObject.Find(destinationName));
        //     DestinationUINameList.Add(UISelectTable[id][i]);
        // }
        
        for (int i = 0; i < DestinationList.Count; i++)
        {
            DestinationGoList.Add(GameObject.Find(DestinationList[i]));
        }
        
        
    }
    public Transform GetDestination(int selectId)
    {
        //哪一个地点
        // string destinationName = SelectTable[selectNames2Id["destination1"]][selectId];
        // // Debug.Log("destinationName: " + destinationName);
        // return GameObject.Find(destinationName).transform;
        
        return DestinationGoList[selectId].transform;
    }

    public int GetCharacterNum()
    {
        int characterNum = SelectTable[selectNames2Id["character"]].Count;
        return characterNum;
    }
    //写一个函数 遍历所有角色，并调用GetAllBlendShapeUsedInExpression
    public void GetAllBlendShapeUsed()
    {
        int characterNum = GetCharacterNum();
        for (int i = 0; i < characterNum; i++)
        {
            GetAllBlendShapeUsedInExpression(i);
        }
    }
    
    public void GetAllBlendShapeUsedInExpression(int givenCharacter)
    {
        //获取blendshapeIndexs中的所有的blendshapeIndex 
        //给定角色，得到这个角色的而所有表情的index，得到他所有的表情的blendshape，作为一个序列，比如给qiyu，得到24,10,18等构成的list
        
        List<int> characterExpressionI = new List<int>();
        for (int i = 0; i < blendshapeIndexs.Count; i++)
        {
            for (int k = 0; k < blendshapeIndexs[i][givenCharacter].Count; k++)
            {
                characterExpressionI.Add(blendshapeIndexs[i][givenCharacter][k]);
            }
            
        }
        characterExpressionIndices.Add(characterExpressionI);
        
    }
    
    
    List<string> animationChooseList = new List<string>();
    public List<string> getAnimationChooseList()
    {
        //获取动画选择列表 
        for(int i = 1; i <= 5; i++)
        {
            //选择了SelectTable中id为多少的那个动画选项框 是第一个动画选项框还是第二个第三个，这里的chooseid是2，3，4（因为前面有个角色选项框）
            int chooseid = selectNames2Id["animation" + i];
            int selectID = selectId["animation"+i];
            string AnimStr = SelectTable[chooseid][selectID];
            animationChooseList.Add(AnimStr);
        }
        return animationChooseList;
        
    }
    
   
    public List<bool> isMoveList = new List<bool>();
    public void UpdateMoveOrNoMoveAnimationList(int index,int indexInSequence,bool isMove)
    {
        if (isMove)
        {
            isMoveList.Add(true);
            UpdateSelectTable(index,animationMoveList);
            UpdateUISelectTable(index,animationMoveListInUI);
        }
        else
        {
            isMoveList.Add(false);
            UpdateSelectTable(index,animationNoMoveList);
            UpdateUISelectTable(index,animationNoMoveListInUI);
            
            //错误 因为他可能新增节点后又去改前面的节点，所以这个是不对的
            //UpdateSelectId("destination"+indexInSequence,selectId["destination"+(indexInSequence-1)]);
            
            //不移动的话可能还得改当前节点的位置为上一个节点的位置
            // if (indexInSequence != 1)
            // {
            //     UpdateSelectId("destination"+indexInSequence,selectId["destination"+(indexInSequence-1)]);
            // }
            // else
            // {
            //     //此时这个是有问题的！！！因为这个应该检索起点的索引！！，不应该是0
            //     UpdateSelectId("destination"+indexInSequence,0);
            // }
        }
    }
    // void UpdateDestinationList()
    // {
    //     for(int i = 1; i <= 5; i++)
    //     {
    //         //选择了SelectTable中id为多少的那个动画选项框 是第一个动画选项框还是第二个第三个，这里的chooseid是2，3，4（因为前面有个角色选项框）
    //         int chooseid = selectNames2Id["destination" + i];
    //         UpdateSelectTable(chooseid,DestinationList);
    //         UpdateUISelectTable(chooseid,DestinationUINameList);
    //     }
    // }
    //
    // void UpdateCameraList()
    // {
    //     for(int i = 1; i <= 5; i++)
    //     {
    //         int chooseid = selectNames2Id["camera" + i];
    //         UpdateSelectTable(chooseid,cameraNamesList);
    //         UpdateUISelectTable(chooseid,cameraUINameList);
    //     }
    // }

    void UpdateTableList(string name, List<string> nameList, List<string> nameListInUI)
    {
        for(int i = 1; i <= 5; i++)
        {
            int chooseid = selectNames2Id[name + i];
            UpdateSelectTable(chooseid,nameList);
            UpdateUISelectTable(chooseid,nameListInUI);
        }
    }
   
    //将selecttable中的某一行的数据转换成一个list
    void UpdateSelectTable(int index, List<string> list)
    {
        SelectTable[index] = list;
    }
    
    void UpdateUISelectTable(int index, List<string> list)
    {
        UISelectTable[index] = list;
    }
    void UpdateSelectId(string name,int value)
    {
        selectId[name] = value;
    }
    
    //开始运行timeline前 但是点击了确定之后这个脚本要做的
    public void BeforePlayTimeline()
    {
        Debug.Log("PlayTimeMove!!!!!!!!!!!!!!!");
        //循环遍历move 列表，把所有没有move的 不移动的话可能还得改当前节点的位置为上一个节点的位置
        for (int i = 0; i < isMoveList.Count; i++)
        {
            if (!isMoveList[i])
            {
                if (i != 0)
                {
                    UpdateSelectId("destination"+(i+1),selectId["destination"+i]);
                }
                else
                {
                    UpdateSelectId("destination"+(i+1),0);
                }
            }
        }
    }


    
    
}