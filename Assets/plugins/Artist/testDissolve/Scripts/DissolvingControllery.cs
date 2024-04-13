using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingControllery : MonoBehaviour
{

    //单例
    //public static DissolvingControllery Instance;

    // private void Awake()
    // {
    //     Instance = this;
    // }

    public float dissolveRate = 0.0125f;

    public float refreshRate = 0.025f;
    // Start is called before the first frame update
    void Start()
    {
        
        //SetMaterialsPropAndBeginDissolve(gameObject);
        
    }

    
    public void SetMaterialsPropAndBeginDissolve(GameObject go)
    {
        
        Renderer[] PropList = null;
        
        MaterialPropertyBlock prop = null;
        
        PropList = go.GetComponentsInChildren<Renderer>(true);
        prop = new MaterialPropertyBlock();
        
        
        StartCoroutine(DissolveProp(prop, PropList,0));
    }
    public void SetMaterialsPropAndBeginDissolve(GameObject go,float delay)
    {
        
        Renderer[] PropList = null;
        
        MaterialPropertyBlock prop = null;
        
        PropList = go.GetComponentsInChildren<Renderer>(true);
        prop = new MaterialPropertyBlock();
        
        
        StartCoroutine(DissolveProp(prop, PropList,delay));
    }
    public void SetBeginAndEndAndMaterialsPropAndBeginDissolve(GameObject go,float delay,float begin,float end)
    {
        
        Renderer[] PropList = null;
        
        MaterialPropertyBlock prop = null;
        
        PropList = go.GetComponentsInChildren<Renderer>(true);
        prop = new MaterialPropertyBlock();
        
        
        StartCoroutine(DissolveProp(prop, PropList,delay,begin,end));
    }
    public IEnumerator DissolveProp(MaterialPropertyBlock prop , Renderer[] PropList ,float delay,float begin,float end)
    {
        prop.SetFloat("_DissolveAmount",begin);
        yield return new WaitForSeconds(delay);
        if(end<begin)
        {
            dissolveRate = -dissolveRate;
            float counter = begin;
        
            if (prop.GetFloat("_DissolveAmount") != null)
            {
                while (prop.GetFloat("_DissolveAmount") > end)//eg :1-0
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
        
    }

    IEnumerator DissolveProp(MaterialPropertyBlock prop , Renderer[] PropList ,float delay)
    {
        yield return new WaitForSeconds(delay);
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
        
        Destroy(gameObject, 10f);
        
    }
    
}
