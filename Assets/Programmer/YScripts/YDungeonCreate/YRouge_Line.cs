using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class YRouge_Line
{
    Orientation orientation;
    public Orientation Orientation
    {
        get => orientation;
        set => orientation = value;
    }
    
    Vector2Int coordinate;//坐标
    public Vector2Int Coordinate
    {
        get => coordinate;
        set => coordinate = value;
    }

    
    public YRouge_Line(Orientation orientation, Vector2Int coordinate)
    {
        this.orientation = orientation;
        this.coordinate = coordinate;
    }
}
public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}
