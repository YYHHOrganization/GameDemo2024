
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
public static class YCharacterInfoManager 
{
    // static int currentLevelIndex = 0;
    // static List<string> levelnames;
    // static YLevelPanel mpanel;
    

    /// <summary> 
    /// 加载Xml文件 
    /// </summary>
    /// <returns>The Character.</returns>
    public static List<YCharacterInfo> LoadCharacterInfo()
    {
        List<string> names = new List<string>();
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        //如果本地存在配置文件则读取配置文件
        //否则在本地创建配置文件的副本
        //为了跨平台及可读可写，需要使用Application.persistentDataPath
        string filePath = Application.dataPath + "/Designer/XMLTable/charactersXML.xml";
        
        // if (!IOUntility.isFileExists (filePath)) {
        //     xmlDoc.LoadXml (((TextAsset)Resources.Load ("levels")).text);
        //     IOUntility.CreateFile (filePath, xmlDoc.InnerXml);
        // } else {
        //     xmlDoc.Load(filePath);
        // }
        xmlDoc.Load(filePath);
        
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList Node = root.SelectNodes("/characters/character");
        //初始化关卡列表
        List<YCharacterInfo> characters = new List<YCharacterInfo>();
        foreach (XmlElement xe in Node) 
        {
            // DocumentationSortingAttribute.Level l=new DocumentationSortingAttribute.Level();
            YCharacterInfo l = new YCharacterInfo();
            l.ID=xe.GetAttribute("id");
            l.Name=xe.GetAttribute("name");
            l.UIName=xe.GetAttribute("uiname");
            //使用unlock属性来标识当前关卡是否解锁
            if(xe.GetAttribute("unlock")=="1"){
                l.UnLock=true;
            }else{
                l.UnLock=false;
            }

            characters.Add(l);
            
            names.Add(l.Name);
        }

        return characters;
    }

    /// <summary>
    /// 设置某一(元组)的状态
    /// </summary>
    /// <param name="name">(元组)名称</param>
    /// <param name="locked">是否解锁</param>
    public static void SetStatusByName(string name,bool unlock)
    {
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        string filePath = Application.dataPath + "/Designer/XMLTable/charactersXML.xml";
        xmlDoc.Load(filePath);
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList levelsNode = root.SelectNodes("/characters/character");
        foreach (XmlElement xe in levelsNode) 
        {
            //根据名称找到对应的
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

    public static void SetCatcakeStatusByID(string id, bool unlock)
    {
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        string filePath = Application.dataPath + "/Designer/XMLTable/PlayerContentInfos.xml";
        xmlDoc.Load(filePath);
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList levelsNode = root.SelectNodes("/PlayerSaveFile/CatcakeInfo/catcake");
        foreach (XmlElement xe in levelsNode) 
        {
            Debug.Log(xe.GetAttribute("id"));
            //根据名称找到对应的
            if(xe.GetAttribute("id")==id)
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
    
    public static void SetStatusByID(string id,bool unlock)
    {
        //创建Xml对象
        XmlDocument xmlDoc = new XmlDocument();
        string filePath = Application.dataPath + "/Designer/XMLTable/charactersXML.xml";
        xmlDoc.Load(filePath);
        XmlElement root = xmlDoc.DocumentElement;
        XmlNodeList levelsNode = root.SelectNodes("/characters/character");
        foreach (XmlElement xe in levelsNode) 
        {
            //根据名称找到对应的
            if(xe.GetAttribute("id")==id)
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

    //通过ID获取角色信息 ，比如获取是否上锁
    public static YCharacterInfo GetCharacterInfoByID(string id)
    {
        List<YCharacterInfo> characters = LoadCharacterInfo();
        foreach (YCharacterInfo character in characters)
        {
            if (character.ID == id)
            {
                return character;
            }
        }
        return null;
    }
  
    //通过ID获取角色信息 ，比如获取是否上锁，返回是否上锁，
    public static bool GetCharacterUnLockStatusByID(string id)
    {
        List<YCharacterInfo> characters = LoadCharacterInfo();
        foreach (YCharacterInfo character in characters)
        {
            if (character.ID == id)
            {
                return character.UnLock;
            }
        }
        return false;
    }
    
}
