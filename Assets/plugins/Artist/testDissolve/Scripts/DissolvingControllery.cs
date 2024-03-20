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
    Renderer[] PropList = null;
    
    private List<Material> materials = new List<Material>();
    
    MaterialPropertyBlock prop = null;
    
    // Start is called before the first frame update
    void Start()
    {
        // if (skinnedeMesh != null)
        // {
        //     skinnedMaterials = skinnedeMesh.materials;
        // }
        // rendArray = gameObject.transform.GetComponentsInChildren<Renderer>(true);
        // for (int i = 0; i < rendArray.Length; i++)
        // {
        //     Material[] mats = rendArray[i].materials;
        //     for (int j = 0; j < mats.Length; j++)
        //     {
        //         materials.Add(mats[j]);
        //     }
        // }
        
        // rendArray = gameObject.transform.GetComponentsInChildren<Renderer>(true);
        // PropList = new Renderer[rendArray.Length];
        // for (int i = 0; i < rendArray.Length; i++)
        // {
        //     PropList[i] = rendArray[i];
        // }
        // prop = new MaterialPropertyBlock();
        //SetMaterialsProp(gameObject);
        
        
        
        SetMaterialsPropAndBeginDissolve(gameObject);
        
    }
    
    // public void SetMaterials(GameObject go)
    // {
    //     rendArray = go.transform.GetComponentsInChildren<Renderer>(true);
    //     for (int i = 0; i < rendArray.Length; i++)
    //     {
    //         Material[] mats = rendArray[i].materials;
    //         for (int j = 0; j < mats.Length; j++)
    //         {
    //             materials.Add(mats[j]);
    //         }
    //     }
    // }
    
    public void SetMaterialsPropAndBeginDissolve(GameObject go)
    {
        
        rendArray = go.transform.GetComponentsInChildren<Renderer>(true);
        PropList = new Renderer[rendArray.Length];
        for (int i = 0; i < rendArray.Length; i++)
        {
            PropList[i] = rendArray[i];
        }
        prop = new MaterialPropertyBlock();
        
        StartCoroutine(DissolveProp());
    }
    

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartCoroutine(DissolveCo());
        // }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     // prop.SetFloat("_DissolveAmount", 0.5f);
        //     // //Prop.GetComponentInChildren<Renderer>().SetPropertyBlock(prop);
        //     // for (int i = 0; i < PropList.Length; i++)
        //     // {
        //     //     PropList[i].SetPropertyBlock(prop);
        //     // }
        //     StartCoroutine(DissolveProp());
        // }
       
    }

    IEnumerator DissolveProp()
    {
        float counter = 0;
        if (prop.GetFloat("_DissolveAmount") != null)
        {
            while (prop.GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                prop.SetFloat("_DissolveAmount",counter);
                for (int i = 0; i < PropList.Length; i++)
                {
                    PropList[i].SetPropertyBlock(prop);
                }
                yield return new WaitForSeconds(refreshRate);
            }
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
