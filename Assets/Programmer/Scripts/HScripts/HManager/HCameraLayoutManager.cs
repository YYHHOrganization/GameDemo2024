using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class HCameraLayoutManager : MonoBehaviour
{
    //单例模式
    private static HCameraLayoutManager _instance;
    public Camera playerCamera;
    public Camera puppetCamera;
    

    private void SetPlayerOutputViewportRect(float x, float y, float width, float height)
    {
        playerCamera.rect = new Rect(x, y, width, height);
    }
    
    private void SetPuppetOutputViewportRect(float x, float y, float width, float height)
    {
        puppetCamera.rect = new Rect(x, y, width, height);
    }

    public void SetPlayerCameraWholeScreen()
    {
        playerCamera.gameObject.SetActive(true);
        SetPlayerOutputViewportRect(0,0,1,1);
        puppetCamera.gameObject.SetActive(false);
    }

    public void SetTwoCamerasEachHalf()
    {
        playerCamera.gameObject.SetActive(true);
        puppetCamera.gameObject.SetActive(true);
        SetPlayerOutputViewportRect(0,0,0.5f,1);
        SetPuppetOutputViewportRect(0.5f,0,0.5f,1);
    }

    public void SetPuppetCameraLittle()
    {
        playerCamera.gameObject.SetActive(true);
        puppetCamera.gameObject.SetActive(true);
        
        SetPlayerOutputViewportRect(0,0,1,1);
        SetPuppetOutputViewportRect(0.7f,0.7f,0.3f,0.3f);
        
        var camera = puppetCamera.gameObject.GetComponent<Camera>();
        camera.depth = 0.2f;
    }

    private void Awake()
    {
        _instance = this.gameObject.GetComponent<HCameraLayoutManager>();
        //SetPlayerCameraWholeScreen();
        //SetPuppetCameraLittle();
        // SetTwoCamerasEachHalf();
    }

    public static HCameraLayoutManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HCameraLayoutManager>();
            }
            return _instance;
        }
    }
    
    
    
    
}
