using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InstancedIndirectGrassPosDefine : MonoBehaviour
{
    [Range(1, 40000000)]
    public int instanceCount = 1000000;//草地实例的数量
    public float drawDistance = 125;//草地实例的绘制距离

    private int cacheCount = -1;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePosIfNeeded();
    }
    private void Update()
    {
        UpdatePosIfNeeded();
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(300, 50, 200, 30), "Instance Count: " + instanceCount / 1000000 + "Million");
        instanceCount = Mathf.Max(1, (int)(GUI.HorizontalSlider(new Rect(300, 100, 200, 30), instanceCount / 1000000f, 1, 10)) * 1000000);

        GUI.Label(new Rect(300, 150, 200, 30), "Draw Distance: " + drawDistance);
        drawDistance = Mathf.Max(1, (int)(GUI.HorizontalSlider(new Rect(300, 200, 200, 30), drawDistance / 25f, 1, 8)) * 25);
        InstancedIndirectGrassRenderer.instance.drawDistance = drawDistance;
    }
    private void UpdatePosIfNeeded()
    {
        //首先检查instanceCount是否发生了变化，如果没有变化则直接返回。
        if (instanceCount == cacheCount)
            return;

        Debug.Log("UpdatePos (Slow)");

        // 如果有变化，那么就会生成新的草地实例的位置，并将这些位置发送给InstancedIndirectGrassRenderer类的实例。 
        //这里只在这一块内进行渲染
        //same seed to keep grass visual the same
        UnityEngine.Random.InitState(123);

        //auto keep density the same
        float scale = Mathf.Sqrt((instanceCount / 4)) / 2f;
        transform.localScale = new Vector3(scale, transform.localScale.y, scale);

        //////////////////////////////////////////////////////////////////////////
        //can define any posWS in this section, random is just an example
        //////////////////////////////////////////////////////////////////////////
        List<Vector3> positions = new List<Vector3>(instanceCount);
        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 pos = Vector3.zero;

            pos.x = UnityEngine.Random.Range(-1f, 1f) * transform.lossyScale.x;
            pos.z = UnityEngine.Random.Range(-1f, 1f) * transform.lossyScale.z;

            //transform to posWS in C#
            pos += transform.position;

            positions.Add(new Vector3(pos.x, pos.y, pos.z));
        }

        //send all posWS to renderer
        InstancedIndirectGrassRenderer.instance.allGrassPos = positions;
        cacheCount = positions.Count;
    }

}
