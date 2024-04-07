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

    public Dictionary<string, int> WorldItemCounts
    {
        get => worldItemCounts;
    }

    public int GetDictLength()
    {
        return worldItemCounts.Count;
    }
    
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
        if(itemId == "20000012")
        {
            HRougeAttributeManager.Instance.characterValueAttributes["RogueXingqiong"] = worldItemCounts[itemId];
            HRougeAttributeManager.Instance.UpdateEverythingInAttributePanel();
        }
        else if(itemId == "20000013")
        {
            HRougeAttributeManager.Instance.characterValueAttributes["RogueXinyongdian"] = worldItemCounts[itemId];
            HRougeAttributeManager.Instance.UpdateEverythingInAttributePanel();
        }
        
    }
    
    public void RemoveItem(string itemId, int count)
    {
        if (worldItemCounts.ContainsKey(itemId))
        {
            worldItemCounts[itemId] -= count;
            if(itemId == "20000012")
            {
                HRougeAttributeManager.Instance.characterValueAttributes["RogueXingqiong"] = worldItemCounts[itemId];
                HRougeAttributeManager.Instance.UpdateEverythingInAttributePanel();
            }
            else if(itemId == "20000013")
            {
                HRougeAttributeManager.Instance.characterValueAttributes["RogueXinyongdian"] = worldItemCounts[itemId];
                HRougeAttributeManager.Instance.UpdateEverythingInAttributePanel();
            }
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
