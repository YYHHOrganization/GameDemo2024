using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class HRadialBlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private RenderTargetIdentifier source;
    private HRadialBlurSettings blurSettings;
    private RenderTargetHandle blurTex;
    private int blurTexID;
    
    RTHandle m_CameraColorTarget;
    private int screenWidth;
    private int screenHeight;
    
    public void SetUp(RenderTargetIdentifier source)
    {
        this.source = source;
    }

    public void Init(ScriptableRenderer renderer)
    {
        source = renderer.cameraColorTarget;
        blurSettings = VolumeManager.instance.stack.GetComponent<HRadialBlurSettings>();
        //blurSettings = new HBlurCommonSettings();
        //在URP的pp之前,eg.bloom
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        
        if(blurSettings!=null && blurSettings.IsActive())
        {
            material = new Material(Shader.Find("HPostProcessing/HRadialBlur"));
        }
        // if(!material)
        //     Debug.LogError("No material found for HRadialBlurRenderPass");
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
    
    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(blurTexID);
        base.FrameCleanup(cmd);
    }
    
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(blurSettings==null || !blurSettings.IsActive())
        {
            return;
        }
        //从命令缓冲区池获取一个带标签的命令缓冲区，该标签名在帧调试器中可以见到
        CommandBuffer cmd = CommandBufferPool.Get("RadialBlur Post Process");
        //获取目标相机的描述信息，创建一个结构体，里面有render texture各中信息，比如尺寸，深度图精度等等
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        //设置深度缓冲区，0表示不需要深度缓冲区
        opaqueDesc.depthBufferBits = 0;

        int downSample = blurSettings.RTDownSampling.value;
        screenWidth = renderingData.cameraData.cameraTargetDescriptor.width;
        screenHeight = renderingData.cameraData.cameraTargetDescriptor.height;
        
        int rtW = screenWidth / downSample;
        int rtH = screenHeight / downSample;
        
        int iterations = blurSettings.blurIterations.value;
        float radius = blurSettings.blurRadius.value;
        float centerX = blurSettings.blurCenterX.value;
        float bufferRadius = blurSettings.bufferRadius.value;
        material.SetFloat("_BlurRadius", radius);
        material.SetFloat("_BlurCenterX", centerX);
        material.SetFloat("_BlurCenterY", blurSettings.blurCenterY.value);
        material.SetInt("_BlurIterations", iterations);
        material.SetFloat("_BufferRadius", bufferRadius);
        
        //申请临时图像
        int tmpID = Shader.PropertyToID("_Temp1");
        cmd.GetTemporaryRT(tmpID, rtW, rtH, 0, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);
        cmd.GetTemporaryRT(blurTex.id, opaqueDesc);
        
        cmd.Blit(source, tmpID);
        cmd.Blit(tmpID, blurTex.Identifier(), material);
        cmd.Blit(blurTex.Identifier(), source);
        context.ExecuteCommandBuffer(cmd);
        
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}
