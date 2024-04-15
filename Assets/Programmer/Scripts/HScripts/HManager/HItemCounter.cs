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
    private Dictionary<string, int> rogueItemCounts = new Dictionary<string, int>();

    public Dictionary<string, int> WorldItemCounts
    {
        get => worldItemCounts;
    }
    
    public Dictionary<string, int> RogueItemCounts
    {
        get => rogueItemCounts;
    }

    public int GetDictLength()
    {
        return worldItemCounts.Count;
    }

    public int GetRogueDictLength()
    {
        return rogueItemCounts.Count;
    }
    
    // private void DebugDictionary()
    // {
    //     foreach (var item in worldItemCounts)
    //     {
    //         Debug.Log(item.Key + " " + item.Value);
    //     }
    // }

    private void Update()
    {
        // if (debugMode)
        // {
        //     DebugDictionary();
        //     debugMode = false;
        // }
    }

    public void AddItemInRogue(string itemId, int count)
    {
        if (rogueItemCounts.ContainsKey(itemId))
        {
            rogueItemCounts[itemId] += count;
        }
        else
        {
            rogueItemCounts.Add(itemId, count);
        }
    }
    
    public void RemoveItemInRogue(string itemId, int count)
    {
        if (rogueItemCounts.ContainsKey(itemId))
        {
            rogueItemCounts[itemId] -= count;
            if (rogueItemCounts[itemId] <= 0)
            {
                rogueItemCounts.Remove(itemId);
            }
        }
    }
    public bool CheckAndRemoveItemInRogue(string itemId, int count)
    {
        if(count<=0) return true;
        if (rogueItemCounts.ContainsKey(itemId))
        {
            //如果数量不够
            if(rogueItemCounts[itemId] - count < 0)
            {
                return false;
            }
            rogueItemCounts[itemId] -= count;
            if (rogueItemCounts[itemId] <= 0)
            {
                rogueItemCounts.Remove(itemId);
                return true;
            }
        }
        else if(worldItemCounts.ContainsKey(itemId))
        {
            if(worldItemCounts[itemId] - count < 0)
            {
                return false;
            }
            RemoveItem(itemId, count);
            return true;
        }
        
        return false;
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
            HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueXingqiong"] = worldItemCounts[itemId];
            HRoguePlayerAttributeAndItemManager.Instance.UpdateEverythingInAttributePanel();
        }
        else if(itemId == "20000013")
        {
            HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueXinyongdian"] = worldItemCounts[itemId];
            HRoguePlayerAttributeAndItemManager.Instance.UpdateEverythingInAttributePanel();
        }
        
    }
    
    public void RemoveItem(string itemId, int count)
    {
        if (worldItemCounts.ContainsKey(itemId))
        {
            worldItemCounts[itemId] -= count;
            if(itemId == "20000012")
            {
                HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueXingqiong"] = worldItemCounts[itemId];
                HRoguePlayerAttributeAndItemManager.Instance.UpdateEverythingInAttributePanel();
            }
            else if(itemId == "20000013")
            {
                HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueXinyongdian"] = worldItemCounts[itemId];
                HRoguePlayerAttributeAndItemManager.Instance.UpdateEverythingInAttributePanel();
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
