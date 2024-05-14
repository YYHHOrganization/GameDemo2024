using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogFeature : ScriptableRendererFeature
{
    private FogPass fogPass;
    private YFogPassDistance yFogPassDistance;
    public Material distanceMaterial;
    public Material highMaterial;
    public override void Create()
    {
        fogPass = new FogPass();
        yFogPassDistance = new YFogPassDistance();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(fogPass);
        renderer.EnqueuePass(yFogPassDistance);
        
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        fogPass.Setup(renderer.cameraColorTarget,distanceMaterial,highMaterial);
        yFogPassDistance.Setup(renderer.cameraColorTarget,distanceMaterial,highMaterial);
    }
}
