using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class H_GaussianBlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private RenderTargetIdentifier source;
    private H_GaussianBlurSettings blurSettings;
    private RenderTargetHandle blurTex;
    private int blurTexID;

    
    public void SetUp(RenderTargetIdentifier source)
    {
        this.source = source;
    }

    public bool Setup(ScriptableRenderer renderer)
    {
        source = renderer.cameraColorTarget;
        blurSettings = VolumeManager.instance.stack.GetComponent<H_GaussianBlurSettings>();
        //在URP的pp之前,eg.bloom
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        
        if(blurSettings!=null && blurSettings.IsActive())
        {
            material = new Material(Shader.Find("HPostProcessing/HGaussianBlur"));
            return true;
        }
        // if(!material)
        //     Debug.LogError("No material found for HGaussianBlurRenderPass");
        return false;
    }
    
    
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        if(blurSettings==null || !blurSettings.IsActive())
        {
            return;
        }
        blurTexID = Shader.PropertyToID("_BlurTex");
        blurTex = new RenderTargetHandle();
        blurTex.id = blurTexID;
        cmd.GetTemporaryRT(blurTex.id, cameraTextureDescriptor);
        base.Configure(cmd, cameraTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(blurSettings==null || !blurSettings.IsActive())
        {
            return;
        }
        //从命令缓冲区池获取一个带标签的命令缓冲区，该标签名在帧调试器中可以见到
        CommandBuffer cmd = CommandBufferPool.Get("GaussianBlur Post Process");
        //获取目标相机的描述信息，创建一个结构体，里面有render texture各中信息，比如尺寸，深度图精度等等
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        //设置深度缓冲区，0表示不需要深度缓冲区
        opaqueDesc.depthBufferBits = 0;
        
        int gridSize = Mathf.CeilToInt(blurSettings.strength.value * 6.0f);
        if (gridSize % 2 == 0)
        {
            gridSize++;
        }
        material.SetInteger("_GridSize", gridSize);
        material.SetFloat("_Spread", blurSettings.strength.value);
        
        //申请临时图像
        cmd.GetTemporaryRT(blurTex.id, opaqueDesc);
        
        cmd.Blit(source, blurTex.Identifier(), material, 0);
        cmd.Blit(blurTex.Identifier(), source, material, 1);
        context.ExecuteCommandBuffer(cmd);
        
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(blurTexID);
        base.FrameCleanup(cmd);
    }
}
