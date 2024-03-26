using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HItemCounter : MonoBehaviour
{
    //单例模式
    public static HItemCounter instance;
    public static HItemCounter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HItemCounter>();
            }

            return instance;
        }
    }
    
    public bool debugMode = false;

    private void Awake()
    {
        instance = this;
    }
    
    private Dictionary<string, int> worldItemCounts = new Dictionary<string, int>();
    
    private void DebugDictionary()
    {
        foreach (var item in worldItemCounts)
        {
            Debug.Log(item.Key + " " + item.Value);
        }
    }

    private void Update()
    {
        if (debugMode)
        {
            DebugDictionary();
            debugMode = false;
        }
    }

    public void AddItem(string itemId, int count)
    {
        if (worldItemCounts.ContainsKey(itemId))
        {
            worldItemCounts[itemId] += count;
        }
        else
        {
            worldItemCounts.Add(itemId, count);
        }
    }
    
    public void RemoveItem(string itemId, int count)
    {
        if (worldItemCounts.ContainsKey(itemId))
        {
            worldItemCounts[itemId] -= count;
            if (worldItemCounts[itemId] <= 0)
            {
                worldItemCounts.Remove(itemId);
            }
        }
    }
    
    public int CheckCountWithItemId(string itemId)
    {
        if (worldItemCounts.ContainsKey(itemId))
        {
            return worldItemCounts[itemId];
        }
        else
        {
            return 0;
        }
    }
}
