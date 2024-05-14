using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogPass : ScriptableRenderPass
{
    private RenderTargetIdentifier source;
    private RenderTargetHandle tempTargetHandle;
    private Material distanceMaterial;
    private Material highMaterial;
    private int fogColorId = Shader.PropertyToID(FogShaderName.FogColor);
    private int fogIntensityId = Shader.PropertyToID(FogShaderName.FogIntensity);
    private int fogDistanceId = Shader.PropertyToID(FogShaderName.FogDistance);
    private int fogHighId = Shader.PropertyToID(FogShaderName.FogHigh);
    
    private int noiseTextureId = Shader.PropertyToID(FogShaderName.NoiseTexture);
    private int fogXSpeedId = Shader.PropertyToID(FogShaderName.FogXSpeed);
    private int fogYSpeedId = Shader.PropertyToID(FogShaderName.FogYSpeed);
    private int noiseAmountId = Shader.PropertyToID(FogShaderName.NoiseAmount);
    
    public FogPass()
    {
        tempTargetHandle.Init("tempfog");
    }

    public void Setup(RenderTargetIdentifier source,Material distanceMaterial,Material highMaterial)
    {
        this.source = source;
        this.distanceMaterial = distanceMaterial;
        this.highMaterial = highMaterial;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        FogVolme fogVolme = VolumeManager.instance.stack.GetComponent<FogVolme>();
        if (fogVolme.IsActive())
        {
            CommandBuffer cmd = CommandBufferPool.Get("FogCmd");
            var dec = renderingData.cameraData.cameraTargetDescriptor;
            dec.msaaSamples = 1;
            dec.depthBufferBits = 0;
            cmd.GetTemporaryRT(tempTargetHandle.id,dec);
            if (fogVolme.fogMode == FogMode.distance)
            {
                distanceMaterial.SetColor(fogColorId,fogVolme.fogColor.value);
                distanceMaterial.SetFloat(fogIntensityId,fogVolme.intensity.value);
                distanceMaterial.SetFloat(fogDistanceId,fogVolme.distance.value);
                //噪声
                distanceMaterial.SetTexture(noiseTextureId,fogVolme.noiseTexture.value);
                distanceMaterial.SetFloat(fogXSpeedId,fogVolme.fogXSpeed.value);
                distanceMaterial.SetFloat(fogYSpeedId,fogVolme.fogYSpeed.value);
                distanceMaterial.SetFloat(noiseAmountId,fogVolme.noiseAmount.value);
                
                cmd.Blit(source,tempTargetHandle.Identifier(),distanceMaterial);
            }
            else if (fogVolme.fogMode == FogMode.high)
            {
                highMaterial.SetColor(fogColorId,fogVolme.fogColor.value);
                highMaterial.SetFloat(fogIntensityId,fogVolme.intensity.value);
                highMaterial.SetFloat(fogHighId,fogVolme.distance.value);
                //噪声
                highMaterial.SetTexture(noiseTextureId,fogVolme.noiseTexture.value);
                highMaterial.SetFloat(fogXSpeedId,fogVolme.fogXSpeed.value);
                highMaterial.SetFloat(fogYSpeedId,fogVolme.fogYSpeed.value);
                highMaterial.SetFloat(noiseAmountId,fogVolme.noiseAmount.value);
                
                cmd.Blit(source,tempTargetHandle.Identifier(),highMaterial);
            }
            cmd.Blit(tempTargetHandle.Identifier(),source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            cmd.ReleaseTemporaryRT(tempTargetHandle.id);
        }
    }
}
