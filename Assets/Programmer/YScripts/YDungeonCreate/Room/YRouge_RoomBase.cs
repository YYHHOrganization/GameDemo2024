using System;
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
    
    //房间的初始space
    roomSpaceKeep roomSpaceKeep;
    protected GameObject roomLittleMapMask;
    public void SetRoomNodeSpace(roomSpaceKeep roomSpaceKeep)
    {
        this.roomSpaceKeep = roomSpaceKeep;
    }
    
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
    
    // public List<GameObject> doors=new List<GameObject>();
    public List<GameObject> horizontaldoors=new List<GameObject>();
    public List<GameObject> vertiacaldoors=new List<GameObject>();
    
    //doors get set
    // public List<GameObject> Doors
    // {
    //     get => doors;
    //     set => doors = value;
    // }
    // Start is called before the first frame update
    
    BoxCollider boxCollider = null;
    int roomWidth;
    int roomLength;
    
    protected List<GameObject> enemies = new List<GameObject>();

    public List<GameObject> Enemies
    {
        get { return enemies; }
    }
    public void Start()
    {
        SetAllDoorsPosition();

        GenerateIcon();
        
        roomWidth = roomNode.TopRightAreaCorner.x - roomNode.BottomLeftAreaCorner.x;
        roomLength = roomNode.TopRightAreaCorner.y - roomNode.BottomLeftAreaCorner.y;
        //在房间中心生成一个trigger，当玩家进入这个trigger的时候，房间的逻辑就会被激活
        // GameObject trigger = new GameObject();
        // trigger.transform.parent = this.transform;
        // trigger.transform.localPosition = new Vector3(0, 0, 0);
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        // boxCollider.size = new Vector3(10, 10, 10);
        boxCollider.size = new Vector3(roomWidth-0.5f, 10, roomLength-0.5f);
        boxCollider.center = new Vector3(0, 0, 0);
        
        gameObject.layer = LayerMask.NameToLayer("IgnoreBullet");
        
        //生成一个黑色mask mask在地图上 大小根据roomSpaceKeep的大小来确定
        GenerateLittleMapMask();
        
    }

    private void GenerateLittleMapMask()
    {
        //生成一个黑色mask mask在地图上 大小根据roomSpaceKeep的大小来确定
        string AddressLink = "YPlaneMask";
        var op = Addressables.InstantiateAsync(AddressLink);
        roomLittleMapMask  = op.WaitForCompletion() as GameObject;
        roomLittleMapMask.transform.position = new Vector3
        (roomSpaceKeep.bottomLeft.x + roomSpaceKeep.width / 2,
            150,
            roomSpaceKeep.bottomLeft.y + roomSpaceKeep.length / 2) + YRogueDungeonManager.Instance.RogueDungeonOriginPos;
        
        roomLittleMapMask.transform.parent = transform;
        
        // // mask.AddComponent<MeshFilter>();
        // var meshFilter = mask.AddComponent<MeshFilter>();
        // //plane
        // meshFilter.mesh = GameObject.CreatePrimitive(PrimitiveType.Plane).GetComponent<MeshFilter>().mesh; 
        // var meshRenderer = mask.AddComponent<MeshRenderer>();
        // meshRenderer.material = new Material(Shader.Find("Unlit/Color"));
        // meshRenderer.material.color = new Color(250, 250, 200);
        
        roomLittleMapMask.transform.localScale = new Vector3(roomSpaceKeep.width/10.0f+0.05f, 1, roomSpaceKeep.length/10.0f+0.05f);
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
        if (YRogueDungeonManager.Instance.flagGameBegin == false) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入房间！！");
            //玩家进入房间，房间的逻辑就会被激活
            SetResultOn();
            SetMaskOff();//后面这个要改为进入房间而不是激活房间的时候显示
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetResultOff();
        }
    }

    private void SetMaskOff()
    {
        Destroy(roomLittleMapMask);
    }

    public virtual void SetResultOn()
    {
        // SetAllDoorsUp();
        YRogue_RoomAndItemManager.Instance.SetCurRoom(gameObject);
    }
    
    public virtual void SetResultOff()
    {
        // SetAllDoorsUp();
    }

    
    
    //如果门要下降 y-》localpos -10
    //设置所有门的初始位置
    public void SetAllDoorsPosition()
    {
        foreach (var door in horizontaldoors)
        {
            door.transform.localPosition = new Vector3(door.transform.localPosition.x, -11, door.transform.localPosition.z);
        }
        foreach (var door in vertiacaldoors)
        {
            door.transform.localPosition = new Vector3(door.transform.localPosition.x, -11, door.transform.localPosition.z);
        }
    }
    //让门上升
    public void SetAllDoorsUp()
    {
        foreach (var door in horizontaldoors)
        {
            door.transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBounce);
        }
        foreach (var door in vertiacaldoors)
        {
            door.transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBounce);
        }
    }
    //让门下降
    public void SetAllDoorsDown()
    {
        foreach (var door in horizontaldoors)
        {
            door.transform.DOLocalMoveY(-11, 1f).SetEase(Ease.OutBounce);
        }
        foreach (var door in vertiacaldoors)
        {
            door.transform.DOLocalMoveY(-11, 1f).SetEase(Ease.OutBounce);
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
