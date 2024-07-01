using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

/// <summary>
/// 是单例
/// </summary>
public class YObjectPool : YSingleTemplate<YObjectPool>
{
    //用string名字来区分子对象池
    private Dictionary<string, YSubPool> objectPoolDic;
    protected override void Awake()
    {
        base.Awake();
        objectPoolDic = new Dictionary<string, YSubPool>();
    }

    private void Start()
    {
        // string nameid = "33300000";
        // CreateSubPool(nameid);
        //
        // CreateSubPool("33300001");
        
        CreateSubPool("33300004");
        CreateSubPool("33300005");
        CreateSubPool("33310000");
    }

    //取对象
    public event Action<GameObject> ObjectSpawned;
    public GameObject Spawn(string nameid)
    {
        GameObject go = null;
        if (!objectPoolDic.ContainsKey(nameid))//没有子对象池
        {
            CreateSubPool(nameid);
        }
        go = objectPoolDic[nameid].GetPooledObject();

        if (SD_ObjectPoolCSVFile.Class_Dic[nameid]._isRecallable() == 1)
        {
            //感觉这里耦合了 用观察者模式可能能解决，这里我们是需要将go加入到recall的列表中
            ObjectSpawned?.Invoke(go);
        }
        return go;
    }
    
   
    //回收对象
    public void UnSpawn(GameObject go)
    {
        foreach (YSubPool item in objectPoolDic.Values)
        {
            if (item.Contains(go))
            {
                item.UnSpawn(go);
                return;
            }
            else
            {
                Debug.Log("没有改对象!");
            }
        }
    }
    //创建子对象池
    private void  CreateSubPool(string nameid)
    {
        YSubPool sub = new YSubPool(nameid, transform);
        objectPoolDic.Add( nameid, sub);
    }
    private void  CreateSubPool(string name,GameObject prefab)
    {
        
        YSubPool sub = new YSubPool(name, transform);
        objectPoolDic.Add(name, sub);
    }
}
