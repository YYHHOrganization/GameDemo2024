using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

//单例
public class YEffManager : YSingleTemplate<YEffManager>
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    GameObject LockedShader;
    public string LockedShaderAddress="YLockEff";
    public void SetLockedShaderOnOrOff(bool isOn,Transform showPlayerTransform)
    {
        if (isOn)
        {
            if (LockedShader == null)
            {
                // GameObject RogueLittleMapCamera = Addressables.LoadAssetAsync<GameObject>("YLittleMapCamera").WaitForCompletion();
                // GameObject LittleMapCamera = Instantiate(RogueLittleMapCamera);
                
                GameObject go =Addressables.LoadAssetAsync<GameObject>(LockedShaderAddress).WaitForCompletion();
                LockedShader = Instantiate(go);
                //将这个shader放到角色的位置，但是并不用作为子物体
                LockedShader.transform.position = showPlayerTransform.position;
                LockedShader.transform.rotation = showPlayerTransform.rotation;
                LockedShader.SetActive(true);
            }
            else
            {
                LockedShader.SetActive(true);
            }
        }
        else
        {
            if (LockedShader != null)
            {
                LockedShader.SetActive(false);
            }
        }
        
        
    }
}
