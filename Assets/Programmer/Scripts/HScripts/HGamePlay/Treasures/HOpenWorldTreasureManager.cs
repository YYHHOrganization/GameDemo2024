using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HOpenWorldTreasureManager : MonoBehaviour
{
    // 单例模式，控制大世界宝箱的生成，以及计数，读档存档等功能
    private static HOpenWorldTreasureManager instance;
    public static HOpenWorldTreasureManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HOpenWorldTreasureManager>();
            }

            return instance;
        }
    }

    private Dictionary<string, HOpenWorldTreasureStruct> treasureTypes =
        new Dictionary<string, HOpenWorldTreasureStruct>();
    private List<HOpenWorldTreasure> treasures = new List<HOpenWorldTreasure>();

    private void Awake()
    {
        instance = this;
    }

    public void InstantiateAllTreasures()
    {
        //todo: 读档，根据存档数据生成玩家还没有开启的宝箱
        //暂时先把所有的宝箱生成出来
        
    }

    public void ReadCSVFile(string treasureLayoutPath, string treasureDataPath)
    {
        //把所有的宝箱类型和对应开出物品的逻辑读取完毕
        string[] fileData = File.ReadAllLines(treasureDataPath);
        for (int i = 3; i < fileData.Length; i++)
        {
            string[] rowData = fileData[i].Split(',');
            HOpenWorldTreasureStruct treasure = new HOpenWorldTreasureStruct();
            string treasureTypeId = rowData[0];
            treasure.id = int.Parse(rowData[0]);
            treasure.SetFixedItems(rowData[5], rowData[6]);
            treasure.treasureType = (TreasureType) Enum.Parse(typeof(TreasureType), rowData[1]);
            treasure.addressableLink = rowData[7];
            
            treasure.SetRandomItems(rowData[3], rowData[4]);
            treasure.UILayoutType = rowData[8];
            
            treasureTypes.Add(treasureTypeId, treasure);
        }
        
        //开始读取大世界的所有宝箱并生成，把宝箱的类型存储进去
        //InstantiateTreasureWithInfo(treasureLayoutPath);
        //StartCoroutine(InstantiateTreasureWithInfo(treasureLayoutPath));
    }
    
    Dictionary<string, GameObject> treasureLayouts = new Dictionary<string, GameObject>();
    //Get
    public GameObject GetTreasureLayout(string id)
    {
        return treasureLayouts[id];
    }

    IEnumerator InstantiateTreasureWithInfo(string treasureLayoutPath)
    {
        string[] fileData = File.ReadAllLines(treasureLayoutPath);
        for (int i = 3; i < fileData.Length; i++)
        {
            string[] rowData = fileData[i].Split(',');
            string root = rowData[2];
            string treasureTypeId = rowData[1];
            string[] position = rowData[3].Split(';');
            Vector3 pos = new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2]));
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(treasureTypes[treasureTypeId].addressableLink, pos, Quaternion.identity, GameObject.Find(root).transform);
            yield return handle;
            //用这种协程的方式去写，可以保证每个宝箱都有自己的信息,不然不好对Instantiate得到的结果进行处理
            handle.Result.GetComponent<HOpenWorldTreasure>().SetTreasure(treasureTypes[treasureTypeId]);

            if (rowData[5] == "0")
            {
                handle.Result.gameObject.SetActive(false);
            }
            treasureLayouts.Add(rowData[0], handle.Result);
        }
    }
    
    public void InstantiateATreasureAndSetInfoWithTypeId(string treasureTypeId, Vector3 position, Transform root)
    {
        var op = Addressables.InstantiateAsync(treasureTypes[treasureTypeId].addressableLink, position, Quaternion.identity, root.transform);
        GameObject result = op.WaitForCompletion();
        result.GetComponent<HOpenWorldTreasure>().SetTreasure(treasureTypes[treasureTypeId]);
    }
    
    private void InstantiateTreasure(string name, Vector3 position, string root)
    {
        Addressables.InstantiateAsync(name, position, Quaternion.identity, GameObject.Find(root).transform).Completed += OnInstantiateDone;
    }

    private void OnInstantiateDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log("Instantiate Done" + obj.Result.name);
        
    }

    public void TestInstantiateAddressable(string name)
    {
        Addressables.LoadAssetAsync<GameObject>(name).Completed += OnLoadDone;
    }
    
    private void OnLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Instantiate(obj.Result, Vector3.zero, Quaternion.identity);
            Debug.Log("Instantiate Done");
        }
    }
    
    public void JustGiveoutSomeTreasures(string id, int count)
    {
        HItemShowPanel panel = new HItemShowPanel();
        string itemId = id;
        var thisItem = yPlanningTable.Instance.worldItems[itemId];
            
        string itemName = thisItem.chineseName;
        int itemNum = count;
        HItemCounter.Instance.AddItem(itemId, itemNum);
        HOpenWorldTreasure.ItemToShow itemToShow = new HOpenWorldTreasure.ItemToShow(itemName, itemNum, thisItem.UIIconLink, thisItem.isExpensive, thisItem.description);
        
        YGameRoot.Instance.Push(panel);
        panel.SetLeftScrollPanelActive(true);
        panel.SetMiddlePanelActive(false);
        var op2 = Addressables.LoadAssetAsync<Sprite>(itemToShow.ItemImageLink);
        Sprite go = op2.WaitForCompletion();
        panel.ShowItemsLeftScroll(go, itemName, count);
        StartCoroutine(PopPanel(panel));
    }

    IEnumerator PopPanel(BasePanel panel)
    {
        yield return new WaitForSeconds(1.2f);    
        panel.Pop();
    }
}
