using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YNameUI : MonoBehaviour
{
    // public GameObject trans;
    public Transform ShowPlaceTrans;
    public GameObject camera;
    public string name;
    TMP_Text nameUI;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        
        name = "测试";
    }

    // Update is called once per frame
    // void Update()
    // {
    //     this.transform.position = new Vector3(trans.transform.position.x, trans.transform.position.y+1f,
    //         trans.transform.position.z);
    //     
    //     this.transform.rotation = camera.transform.rotation;
    // }
    
    //赋值
    public void SetAttribute(string name,Transform ShowPlaceTrans,GameObject camera)
    {
        this.ShowPlaceTrans = ShowPlaceTrans;
        this.camera = camera;
        this.name = name;
        // this.transform.position = new Vector3(ShowPlaceTrans.position.x, ShowPlaceTrans.position.y,
        //     ShowPlaceTrans.position.z);//如果这个目的地会移动的话就要放在Update里面
        this.transform.position = ShowPlaceTrans.position;
        //改名 寻找自己下面的节点name
        nameUI = this.transform.Find("Name").GetComponent<TMP_Text>();//这句话的意思是找到自己下面的节点name
        nameUI.text = name;
    }   
   
    public void SetAttribute(string name,Transform ShowPlaceTrans,GameObject camera,float bias)
    {
        this.ShowPlaceTrans = ShowPlaceTrans;
        this.camera = camera;
        this.name = name;
        this.transform.position = new Vector3(ShowPlaceTrans.position.x, ShowPlaceTrans.position.y+bias,
            ShowPlaceTrans.position.z);//如果这个目的地会移动的话就要放在Update里面
        //改名 寻找自己下面的节点name
        nameUI = this.transform.Find("Name").GetComponent<TMP_Text>();//这句话的意思是找到自己下面的节点name
        nameUI.text = name;
    }   

    public void OnEnterShowName()
    {
        Debug.Log("OnEnterShowName");
        
        
    }
    private void LateUpdate()
    {
        
        this.transform.rotation = camera.transform.rotation;
        
        
        
        // this.transform.LookAt(ShowPlaceTrans.position + camera.transform.rotation * Vector3.forward,camera.transform.rotation * Vector3.up);
        // this.transform.LookAt(ShowPlaceTrans.position + camera.transform.rotation * Vector3.forward,camera.transform.rotation * Vector3.up);
    }
}
