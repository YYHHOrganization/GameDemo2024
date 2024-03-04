using System;
using System.Collections;
using System.Collections.Generic;
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
    public ScriptableRendererFeature[] effs ;
    //角色list
    public List<string> characterName;

    public List<string> effName;
    public List<List<string>> SelectTable;
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
        effs =new ScriptableRendererFeature[2]{null,null};

        effName = new List<string>()
        {
            "FullScreenWrapPassRendererFeature",
            "RadialBlur",
        };
        
        SelectTable = new List<List<string>>()
        {
            //顺序是角色 动画 音效 特效 相机 起点 终点
           
            new List<string>() { "qiyu", "xina", },//0
            new List<string>(){ "Run","Swagger","Creep"},//小步快跑Run/大摇大摆/阴暗的爬行//1
            new List<string>(){ "audio1","audio2","audio3"},//2
            new List<string>(){ "FullScreenWrapPassRendererFeature", "RadialBlur", "null"},//3
            new List<string>(){ "Look down","Follow"},//4
            new List<string>(){ "origin1","origin2","origin3"},
            new List<string>(){ "destination1","destination2","destination3"},
            // new List<string>(){ "treasure2.5Years","treasure3.5Years","treasure3000Years"},//7
            new List<string>(){ "2.5Years","3.5Years","3000Years"},//7
        };
    }
    
    //定义一个eff 用于存放特效 其中有每个特效的名称和id
    private void Start()
    {
        preLoadEffectFullScreen();
        SetAllEffRendererFeatureOff();
    }
    
    
    public string GetEff(int index)
    {
        return effName[index];
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
        Debug.Log("InSetAll : "+ effs.Length);
        for (int i = 0; i < effs.Length; i++)
        {
            effs[i].SetActive(false);
            // if (i != index)
            // {
            //     effs[i].SetActive(false);
            // }
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

        for(int i = 0; i < srfList.Count; i++)
        {
            if (srfList[i].name == effName[0])
            {
                effs[0] = srfList[i];
                // unitRendererFeature.SetActive(false);
            }
            if (srfList[i].name == effName[1])
            {
                effs[1] = srfList[i];
                // unitRendererFeature.SetActive(false);
            }
        }
    }
}