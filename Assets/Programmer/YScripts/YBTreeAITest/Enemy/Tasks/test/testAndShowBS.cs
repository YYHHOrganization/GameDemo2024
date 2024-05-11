using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAndShowBS : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {

        //输出这个blend shape的所有名称与对应的index：先index 写入文件txt中
        string path = Application.dataPath + "/Programmer/YScripts/YBTreeAITest/Enemy/BlendShapeIndex" + skinnedMeshRenderer.sharedMesh.name + ".txt";
        string content = "";
        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            content += i + " " + skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i) + "\n";
        }
        System.IO.File.WriteAllText(path, content);
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
