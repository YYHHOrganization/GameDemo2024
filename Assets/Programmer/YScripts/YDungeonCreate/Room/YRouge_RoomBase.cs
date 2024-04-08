using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_RoomBase : MonoBehaviour
{
    public YRoomNode roomNode;

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
    
    public List<GameObject> doors=new List<GameObject>();
    //doors get set
    // public List<GameObject> Doors
    // {
    //     get => doors;
    //     set => doors = value;
    // }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
