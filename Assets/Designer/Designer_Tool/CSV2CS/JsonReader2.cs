using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonReader2
{
    static string mFileName;

    static string mFolderName;

    //表格内第二级字段对应内容,表格第一列需要标明每一行的ID或者用途
    public static Dictionary<string, JsonData> data_Value = new Dictionary<string, JsonData>();
    public static Dictionary<string, string> dic_Value = new Dictionary<string, string>();
    
    //文件路径，合并文件夹名和文件名
    static string FileName{
        get{ 
            return Path.Combine (FolderName,mFileName);
        }
    }
    //文件夹路径，合并persistentDataPath和文件夹名
    static string FolderName{
        get{ 
            return Path.Combine ("/",mFolderName);
        }
    }
    
    //读取json内容
    static void read(){
        try {
            //如果没有文件夹则创建之
            if (!Directory.Exists (FolderName)) {
                Directory.CreateDirectory (FolderName);
                Debug.Log ("没有找到json文件路径，创建文件夹："+FolderName);
            }
            TextAsset ta = Resources.Load(FileName) as TextAsset;
            //保存json中的数据到jd
            JsonData jd = JsonMapper.ToObject (ta.text);
            foreach (var key in jd.Keys) { 
                data_Value.Add(key,jd[key]);}
        } catch (System.Exception ex) {
            Debug.Log ("该键/值已经存在："+ex);
        }}
    /// 根据路径初始化Dictionary和文件路径信息读取文件

    public static void init_Read(string pFolderName,string pFileName){
        mFileName = pFileName;
        mFolderName = pFolderName;
        data_Value.Clear ();
        dic_Value.Clear ();
        read ();
    }
    
    /// 判断当前是否存在该key值  

    public static bool HasKey(string pKey) {
        if (data_Value.ContainsKey (pKey)==false) {
            Debug.Log ("【"+pKey+"】"+"查找失败！");
        }
        return data_Value.ContainsKey(pKey);  
    }  
    public static bool HasKey_Key(string pKey_Key) {
        if (data_Value.ContainsKey (pKey_Key)==false) {
            Debug.Log ("【"+pKey_Key+"】"+"查找失败！");
        }
        return dic_Value.ContainsKey(pKey_Key);  
    }
    
    /// 读取string值  

    // public static string GetString(string pKey,string pKey_Key) { 
    //     data_jd = data_Value [pKey];
    //     foreach (var key in data_jd.Keys) { 
    //         dic_Value.Add(key,data_jd[key].ToString());
    //     }
    //     //判断是否包含查找对象，否则返回空
    //     if(HasKey(pKey) && HasKey_Key(pKey_Key)) { 
    //         return dic_Value[pKey_Key]; 
    //     } else {  
    //         Debug.Log (pKey+" 或者 "+pKey_Key+" 查找失败！");
    //         return string.Empty;  
    //     }  
    // }
    //
}

