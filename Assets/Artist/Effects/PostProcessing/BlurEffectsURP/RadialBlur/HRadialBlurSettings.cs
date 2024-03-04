using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("CommonBlurSettings")]
public class HRadialBlurSettings : VolumeComponent, IPostProcessComponent
{ 
    [Tooltip("Blur radius in common settings")]
    public ClampedFloatParameter blurRadius = new ClampedFloatParameter(0.0f, 0.0f, 15f);
    [Tooltip("Blur iterations in common settings")]
    public IntParameter blurIterations = new IntParameter(1);

    [Tooltip("Blur Center Point")]
    public ClampedFloatParameter blurCenterX = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
    public ClampedFloatParameter blurCenterY = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
    
    [Range(0f, 10f), Tooltip("降采样次数")]
    public IntParameter RTDownSampling = new ClampedIntParameter(1, 1, 10); 
    
    [Range(0f, 10f), Tooltip("模糊半径")]
    public FloatParameter bufferRadius = new ClampedFloatParameter(1.0f, 0.0f, 5.0f); 
    public bool IsActive()
    {
        return (blurRadius.value > 0.0f) && active;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}
