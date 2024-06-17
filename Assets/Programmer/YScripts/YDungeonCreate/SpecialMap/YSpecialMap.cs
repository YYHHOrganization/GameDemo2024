using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class YSpecialMap : MonoBehaviour
{
    public string mapName;
    [SerializeField]private Transform PlaceChestCenter;
    [SerializeField]private string specialMapID;
    private List<GameObject> items = new List<GameObject>();
    //平台 放宝物的
    List<GameObject> platformList = new List<GameObject>();
    [SerializeField]YPortalToSomeWhere portalToSomeWhere_ExitPortal;
    //unity外部可改
    [SerializeField]
    private Transform bornPlace;
    //get

    //当进入特殊地图之后，有一个函数会一直监听是否离开了地图
    
    
    public Transform BornPlace
    {
        get
        {
            return bornPlace;
        }
    }
    
    [SerializeField]
    private Transform exitPlace;
    //get
    public Transform ExitPlace
    {
        get
        {
            return exitPlace;
        }
    }
    [SerializeField]
    private Transform landingPlace; 
    //get
    public Transform LandingPlace
    {
        get
        {
            return landingPlace;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //test
        // ReadSpecialMapData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField]private GameObject CameraIn;
    [SerializeField]private GameObject CameraIn2;
    public void OnPlayerEnter()
    {
        
        //OnPlayerExit+=portalToSomeWhere_ExitPortal.OnPlayerExit;
        //此时让OnPlayerExit监听 portalToSomeWhere_ExitPortal.OnPlayerPortal
        portalToSomeWhere_ExitPortal.OnPlayerPortal += OnPlayerExit;//意思是：
             //portalToSomeWhere_ExitPortal.OnPlayerPortal这个函数执行的时候，
             //OnPlayerExit这个函数也会执行
        
        
        //设置角色出生点
        ReadSpecialMapData();
        //展示相机出现
        CameraIn.SetActive(true);
        DOVirtual.DelayedCall(0.3f, () =>
        {
            CameraIn.SetActive(false);
            CameraIn2.SetActive(true);
        });
        DOVirtual.DelayedCall(0.3f+2.1f, () =>
        {
            CameraIn2.SetActive(false);
            SetBillboard();
        });
        
        //开启雾效
        SetFogOnOrFalse(true);
    }
    void SetFogOnOrFalse(bool isOn)
    {
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogHeight",isOn);
        HPostProcessingFilters.Instance.SetPostProcessingWithName("FogDistance",isOn);
    }
    private void OnPlayerExit()
    {
        Debug.Log("OnPlayerExit");
        //删除item
        foreach (var item in items)
        {
            Destroy(item);
        }
        items = new List<GameObject>();
        
        //删除平台
        foreach (var platform in platformList)
        {
            Destroy(platform);
        }
        platformList = new List<GameObject>();
        
        //关闭雾效
        SetFogOnOrFalse(false);
    }
    private void SetBillboard()
    {
        //所有的道具Icon面向玩家
        foreach (var item in items)
        {
            if (item!=null && item.GetComponent<HRogueItemBase>())
            {
                item.GetComponent<HRogueItemBase>().SetBillboardEffect();
            }
        }
    }

    //解析特殊地图的数据
    void ReadSpecialMapData()
    {
        // YSpecialMap_ShuangZiTa
        Class_RogueSpecialMapFile specialMapData = 
            SD_RogueSpecialMapFile.Class_Dic[specialMapID];
        
        //解析数据
        //获取应该有几个物体摆放在这个房间中
        // int itemCount = specialMapData._ItemCount();
        int itemCount = Random.Range(1,6);
        
        List<Vector3> itemPosition = new List<Vector3>();
        itemPosition=GenerateItemTransorm(itemCount);
        
        //解析道具房间数据 itemRoomData.ItemIDField
        string[] itemIDs = specialMapData.ItemIDField.Split(';');
        //将ids变为list
        List<string> itemIDList = new List<string>(itemIDs);
        
        //从中取出itemcount个，生成
        for (int i = 0; i < itemCount; i++)
        {
            int randomItemIndex = Random.Range(0, itemIDList.Count);
            items.Add
            (
                HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem
                (
                itemIDList[randomItemIndex], 
                // transform,
                PlaceChestCenter,
                itemPosition[i]+new Vector3(0,0.5f,0)
                )
            );
            itemIDList.RemoveAt(randomItemIndex);
            GenerateEffPlatform( PlaceChestCenter,itemPosition[i]);
        }
    }
    private void GenerateEffPlatform(Transform parent,Vector3 pos)
    {
        // YPlatformRogue
        string AddLink = "YPlatformRogue";
        GameObject itemEffPlatform = Addressables.InstantiateAsync(AddLink, parent).WaitForCompletion();
        itemEffPlatform.transform.localPosition = pos;
        
        platformList.Add(itemEffPlatform);
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
    
}