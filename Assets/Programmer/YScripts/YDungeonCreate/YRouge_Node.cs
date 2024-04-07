using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public abstract class YRouge_Node
{
    private List<YRouge_Node> childrenNodeList;
    public List<YRouge_Node> ChildrenNodeList
    {
        get => childrenNodeList;
    }
    
    private YRouge_Node parentNode;
    
    public bool isVisited { get; set; }
    public Vector2Int BottomLeftAreaCorner { get; set; }//左下角
    public Vector2Int BottomRightAreaCorner { get; set; }//右下角
    public Vector2Int TopLeftAreaCorner { get; set; }//左上角
    public Vector2Int TopRightAreaCorner { get; set; }//右上角
    
    public int TreeLayerIndex { get; set; }
    
    public YRouge_Node(YRouge_Node parentNode)
    {
        this.parentNode = parentNode;
        childrenNodeList = new List<YRouge_Node>();
        if (parentNode != null)
        {
            parentNode.AddChildNode(this);
        }
    }
    public void AddChildNode(YRouge_Node childNode)
    {
        childrenNodeList.Add(childNode);
    }
    public void RemoveChildNode(YRouge_Node childNode)
    {
        childrenNodeList.Remove(childNode);
    }
}
