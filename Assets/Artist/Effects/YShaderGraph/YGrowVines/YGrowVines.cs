using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//做生长藤蔓的效果
public class YGrowVines : MonoBehaviour
{
    [SerializeField]List<MeshRenderer> GrowVinesMeshRenderers;
    [SerializeField]float time2Grow = 5.0f;
    [SerializeField]float refreshRate = 0.05f;
    [Range(0,1),SerializeField]float minGrow = 0.02f;
    [Range(0,1),SerializeField]float maxGrow = 0.98f;
    
    private List<Material> _materials = new List<Material>();
    private bool isFullyGrow = false;
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<GrowVinesMeshRenderers.Count;i++)
        {
            for(int j=0;j<GrowVinesMeshRenderers[i].materials.Length;j++)
            {
                if(GrowVinesMeshRenderers[i].materials[j].HasProperty("_Grow"))//_Grow是在Shader中定义的
                {
                    _materials.Add(GrowVinesMeshRenderers[i].materials[j]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //测试用
        if (Input.GetKeyDown(KeyCode.G))
        {
            BeginGrowVines();
        }
    }
    void BeginGrowVines()
    {
        for(int i=0;i<_materials.Count;i++)
        {
            StartCoroutine(GrowVines(_materials[i]));
        }
    }

    IEnumerator GrowVines(Material material)
    {
        float growValue = material.GetFloat("_Grow");
        if (!isFullyGrow)//生长
        {
            while(growValue< maxGrow)
            {
                growValue += refreshRate / time2Grow;//例子 refreshRate = 0.05f, time2Grow = 5.0f 时，每次增加0.01
                material.SetFloat("_Grow", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else //凋零 缩小
        {
            while (growValue > minGrow)
            {
                growValue -= refreshRate / time2Grow;//例子 refreshRate = 0.05f, time2Grow = 5.0f 时，每次增加0.01
                material.SetFloat("_Grow", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
            
        }
        if(growValue>=maxGrow)isFullyGrow = true;
        else isFullyGrow = false;
    }
}
