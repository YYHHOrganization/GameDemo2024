using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;


public class YRouge_ItemRoom : YRouge_RoomBase
{
    // Start is called before the first frame update
    private List<GameObject> items = new List<GameObject>();
    void Start()
    {
        roomType = RoomType.ItemRoom;
        base.Start();
        
        ReadItemRoomData();
    }

    private void ReadItemRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        int randomIndex = Random.Range(0, SD_ItemRoomCSVFile.Class_Dic.Count);
        Debug.Log("randomIndex: " + randomIndex);
        
        // Class_ItemRoomCSVFile itemRoomData = SD_ItemRoomCSVFile.Class_Dic["ItemRoom" + randomIndex];
        Class_ItemRoomCSVFile itemRoomData = SD_ItemRoomCSVFile.Class_Dic["6661000"+randomIndex];
        
        //获取应该有几个物体摆放在这个房间中
        int itemCount = itemRoomData._ItemCount();
        
        // testSpecialRoom();
        // return;
        
        //生成摆放物体的位置
        List<Vector3> itemPosition = new List<Vector3>();
        itemPosition=GenerateItemTransorm(itemCount);
        
        //解析道具房间数据 itemRoomData.ItemIDField
        if(itemRoomData.ItemIDField == "all")
        {
            Debug.Log("随机选取所有道具");
            GenerateItem(itemCount,itemPosition);
            
        }
        else
        {
            string[] itemIDs = itemRoomData.ItemIDField.Split(';');
            //将ids变为list
            List<string> itemIDList = new List<string>(itemIDs);
            //从中取出itemcount个，生成
            for (int i = 0; i < itemCount; i++)
            {
                int randomItemIndex = Random.Range(0, itemIDList.Count);
                items.Add(HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(itemIDList[randomItemIndex], transform,itemPosition[i]+new Vector3(0,0.5f,0)));
                itemIDList.RemoveAt(randomItemIndex);
                GenerateEffPlatform(transform,itemPosition[i]);
            }
        }
        //生成道具

    }

    private void GenerateItem(int itemCount, List<Vector3> itemPosition)
    {
        for (int i = 0; i < itemCount; i++)
        {
            items.Add(HRoguePlayerAttributeAndItemManager.Instance.RollingARandomItem(transform,itemPosition[i]+new Vector3(0,0.5f,0)));
            GenerateEffPlatform(transform,itemPosition[i]);
        }
    }


    //如果1个，那么就是0，0，0
    private List<Vector3> bias1PosList = new List<Vector3>() { new Vector3(0,0,0)};
    //如果2个，那么就是-5，0，0 5，0，0
    private List<Vector3> bias2PosList = new List<Vector3>() { new Vector3(-5,0,0),new Vector3(5,0,0)};
    //如果3个，那么就是-5，0，0 0，0，0 5，0，0
    private List<Vector3> bias3PosList = new List<Vector3>() { new Vector3(-5,0,0),new Vector3(0,0,0),new Vector3(5,0,0)};
    //如果4个，那么就是-5，0，5 5，0，5 -5，0，-5 5，0，-5
    private List<Vector3> bias4PosList = new List<Vector3>() { new Vector3(-5,0,5),new Vector3(5,0,5),new Vector3(-5,0,-5),new Vector3(5,0,-5)};
    //如果5个，那么就是-5，0，5 5，0，5 -5，0，-5 5，0，-5 0，0，0
    private List<Vector3> bias5PosList = new List<Vector3>() { new Vector3(-5,0,5),new Vector3(5,0,5),new Vector3(-5,0,-5),new Vector3(5,0,-5),new Vector3(0,0,0)};
    //如果6个，那么就是-5，0，5 5，0，5 -5，0，-5 5，0，-5 0，0，5 0，0，-5
    private List<Vector3> bias6PosList = new List<Vector3>() { new Vector3(-5,0,5),new Vector3(5,0,5),new Vector3(-5,0,-5),new Vector3(5,0,-5),new Vector3(0,0,5),new Vector3(0,0,-5)};
    private List<Vector3> GenerateItemTransorm(int itemCount)
    {
        //生成摆放物体的位置 总房间大小是20，20
        //count其实应该只有1，2，3，4，5等固定的吧，固定排布可能比较好
        //假设中心为0，0，0 那么以这个为基准加和减
        
        //如果1个，那么就是0，0，0
        //如果2个，那么就是-5，0，0 5，0，0
        //如果3个，那么就是-5，0，0 0，0，0 5，0，0
        //如果4个，那么就是-5，0，5 5，0，5 -5，0，-5 5，0，-5
        //如果5个，那么就是-5，0，5 5，0，5 -5，0，-5 5，0，-5 0，0，0
        //如果6个，那么就是-5，0，5 5，0，5 -5，0，-5 5，0，-5 0，0，5 0，0，-5
        switch (itemCount)
        {
            case 1:
                return bias1PosList;
                break;
            case 2:
                return bias2PosList;
                break;
            case 3:
                return bias3PosList;
                break;
            case 4:
                return bias4PosList;
                break;
            case 5:
                return bias5PosList;
                break;
            case 6:
                return bias6PosList;
                break;
        }
        return null;
    }

    private void GenerateEffPlatform(Transform parent,Vector3 pos)
    {
        // YPlatformRogue
        string AddLink = "YPlatformRogue";
        GameObject itemEffPlatform = Addressables.InstantiateAsync(AddLink, parent).WaitForCompletion();
        itemEffPlatform.transform.localPosition = pos;
    }

    private void testSpecialRoom()
    {
        // 66610003
        Class_ItemRoomCSVFile itemRoomData = SD_ItemRoomCSVFile.Class_Dic["66610003"];
        HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(itemRoomData.ItemIDField, transform,new Vector3(0,0.3f,0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void SetResultOn()
    {
        base.SetResultOn();
        //所有的道具Icon面向玩家
        foreach (var item in items)
        {
            if (item!=null && item.GetComponent<HRogueItemBase>())
            {
                item.GetComponent<HRogueItemBase>().SetBillboardEffect();
            }
        }
    }
}
