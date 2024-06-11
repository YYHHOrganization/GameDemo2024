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
    string LockedShaderAddress="YLockEff";
    
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
    //pet走这个
    GameObject PetLockedShader;
    string LockedShaderPetAddress2="YLockEffPet";
    public void SetPetLockedShaderOnOrOff(bool isOn,Transform showPlayerTransform)
    {
        if (isOn)
        {
            if (PetLockedShader == null)
            {
                GameObject go =Addressables.LoadAssetAsync<GameObject>(LockedShaderPetAddress2).WaitForCompletion();
                PetLockedShader = Instantiate(go);
                //将这个shader放到角色的位置，但是并不用作为子物体
                PetLockedShader.transform.position = showPlayerTransform.position;
                PetLockedShader.transform.rotation = showPlayerTransform.rotation;
                PetLockedShader.SetActive(true);
            }
            else
            {
                PetLockedShader.SetActive(true);
            }
        }
        else
        {
            if (PetLockedShader != null)
            {
                PetLockedShader.SetActive(false);
            }
        }
    }
}
