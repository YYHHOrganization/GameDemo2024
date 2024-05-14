

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogVolme : VolumeComponent,IPostProcessComponent
{
    public FloatParameter intensity = new FloatParameter(0);
    public ColorParameter fogColor = new ColorParameter(Color.white);
    public FloatParameter distance = new FloatParameter(0);
    public FogModeParameter fogMode = new FogModeParameter(FogMode.off);
    //使用噪声
    //
    //
    // public Texture noiseTexture;
    public TextureParameter noiseTexture = new TextureParameter(null);
    // [Range(-0.5f,0.5f)]
    // public float fogXSpeed = 0.1f;//雾在x轴上的移动速度
    public FloatParameter fogXSpeed = new FloatParameter(0.1f);
    // [Range(-0.5f,0.5f)]
    // public float fogYSpeed = 0.1f;//雾在y轴上的移动速度
    public FloatParameter fogYSpeed = new FloatParameter(0.1f);
    // [Range(0.0f,3.0f)]
    // public float noiseAmount = 1.0f;//雾的噪声强度
    public FloatParameter noiseAmount = new FloatParameter(1.0f);
    
    
    public bool IsActive()
    {
        return intensity.value > 0 && fogMode != FogMode.off;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}

public enum FogMode
{
    off,
    distance,
    high
}
[Serializable]
public sealed class FogModeParameter : VolumeParameter<FogMode>
{
    public FogModeParameter(FogMode value, bool overriderState = false) : base(value, overriderState)
    {
        
    }
}