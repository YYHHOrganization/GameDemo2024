using UnityEngine;
using System.Collections.Generic;
using LitJson;

public static class JsonReader
{

    //读取json数据，根据id查找（表格里的第一行），返回id和类对象互相对应的字典。
    public static Dictionary<string, T> ReadJson<T>(string fileName)
    {
        TextAsset ta = Resources.Load(fileName) as TextAsset;
        if (ta.text == null) { Debug.Log("根据路径未找到对应表格数据"); };
        Dictionary<string, T> d = JsonMapper.ToObject<Dictionary<string, T>>(ta.text);
        return d;
    }

    //写入json数据，传入类类型变量。
    public static void WriteJson(string path, object jsonData)
    {
        JsonMapper.ToJson(jsonData);
    }
}