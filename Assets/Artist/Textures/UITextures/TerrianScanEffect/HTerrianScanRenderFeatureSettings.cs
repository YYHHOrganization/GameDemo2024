using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("TerrianScanSettings")]
public class HTerrianScanRenderFeatureSettings: VolumeComponent, IPostProcessComponent
{
    [Tooltip("Scan Depth")]
    // public ClampedFloatParameter scanDepth = new ClampedFloatParameter(0f, 0.0f, 1.0f);
    public ClampedFloatParameter scanDepth = new ClampedFloatParameter(0f, 0.0f, 1.0f);

    [Tooltip("Scan Width")]
    public ClampedFloatParameter scanWidth = new ClampedFloatParameter(30f, 1f, 100f);
    
    [Tooltip("Scan Color")]
    // 传输hdr的颜色：
    public ColorParameter scanColor = new ColorParameter(Color.yellow, true, false, true);
    
    //public ColorParameter scanColor = new ColorParameter(Color.yellow);
    public bool IsActive()
    {
        return (scanDepth.value >= 0.0f) && (scanWidth.value >= 0.0f);
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}
