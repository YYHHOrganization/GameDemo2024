using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

using System.Runtime.InteropServices;
public class YRouge_ShopRoom : YRouge_RoomBase
{
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.ShopRoom;
        base.Start();
        
        ReadItemRoomData();
    }
    private string buyCurrency = "20000013";//"信用点";
    private void ReadItemRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        int randomIndex = Random.Range(0, SD_ShopRoomCSVFile.Class_Dic.Count);
        
        //TEST
        // randomIndex = 9;
        
        Debug.Log("randomIndex: " + randomIndex);
        
        // Class_ItemRoomCSVFile itemRoomData = SD_ItemRoomCSVFile.Class_Dic["ItemRoom" + randomIndex];
        Class_ShopRoomCSVFile itemRoomData = SD_ShopRoomCSVFile.Class_Dic["6666000"+randomIndex];
        
        //获取应该有几个物体摆放在这个房间中
        int itemCount = itemRoomData._ItemCount();
        buyCurrency = itemRoomData.ShopCurrencyID;
        string ShopCurrencyCount = itemRoomData.ShopCurrencyCount;
        //解析ShopCurrencyCount 应该是每个物品都要有一个价格
        // int CurrencyCount = GenerateShopCurrencyCount(ShopCurrencyCount);
        
        // string NPC = itemRoomData.NPC;
        
        // testSpecialRoom();
        // return;
        
        //生成摆放物体的位置
        List<Vector3> itemPosition = new List<Vector3>();
        itemPosition=GenerateItemTransorm(itemCount);
        
        //解析道具房间数据 itemRoomData.ItemIDField
        if(itemRoomData.ItemIDField == "all")
        {
            Debug.Log("随机选取所有道具");
            GenerateItem(itemCount,itemPosition,buyCurrency,ShopCurrencyCount);
            
        }
        else
        {
            string[] itemIDs = itemRoomData.ItemIDField.Split(';');
            //将ids变为list
            List<string> itemIDList = new List<string>(itemIDs);
            //从中取出itemcount个，生成
            for (int i = 0; i < itemCount; i++)
            {
                int CurrencyCount = GenerateShopCurrencyCount(ShopCurrencyCount);
                int randomItemIndex = Random.Range(0, itemIDList.Count);
                
                GameObject obj = HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem
                    (itemIDList[randomItemIndex], 
                        transform,
                        itemPosition[i]+new Vector3(0,0.5f,0),
                        isShop:true,
                        buyCurrency:buyCurrency,
                        howMuch:CurrencyCount);
                itemIDList.RemoveAt(randomItemIndex);
                GenerateEffPlatform(transform,itemPosition[i],buyCurrency:buyCurrency,price:CurrencyCount);
                obj.GetComponent<HRogueItemBase>().SetBillboardEffect();
            }
        }
        //生成道具

    }
    int iSeed = 10;
    private int GenerateShopCurrencyCount(string shopCurrencyCount)
    {
        //解析ShopCurrencyCount
        string[] currencyCounts = shopCurrencyCount.Split(':');
        int minCount = int.Parse(currencyCounts[0]);
        int maxCount = int.Parse(currencyCounts[1]);
        
        //时间戳作为随机种子
        // Random.InitState((int)System.DateTime.Now.Ticks);
        // System.Random random = new System.Random((int)System.DateTime.Now.Ticks);
        // float j = UnityEngine.Random.Range(minCount, maxCount);
        // var randomRestul = random.Next(0, 100);
        
        // int CurrencyCount = Random.Range(minCount, maxCount);
        
        iSeed++;
        System.Random random = new System.Random(iSeed);
        int CurrencyCount = random.Next(minCount, maxCount);
        //
        // System.Random rd = new System.Random(GetRandom(iSeed));
        // int CurrencyCount = rd.Next(minCount, maxCount);
        return CurrencyCount;

    }

    public int GetRandom(object o)
    {
        GCHandle h= GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);
        System.IntPtr addr = GCHandle.ToIntPtr(h);
        return int.Parse( addr.ToString());
    }
    
  
    private int GetMoney()
    {
        //给这个shopplatform更改一下价格str
        int priceStr = Random.Range(0, 100);//随机生成一个价格，但其实应该从item表格读取
        return priceStr;
    }

    private void GenerateItem(int itemCount, List<Vector3> itemPosition,string buyCurrency,string ShopCurrencyCount)
    {
        for (int i = 0; i < itemCount; i++)
        {
            int CurrencyCount = GenerateShopCurrencyCount(ShopCurrencyCount);
            //先暂时都信用点吧
            
            GameObject obj = HRoguePlayerAttributeAndItemManager.Instance.RollingARandomItem
                (transform,
                    itemPosition[i]+new Vector3(0,0.5f,0),
                    isShop:true,
                    buyCurrency:buyCurrency,
                    howMuch:CurrencyCount);
            GenerateEffPlatform(transform,itemPosition[i],buyCurrency:buyCurrency,price:CurrencyCount);
            obj.GetComponent<HRogueItemBase>().SetBillboardEffect();
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

    private void GenerateEffPlatform(Transform parent,Vector3 pos,int price,string buyCurrency)
    {
        // YPlatformRogue
        string AddLink = "YShopPlatformRogue";//"YPlatformRogue";
        GameObject itemEffPlatform = Addressables.InstantiateAsync(AddLink, parent).WaitForCompletion();
        itemEffPlatform.transform.localPosition = pos;
        
        //buyCurrency = "20000013";//"信用点";
        //看yPlanningTable.Instance.worldItems是否有这个buyCurrency
        string priceStr = "";
        if (yPlanningTable.Instance.worldItems.ContainsKey(buyCurrency))
        {
            priceStr = yPlanningTable.Instance.worldItems[buyCurrency].chineseName;
        }
        else
        {
            priceStr = yPlanningTable.Instance.rogueItemBases[buyCurrency].itemChineseName;
        }
        priceStr = "<size=50%>" + priceStr + "</size>";
        priceStr = "<size=100%>"+price+ "</size>" + "\n" + priceStr;
        
        // //给这个shopplatform更改一下价格str
        // string priceStr = Random.Range(1, 10).ToString();//随机生成一个价格，但其实应该从item表格读取
        // priceStr = priceStr + "\n信\n用\n点";
        itemEffPlatform.GetComponent<YShopPlatformRogue>().SetPriceStr(priceStr);
    }

    private void testSpecialRoom()
    {
        // 66610003
        Class_ShopRoomCSVFile itemRoomData = SD_ShopRoomCSVFile.Class_Dic["66660003"];
        HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(itemRoomData.ItemIDField, transform,new Vector3(0,0.3f,0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
