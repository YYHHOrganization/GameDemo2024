using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCinemachineShake : MonoBehaviour
{
    float shakeTimer;//抖动时间
    CinemachineVirtualCameraBase cinemachineVirtualCameraBase;//virtualcamera 的基类 
    CinemachineBasicMultiChannelPerlin[] cinemachineBasicMultiChannelPerlins;//这里是获取所有的rig来设置noise
    
    private void Awake()
    {
        //if (debug) cineName = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.name + "[CinemachineShake.cs]";//deubug 相机名称 
        cinemachineVirtualCameraBase = GetComponent<CinemachineVirtualCameraBase>();//获取当前虚拟相机的基类
        cinemachineBasicMultiChannelPerlins = cinemachineVirtualCameraBase.GetComponentsInChildren<CinemachineBasicMultiChannelPerlin>();//获取该虚拟相机下的noise 设置
    }

    public void ShakeCamera(float intensity, float time, float frequency = 0.1f)
    {
        if (cinemachineBasicMultiChannelPerlins.Length > 0)
        { 
            foreach (var item in cinemachineBasicMultiChannelPerlins)
            {
                item.m_AmplitudeGain = intensity;
                item.m_FrequencyGain = frequency;
            }
            shakeTimer = time;
        }
    }

    private void OnDisable()
    {
        shakeTimer = 0;
        foreach (var item in cinemachineBasicMultiChannelPerlins)
        {
            item.m_AmplitudeGain = 0;
            item.m_FrequencyGain = 0;
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
