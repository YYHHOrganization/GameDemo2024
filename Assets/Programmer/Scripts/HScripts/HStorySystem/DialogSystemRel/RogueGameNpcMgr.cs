using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RogueGameNpcMgr : MonoBehaviour
{
    private Dictionary<string, GameObject> npcDict = new Dictionary<string, GameObject>();

    private void AddNpcsToDict()
    {
        foreach(var npc in SD_RogueGameNPCConfig.Class_Dic)
        {
            if (npc.Value.NPCAlwaysExits == "True") //常驻NPC
            {
                GameObject npcPrefab = Addressables.LoadAssetAsync<GameObject>(npc.Value.NPCAddressable)
                    .WaitForCompletion();
                //194.0307;1.265049;-226.779,类似这种，解析NPC的位置
                string[] posStr = npc.Value.NPCLocation.Split(';');
                Vector3 pos = new Vector3(float.Parse(posStr[0]), float.Parse(posStr[1]), float.Parse(posStr[2]));
                GameObject npcObj = Instantiate(npcPrefab, pos, Quaternion.identity);
                npcDict.Add(npc.Key, npcObj);
            }
        }
    }
    
    private void Start()
    {
        AddNpcsToDict();
    }

    public bool CheckIfNpcExist(string npcId)
    {
        return npcDict.ContainsKey(npcId);
    }
    
    public void AddNpc(string npcId, GameObject npc)
    {
        if (!npcDict.ContainsKey(npcId))
        {
            npcDict.Add(npcId, npc);
        }
    }
    
    public void RemoveNpc(string npcId)
    {
        if (npcDict.ContainsKey(npcId))
        {
            npcDict.Remove(npcId);
        }
    }
    
    public GameObject GetNpcByID(string npcId)
    {
        if (npcDict.ContainsKey(npcId))
        {
            return npcDict[npcId];
        }
        return null;
    }
}
