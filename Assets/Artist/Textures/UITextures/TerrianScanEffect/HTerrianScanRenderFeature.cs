using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HTerrianScanRenderFeature : ScriptableRendererFeature
{
    private HTerrianScanRenderFeatureSettings settings;
    class HTerrianScanRenderPass : ScriptableRenderPass
    {
        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("HTerrianScanProfiler");
        RTHandle m_Handle;
        private RTHandle m_tempHandle;
        private HTerrianScanRenderFeatureSettings settings;
        private RenderTargetIdentifier source;
        private int scanTerrainId;
        private RenderTargetHandle scanShaderTex;

        private Material material;
        
        //构造函数
        public HTerrianScanRenderPass(HTerrianScanRenderFeatureSettings settings)
        {
            //Debug.Log("fuck Unity");
            //this.settings = settings;
            this.settings = VolumeManager.instance.stack.GetComponent<HTerrianScanRenderFeatureSettings>();
            // ask for a depth texture
            ConfigureInput(ScriptableRenderPassInput.Depth);
            material = new Material(Shader.Find("HPostProcessing/HTerrianScanning"));
        }
        
        //资源初始化
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            // Then using RTHandles, the color and the depth properties must be separate
            desc.depthBufferBits = 0;
            // RenderingUtils.ReAllocateIfNeeded(ref m_Handle, desc, FilterMode.Point,
            //     TextureWrapMode.Clamp, name: "_CustomPassHandle");
            RenderingUtils.ReAllocateIfNeeded(ref m_tempHandle, desc, FilterMode.Point,
                TextureWrapMode.Clamp, name: "_CustomPassHandle2");
            //Debug.Log("now in OnCameraSetup");
        }
        
        //执行逻辑
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //从命令缓冲区池获取一个带标签的命令缓冲区，该标签名在帧调试器中可以见到
            CommandBuffer cmd = CommandBufferPool.Get("HTerrianScanRenderPass");
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                //获取目标相机的描述信息，创建一个结构体，里面有render texture各中信息，比如尺寸，深度图精度等等
                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                //设置深度缓冲区，0表示不需要深度缓冲区
                opaqueDesc.depthBufferBits = 0;
                
                // context.ExecuteCommandBuffer(cmd);
                // cmd.Clear();
                //执行渲染逻辑，比如为material当中的shader赋值
                //float scanDepth = scanDepth;
                //Debug.Log("fuck unity scanDepth value" + settings.scanDepth.value);
                material.SetFloat("_ScanDepth", settings.scanDepth.value);
                material.SetFloat("_ScanWidth", settings.scanWidth.value);
                material.SetColor("_LineColor", settings.scanColor.value);
                cmd.GetTemporaryRT(scanShaderTex.id, opaqueDesc);
                cmd.Blit(source, scanShaderTex.Identifier(), material);
                cmd.Blit(scanShaderTex.Identifier(), source);
                //Blitter.BlitCameraTexture(cmd, m_Handle, m_tempHandle, material, 0);
                //Blitter.BlitCameraTexture(cmd, m_tempHandle,renderingData.cameraData.renderer.cameraColorTargetHandle);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        
        //资源释放
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(scanTerrainId);
            base.FrameCleanup(cmd);
        }

        public void SetUpHandler(ScriptableRenderer renderer)
        {
            m_Handle = renderer.cameraColorTargetHandle;
            //old obsolete
            //this.source = renderer.cameraColorTarget;
        }
        
        public void Init(ScriptableRenderer renderer)
        {
            source = renderer.cameraColorTarget;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; //todo:别的好像不行，后面试试
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {   
            if(settings==null || !settings.IsActive())
            {
                return;
            }
            scanTerrainId = Shader.PropertyToID("_ScanTerrain");
            scanShaderTex = new RenderTargetHandle();
            scanShaderTex.id = scanTerrainId;
            cmd.GetTemporaryRT(scanShaderTex.id, cameraTextureDescriptor);
            base.Configure(cmd, cameraTextureDescriptor);
        }
    }
    
    HTerrianScanRenderPass m_ScriptablePass;
    
    //RendererFeature被创建时初始化的函数
    public override void Create()
    {
        /*
         * 作用：初始化ScriptableRenderPass和一些必需的资源。
         * 可以在此方法中执行一些额外的逻辑，比如验证输入的值是否合法，或者更新依赖于该属性的其他属性。
         * 生命周期：Unity在OnEnable/OnValidate中调用此方法。
         */
        m_ScriptablePass = new HTerrianScanRenderPass(settings);
        name = "HTerrianScanRenderFeature";
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    
    //向管线当中添加Pass
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        /*
         * 在RenderSingleCamera中被调用。即每个相机调用一次，每帧更新。
         * 请避免在此处进行实例化。如果指定某一特定相机，可以在此处进行验证。
         * 作用：对ScriptableRenderPass进行排队，且可以将多个通道排队。
         * 也就是说可以依次Enqueue多个Pass
         * 另外，要避免在此方法中访问相机目标，因为这些可能尚未分配，比如renderer.cameraColorTarget。
         */
        if (!renderingData.cameraData.camera.CompareTag("PlayerCamera"))
        {
            return;
        }
        
        renderer.EnqueuePass(m_ScriptablePass);
    }
    
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        /*
         * 生命周期：当渲染目标被分配并准备好使用时，将调用该函数。
         * 作用：获取渲染数据。如果你的功能需要访问相机目标，则应在此方法中执行。
         */
        m_ScriptablePass.Init(renderer);
        m_ScriptablePass.SetUpHandler(renderer);
        
    }

    protected override void Dispose(bool disposing)
    {
        /*
         * 生命周期：OnValidate()、OnDisable()、CreatePipeline()等函数时调用。
         * 作用：释放已分配的资源。
         */
        //m_ScriptablePass?.Dispose();
    }
}
