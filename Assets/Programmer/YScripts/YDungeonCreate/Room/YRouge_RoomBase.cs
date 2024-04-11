using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRouge_RoomBase : MonoBehaviour
{
    public YRoomNode roomNode;
    protected RoomType roomType;
    protected RoomData roomData;

    private GameObject floor;
    //floor get set
    public GameObject Floor
     {
         get => floor;
         set => floor = value;
     }
    public List<GameObject> walls=new List<GameObject>();
    //walls get set
    // public List<GameObject> Walls
    // {
    //     get => walls;
    //     set => walls = value;
    // }
    
    //如果门要下降 y-》localpos -10
    public List<GameObject> doors=new List<GameObject>();
    //doors get set
    // public List<GameObject> Doors
    // {
    //     get => doors;
    //     set => doors = value;
    // }
    // Start is called before the first frame update
    
    BoxCollider boxCollider = null;
    public void Start()
    {
        SetAllDoorsPosition();

        GenerateIcon();
        
        //在房间中心生成一个trigger，当玩家进入这个trigger的时候，房间的逻辑就会被激活
        // GameObject trigger = new GameObject();
        // trigger.transform.parent = this.transform;
        // trigger.transform.localPosition = new Vector3(0, 0, 0);
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(10, 10, 10);
        boxCollider.center = new Vector3(0, 0, 0);
        
        gameObject.layer = LayerMask.NameToLayer("IgnoreBullet");
        
    }

    private void GenerateIcon()
    {
        //YmapBattleRoomIcon
        string Link = "YmapIcon_"+roomType.ToString();
        var op = Addressables.LoadAssetAsync<GameObject>(Link);
        GameObject go = op.WaitForCompletion() as GameObject;
        if (go == null)
        {
            return;
        }
        GameObject iconGo = Instantiate(go, transform);
        iconGo.transform.parent = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入房间！！");
            //玩家进入房间，房间的逻辑就会被激活
            SetResultOn();
        }
    }

    public virtual void SetResultOn()
    {
        // SetAllDoorsUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //如果门要下降 y-》localpos -10
    //设置所有门的初始位置
    public void SetAllDoorsPosition()
    {
        foreach (var door in doors)
        {
            door.transform.localPosition = new Vector3(door.transform.localPosition.x, -10, door.transform.localPosition.z);
        }
    }
    //让门上升
    public void SetAllDoorsUp()
    {
        foreach (var door in doors)
        {
            door.transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBounce);
        }
    }
    //让门下降
    public void SetAllDoorsDown()
    {
        foreach (var door in doors)
        {
            door.transform.DOLocalMoveY(-10, 1f).SetEase(Ease.OutBounce);
        }
    }
    
    protected void ReadRoomItem()
    {
        //读取房间的物品信息
        roomData = YRogue_RoomAndItemManager.Instance.GetRoomDataByRoomType(roomType);
        Debug.Log(roomData.roomType);
        Debug.Log(roomData.roomID);
        Debug.Log(roomData.roomItemDataList[0].FixedItemID);
        Debug.Log(roomData.roomItemDataList[0].FixedItemCount);
    }
    protected void GenerateRoomItem()
    {
        //生成房间的物品
        //根据房间的物品信息roomItemDataList生成物品
        //生成物品的位置是固定的
        foreach (var roomItemData in roomData.roomItemDataList)
        {
            //生成物品
            //根据roomItemData.FixedItemID生成物品
            //生成物品的数量是roomItemData.FixedItemCount
            Debug.Log("生成物品");
            string itemAddLink  = YRogue_RoomAndItemManager.Instance.GetRoomItemLink(roomItemData.FixedItemID);
            if(itemAddLink!=null)
            {
                var op = Addressables.LoadAssetAsync<GameObject>(itemAddLink);
                GameObject go = op.WaitForCompletion() as GameObject;
                //instantiation
                for(int i=0;i<roomItemData.FixedItemCount;i++)
                {
                    GameObject item = Instantiate(go, transform);
                    item.transform.localPosition = new Vector3(0, 0.5f, 0);
                }
                //GameObject item = Instantiate(go, transform);
             
                //item.transform.localPosition = new Vector3(0, 0.5f, 0);
                
                //先让他随机放在场景中的任何位置，但是必须在房间的范围内
                // item.transform.localPosition = new Vector3(Random.Range(roomNode.BottomLeftAreaCorner.x, roomNode.TopRightAreaCorner.x), 0,
                //     Random.Range(roomNode.BottomLeftAreaCorner.y, roomNode.TopRightAreaCorner.y));
            }
        }
        
        
        
    }
}
