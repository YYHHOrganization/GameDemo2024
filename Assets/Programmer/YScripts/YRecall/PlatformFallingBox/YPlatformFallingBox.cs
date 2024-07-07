using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YPlatformFallingBox :YRoomAppendant
{
    public string fallingBoxPrefabID = "33310002";

    public int number = 1;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    protected override void EnterRoomHandle()
    {
        base.EnterRoomHandle();
        //生成掉落盒子
        GenerateFallingBox();

        GenerateReward();
    }

    private void GenerateReward()
    {
        //借用一下双子塔那个表吧
        string specialMapID = "YSpecialMap_ShuangZiTa";
        Class_RogueSpecialMapFile specialMapData = 
            SD_RogueSpecialMapFile.Class_Dic[specialMapID];
        
        //解析道具房间数据 itemRoomData.ItemIDField
        string[] itemIDs = specialMapData.ItemIDField.Split(';');
        //将ids变为list
        List<string> itemIDList = new List<string>(itemIDs);
        int randomItemIndex = Random.Range(0, itemIDList.Count);
        GameObject reward = HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem
        (
            itemIDList[randomItemIndex], 
            // transform,
            transform,
            new Vector3(0,0.5f,0)
        );
        reward.transform.localScale = new Vector3(0.05f,0.05f,0.05f);
        
    }

    private void GenerateFallingBox()
    {
        for (int j = 0; j < number; j++)
        {
            GameObject go = YObjectPool._Instance.Spawn(fallingBoxPrefabID);
            
            //在其前后 各生成2个盒子
            // Random.Range(-2,2),0,Random.Range(-2,2));
            go.transform.position = transform.position+new Vector3(0,0,j*8-4);
            go.SetActive(true);
        }
    }
}
