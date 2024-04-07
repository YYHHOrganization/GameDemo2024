using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YDungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;//迭代次数
    public int coridorWidth;//走廊宽度
    public Material floorMaterial;
    [Range(0.0f,0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f,1.0f)]
    public float roomTopCornerModifier;
    [Range(0,2)]
    public int offset;
    
    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        YDungeonGenerator generator = new YDungeonGenerator(dungeonWidth, dungeonLength);
        var ListOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin,
            roomBottomCornerModifier, roomTopCornerModifier, offset,
            coridorWidth);
        foreach (var room in ListOfRooms)
        {
            createMeshes(room.BottomLeftAreaCorner, room.TopRightAreaCorner);
        }
    }


    private void createMeshes(Vector2 bottomLeftCorner,Vector2 topRightCorner)
    {
        Vector3 bottomLeft = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRight = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topRight = new Vector3(topRightCorner.x, 0, topRightCorner.y);
        Vector3 topLeft = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        
        //以下表示一个矩形的四个顶点
        Vector3[] vertices = new Vector3[] {topLeft, topRight, bottomLeft, bottomRight};
        Vector2[] uvs = new Vector2[vertices.Length];//纹理坐标
        //以下表示一个矩形的四个顶点的纹理坐标
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        int[] triangles = new int[] {0, 1, 2, 2, 1, 3};//三角形顶点索引,这里表示两个三角形,一个是012,另一个是213,这样就构成了一个矩形
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        //it means that the mesh will be rendered with the material floorMaterial
        GameObject floor = new GameObject("Mesh"+bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        
        floor.transform.position = new Vector3(0, 0, 0);
        floor.transform.localScale = new Vector3(1, 1, 1);
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material = floorMaterial;
        
        
    }
}
