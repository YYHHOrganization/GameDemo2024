using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

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
    }

    //取对象
    public GameObject Spawn(string nameid)
    {
        GameObject go = null;
        if (!objectPoolDic.ContainsKey(nameid))//没有子对象池
        {
            CreateSubPool(nameid);
        }
        go = objectPoolDic[nameid].GetPooledObject();
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
