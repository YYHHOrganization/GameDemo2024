using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LittleMapRenderFeature : ScriptableRendererFeature
{
    public static LittleMapRenderFeature Instance;
    [System.Serializable]
    public class LittleMapSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        public int renderQueue = 3000;
    }
    //这个renderFeature会开启一个key：LightLittleMap，用来控制是否渲染小地图
    public LittleMapSettings settings = new LittleMapSettings();
    private LittleMapRenderPass renderPass;
    public class LittleMapRenderPass : ScriptableRenderPass
    {
        public LittleMapRenderPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Render LittleMap");
            cmd.EnableShaderKeyword("_LightLittleMap");
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            DrawingSettings drawingSettings = CreateDrawingSettings(new ShaderTagId("LightLittleMap"), ref renderingData, SortingCriteria.CommonOpaque);
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }
    }
    public override void Create()
    {
        Instance = this;
        renderPass = new LittleMapRenderPass();
        renderPass.renderPassEvent = settings.renderPassEvent;
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(renderPass);
    }
}
