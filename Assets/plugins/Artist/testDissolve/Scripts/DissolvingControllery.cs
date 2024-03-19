using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingControllery : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedeMesh;

    // private Material[] skinnedMaterials;
    public  Material[] skinnedMaterials;

    public float dissolveRate = 0.0125f;

    public float refreshRate = 0.025f;
    
    
    private Renderer[] rendArray;
    private List<Material> materials = new List<Material>();
    // Start is called before the first frame update
    void Start()
    {
        // if (skinnedeMesh != null)
        // {
        //     skinnedMaterials = skinnedeMesh.materials;
        // }
        rendArray = gameObject.transform.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < rendArray.Length; i++)
        {
            Material[] mats = rendArray[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                materials.Add(mats[j]);
            }
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DissolveCo());
        }
    }

    IEnumerator DissolveCo()
    {
        // if (skinnedMaterials.Length > 0)
        // {
        //     float counter = 0;
        //     if (skinnedMaterials[0].GetFloat("_DissolveAmount") != null)
        //     {
        //         while (skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
        //         {
        //             counter += dissolveRate;
        //             for (int i = 0; i < skinnedMaterials.Length; i++)
        //             {
        //                 skinnedMaterials[i].SetFloat("_DissolveAmount",counter);
        //             }
        //
        //             yield return new WaitForSeconds(refreshRate);
        //         }
        //     }
        // }
        
        if (materials.Count > 0)
        {
            float counter = 0;
            if (materials[0].GetFloat("_DissolveAmount") != null)
            {
                while (materials[0].GetFloat("_DissolveAmount") < 1)
                {
                    counter += dissolveRate;
                    for (int i = 0; i < materials.Count; i++)
                    {
                        materials[i].SetFloat("_DissolveAmount",counter);
                    }
                    yield return new WaitForSeconds(refreshRate);
                }
            }
        }
    }
}
