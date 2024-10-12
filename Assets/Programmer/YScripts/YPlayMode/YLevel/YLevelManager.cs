using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
public static class YLevelManager
{
    static int currentLevelIndex = 0;
    static List<string> levelnames;
    static YLevelPanel mpanel;
    /// <summary> 
    /// 加载Xml文件 
    /// </summary>
    /// <returns>The levels.</returns>
    public static List<YLevel> LoadLevels(YLevelPanel panel)
    {
        mpanel = panel;
        levelnames = new List<string>();
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        //如果本地存在配置文件则读取配置文件
        //否则在本地创建配置文件的副本
        //为了跨平台及可读可写，需要使用Application.persistentDataPath
        //string filePath = Application.persistentDataPath + "/YDesigner/XMLTable/levels.xml";
        string filePath = Application.dataPath + "/Designer/XMLTable/levels.xml";
        
        // if (!IOUntility.isFileExists (filePath)) {
        //     xmlDoc.LoadXml (((TextAsset)Resources.Load ("levels")).text);
        //     IOUntility.CreateFile (filePath, xmlDoc.InnerXml);
        // } else {
        //     xmlDoc.Load(filePath);
        // }
        xmlDoc.Load(filePath);
        
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList levelsNode = root.SelectNodes("/levels/level");
        //初始化关卡列表
        // List<DocumentationSortingAttribute.Level> levels = new List<DocumentationSortingAttribute.Level>();
        List<YLevel> levels = new List<YLevel>();
        foreach (XmlElement xe in levelsNode) 
        {
            // DocumentationSortingAttribute.Level l=new DocumentationSortingAttribute.Level();
            YLevel l = new YLevel();
            l.ID=xe.GetAttribute("id");
            l.Name=xe.GetAttribute("name");
            l.UIName=xe.GetAttribute("uiname");
            //使用unlock属性来标识当前关卡是否解锁
            if(xe.GetAttribute("unlock")=="1"){
                l.UnLock=true;
            }else{
                l.UnLock=false;
            }

            levels.Add(l);
            
            levelnames.Add(l.Name);
        }

        return levels;
    }

    /// <summary>
    /// 设置某一关卡的状态
    /// </summary>
    /// <param name="name">关卡名称</param>
    /// <param name="locked">是否解锁</param>
    public static void SetLevels(string name,bool unlock)
    {
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        string filePath = Application.dataPath + "/Designer/XMLTable/levels.xml";
        xmlDoc.Load(filePath);
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList levelsNode = root.SelectNodes("/levels/level");
        foreach (XmlElement xe in levelsNode) 
        {
            //根据名称找到对应的关卡
            if(xe.GetAttribute("name")==name)
            {
                //根据unlock重新为关卡赋值
                if(unlock){
                    xe.SetAttribute("unlock","1");
                }else{
                    xe.SetAttribute("unlock","0");
                }
            }
        }

        //保存文件
        xmlDoc.Save (filePath);
    }
    
    
    public static void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levelnames.Count)
        {
            Debug.Log("已经是最后一关了");
            return;
        }
        else
        {
            WinAndEnterLevelPanel(levelnames[currentLevelIndex]);
        }
    }
    public static void WinAndEnterLevelPanel(string levelName)
    {
        //如果此时应该进入下一个关卡，那么应该是先进入选择关卡界面？？还是直接出现panel，显示下一关
        //直接显示成功+下一关按钮
        
        YGameRoot.Instance.Pop();//有可能需要popall
        YGameRoot.Instance.Push(new YWinAndNextLevelPanel(levelName));
        
        //解锁下一关
        YLevelManager.SetLevels(levelName,true);
        //然后应该是加载下一个关卡所有所需要的资源
    }
    
    public static void LoadAndBeginLevel(string levelName)
    {
        int levelID = levelnames.IndexOf(levelName);
        //如果点击新的关卡
        Debug.Log("LoadAndBeginLevel"+levelName);
        //加载关卡 这里要做的事情很多 比如要加载关卡的资源，加载关卡的UI，加载关卡的数据等等
        //删掉原来的角色 
        YPlayModeController.Instance.EnterNewLevel();
        
        //删掉木偶
        YScreenPlayController.Instance.EnterNewLevel();
        
        yPlanningTable.Instance.EnterNewLevel(levelID);
    }
    
    public static void LoadAndBeginLevel()
    {
        // #if BUILD_MODE
        //     currentLevelIndex = 3;
        // #endif
        string levelName = levelnames[currentLevelIndex];
        int levelID = currentLevelIndex;
        //如果点击新的关卡
        Debug.Log("LoadAndBeginLevel"+levelName);
        //加载关卡 这里要做的事情很多 比如要加载关卡的资源，加载关卡的UI，加载关卡的数据等等
        //删掉原来的角色 
        YPlayModeController.Instance.EnterNewLevel();
        
        //删掉木偶
        YScreenPlayController.Instance.EnterNewLevel();
        
        yPlanningTable.Instance.EnterNewLevel(levelID);
        
        
    }
    public static void SetCurrentLevelIndex(int index)
    {
        currentLevelIndex = index;
        mpanel.SetCurLevel(index);
    }
    
    public static void JustSetCurrentLevelIndex(int index)
    {
        currentLevelIndex = index;
    }

    public static int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    public static void SetCurrentLevelName(string name)
    {
        currentLevelIndex = levelnames.IndexOf(name);
    }
    
    
    
}
