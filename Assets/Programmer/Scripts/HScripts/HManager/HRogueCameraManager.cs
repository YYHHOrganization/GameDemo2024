using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class HRogueCameraManager : MonoBehaviour
{
    //单例模式
    private GameObject currentActiveCamera;
    private static HRogueCameraManager instance;
    public static HRogueCameraManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HRogueCameraManager>();
            }
            return instance;
        }
    }

    private CinemachineBrain cinemachineBrain;
    
    public void SetCurrentActiveCamera(GameObject curCamera)
    {
        currentActiveCamera = curCamera;
        cinemachineBrain = curCamera.GetComponent<CinemachineBrain>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    float shakeTimer;//抖动时间
    float shakeTimerTotal;//抖动总时间
    float startingIntensity;//抖动强度
    
    CinemachineVirtualCameraBase cinemachineVirtualCameraBase;//virtualcamera 的基类 
    CinemachineBasicMultiChannelPerlin[] cinemachineBasicMultiChannelPerlins;//这里是获取所有的rig来设置noise

    private void Start()
    {
        //YTriggerEvents.OnEnterRoomType += ResetCameraShake;
    }

    private void ResetCameraShake(object sender,YTriggerEnterRoomTypeEventArgs e)
    {
        if (!cinemachineVirtualCameraBase) return;
        cinemachineBasicMultiChannelPerlins = cinemachineVirtualCameraBase.GetComponentsInChildren<CinemachineBasicMultiChannelPerlin>();//获取该虚拟相机下的noise 设置
        if (cinemachineBasicMultiChannelPerlins.Length > 0)
        { 
            foreach (var item in cinemachineBasicMultiChannelPerlins)
            {
                item.m_AmplitudeGain = 0;
                item.m_FrequencyGain = 0;
            }
        }
    }
    
    

    public void ShakeCamera(float intensity, float time, float frequency = 0.1f)
    {
        GameObject cinemachine = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject;
        if (cinemachine)
        {
            VirtualCinemachineShake cinemachineShake = cinemachine.GetComponent<VirtualCinemachineShake>();
            if (cinemachineShake)
            {
                cinemachineShake.ShakeCamera(intensity, time, frequency);
            }
        }
    }

    private void Update()
    {
       
    }
}
    
