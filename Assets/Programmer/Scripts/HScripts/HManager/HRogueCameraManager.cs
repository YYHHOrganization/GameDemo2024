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
        YTriggerEvents.OnEnterRoomType += ResetCameraShake;
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
        if (cinemachineBrain.ActiveVirtualCamera != null)
        {
            //if (debug) cineName = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.name + "[CinemachineShake.cs]";//deubug 相机名称 
            cinemachineVirtualCameraBase = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCameraBase>();//获取当前虚拟相机的基类
            cinemachineBasicMultiChannelPerlins = cinemachineVirtualCameraBase.GetComponentsInChildren<CinemachineBasicMultiChannelPerlin>();//获取该虚拟相机下的noise 设置
        }
        if (cinemachineBasicMultiChannelPerlins.Length > 0)
        { 
            foreach (var item in cinemachineBasicMultiChannelPerlins)
            {
                item.m_AmplitudeGain = intensity;
                item.m_FrequencyGain = frequency;
            }
            startingIntensity = intensity;
            shakeTimerTotal = time;
            shakeTimer = time;
        }
        
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            // float lerpValue = Mathf.Lerp(startingIntensity, 0f, shakeTimer / shakeTimerTotal); //线性方式慢慢减小
            // if (shakeTimer < 0)
            // {
            //     lerpValue = 0f; //这里会出现负数的情况，正常来说应该是为0，暂时用判断解决
            //     
            // }
            if (shakeTimer <= 0)
            {
                cinemachineBasicMultiChannelPerlins = cinemachineVirtualCameraBase.GetComponentsInChildren<CinemachineBasicMultiChannelPerlin>();//获取该虚拟相机下的noise 设置
                foreach (var item in cinemachineBasicMultiChannelPerlins)
                {
                     item.m_AmplitudeGain = 0;
                     item.m_FrequencyGain = 0;
                }
            }
                

            // foreach (var item in cinemachineBasicMultiChannelPerlins)
            // {
            //     item.m_AmplitudeGain = lerpValue;
            // }
        }
    }
}
    
