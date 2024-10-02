using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MobileSSPRRenderFeature : ScriptableRendererFeature
{
    public static MobileSSPRRenderFeature instance;

    [System.Serializable]
    public class SSPRSettings
    {
        [Header("Settings")]
        public bool ShouldRenderSSPR = true;
        [Range(0.01f, 1f)]
        public float FadeOutScreenBorderWidthVerticle = 0.25f;
        [Range(0.01f, 1f)]
        public float FadeOutScreenBorderWidthHorizontal = 0.35f;
        [Range(0,8f)]
        public float ScreenLRStretchIntensity = 4;
        [Range(-1f,1f)]
        public float ScreenLRStretchThreshold = 0.7f;
        [ColorUsage(true,true)]
        public Color TintColor = Color.white;

        public ComputeShader cs;
        //////////////////////////////////////////////////////////////////////////////////
        [Header("Performance Settings")]
        [Range(128, 1024)]
        [Tooltip("视觉效果可以接受的话，可以适当降低这个值，以提高性能")]
        public int RT_height = 512;
        [Tooltip("是否启用HDR")]
        public bool UseHDR = true;
        [Tooltip("can set to false for better performance, if visual quality lost is acceptable")]
        public bool ApplyFillHoleFix = true;
        [Tooltip("can set to false for better performance, if flickering is acceptable")]
        public bool ShouldRemoveFlickerFinalControl = true;
        public float HorizontalReflectionPlaneHeightWS = 0.01f; //default higher than ground a bit, to avoid ZFighting if user placed a ground plane at y=0
        
        //////////////////////////////////////////////////////////////////////////////////
        [Header("Danger Zone")]
        [Tooltip("You should always turn this on, unless you want to debug")]
        public bool EnablePerPlatformAutoSafeGuard = true;
    }
    public SSPRSettings settings = new SSPRSettings();

    public class CustomSSPRPass : ScriptableRenderPass
    {
        private SSPRSettings settings;
        private ComputeShader cs;
        static readonly int _SSPR_ColorRT_pid = Shader.PropertyToID("_MobileSSPR_ColorRT");  //这个属性在hlsl文件当中
        static readonly int _SSPR_PackedDataRT_pid = Shader.PropertyToID("_MobileSSPR_PackedDataRT");
        static readonly int _SSPR_PosWSyRT_pid = Shader.PropertyToID("_MobileSSPR_PosWSyRT");
        RenderTargetIdentifier _SSPR_ColorRT_rti = new RenderTargetIdentifier(_SSPR_ColorRT_pid);
        RenderTargetIdentifier _SSPR_PackedDataRT_rti = new RenderTargetIdentifier(_SSPR_PackedDataRT_pid);
        RenderTargetIdentifier _SSPR_PosWSyRT_rti = new RenderTargetIdentifier(_SSPR_PosWSyRT_pid);

        ShaderTagId lightMode_SSPR_sti = new ShaderTagId("MobileSSPR");//reflection plane renderer's material's shader must use this LightMode

        const int SHADER_NUMTHREAD_X = 8; //must match compute shader's [numthread(x)]
        const int SHADER_NUMTHREAD_Y = 8; //must match compute shader's [numthread(y)]
        
        public CustomSSPRPass(SSPRSettings settings)
        {
            this.settings = settings;
            //cs = (ComputeShader)Resources.Load("MobileSSPRComputeShader");  //shader需要放在Resources文件夹下面
            cs = settings.cs;
        }
        
        int GetRTHeight()
        {
            return Mathf.CeilToInt(settings.RT_height / (float)SHADER_NUMTHREAD_Y) * SHADER_NUMTHREAD_Y;
        }
        int GetRTWidth()
        {
            float aspect = (float)Screen.width / Screen.height;
            return Mathf.CeilToInt(GetRTHeight() * aspect / (float)SHADER_NUMTHREAD_X) * SHADER_NUMTHREAD_X;
        }
        
        /// <summary>
        /// If user enabled PerPlatformAutoSafeGuard, this function will return true if we should use mobile path
        /// </summary>
        bool ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve()
        {
            if (settings.EnablePerPlatformAutoSafeGuard)
            {
                //if RInt RT is not supported, use mobile path
                if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RInt))
                    return true;

                //tested Metal(even on a Mac) can't use InterlockedMin().
                //so if metal, use mobile path
                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                    return true;
#if UNITY_EDITOR
                //PC(DirectX) can use RenderTextureFormat.RInt + InterlockedMin() without any problem, use Non-Mobile path.
                //Non-Mobile path will NOT produce any flickering
                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12)
                    return false;
#elif UNITY_ANDROID
                //- samsung galaxy A70(Adreno612) will fail if use RenderTextureFormat.RInt + InterlockedMin() in compute shader
                //- but Lenovo S5(Adreno506) is correct, WTF???
                //because behavior is different between android devices, we assume all android are not safe to use RenderTextureFormat.RInt + InterlockedMin() in compute shader
                //so android always go mobile path
                return true;
#endif
            }

            //let user decide if we still don't know the correct answer
            return !settings.ShouldRemoveFlickerFinalControl;
        }

        //This method is called before executing the render pass.
        //它可以用来配置渲染目标及其清除状态。还可以用来创建临时渲染目标纹理。当为空时，这个渲染通道将渲染到活动相机的渲染目标。
        //你不应该调用 CommandBuffer.SetRenderTarget。应该调用 <c>ConfigureTarget</c> 和 <c>ConfigureClear</c>。渲染管线将确保以高效的方式进行目标设置和清除。
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor rtd = new RenderTextureDescriptor(GetRTWidth(), GetRTHeight(),RenderTextureFormat.Default, 0, 0);
            rtd.enableRandomWrite = true;
            rtd.sRGB = false;  //don't need gamma correction when sampling these RTs, it is linear data already because it will be filled by screen's linear data
            
            //color RT
            bool shouldUseHDRColorRT = settings.UseHDR;
            if (cameraTextureDescriptor.colorFormat == RenderTextureFormat.ARGB32)
            {
                // if there are no HDR info to reflect anyway, no need a HDR colorRT
                shouldUseHDRColorRT = false;
            }
            rtd.colorFormat = shouldUseHDRColorRT ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32; //we need alpha! (usually LDR is enough, ignore HDR is acceptable for reflection)
            cmd.GetTemporaryRT(_SSPR_ColorRT_pid, rtd);
            //PackedData RT
            //以下这个if和else见SSPR的说明文档
            if (ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve())  //理论上，针对移动端，以及一些不支持RInt的平台，或者用户选择不去除闪烁的情况下，我们会使用这个RT
            {
                //use unsafe method if mobile
                //posWSy RT (will use this RT for posWSy compare test, just like the concept of regular depth buffer)
                rtd.colorFormat = RenderTextureFormat.RFloat;
                cmd.GetTemporaryRT(_SSPR_PosWSyRT_pid, rtd);
            }
            else
            {
                //use 100% correct method if console/PC
                rtd.colorFormat = RenderTextureFormat.RInt;
                cmd.GetTemporaryRT(_SSPR_PackedDataRT_pid, rtd);
            }
        }
        
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cb = CommandBufferPool.Get("SSPR"); //这个名字可以随便取，只要不重复就行
            int dispatchThreadGroupXCount = GetRTWidth() / SHADER_NUMTHREAD_X; //divide by shader's numthreads.x
            int dispatchThreadGroupYCount = GetRTHeight() / SHADER_NUMTHREAD_Y; //divide by shader's numthreads.y
            int dispatchThreadGroupZCount = 1; //divide by shader's numthreads.z
            if (settings.ShouldRenderSSPR)
            {
                cb.SetComputeVectorParam(cs, Shader.PropertyToID("_RTSize"), new Vector2(GetRTWidth(), GetRTHeight()));
                cb.SetComputeFloatParam(cs, Shader.PropertyToID("_HorizontalPlaneHeightWS"), settings.HorizontalReflectionPlaneHeightWS);
                cb.SetComputeVectorParam(cs, Shader.PropertyToID("_CameraDirection"), renderingData.cameraData.camera.transform.forward);
                cb.SetComputeFloatParam(cs, Shader.PropertyToID("_FadeOutScreenBorderWidthVerticle"), settings.FadeOutScreenBorderWidthVerticle);
                cb.SetComputeFloatParam(cs, Shader.PropertyToID("_FadeOutScreenBorderWidthHorizontal"), settings.FadeOutScreenBorderWidthHorizontal);
                cb.SetComputeVectorParam(cs, Shader.PropertyToID("_FinalTintColor"), settings.TintColor);
                cb.SetComputeFloatParam(cs, Shader.PropertyToID("_ScreenLRStretchIntensity"), settings.ScreenLRStretchIntensity);
                cb.SetComputeFloatParam(cs, Shader.PropertyToID("_ScreenLRStretchThreshold"), settings.ScreenLRStretchThreshold);
                //we found that on metal, UNITY_MATRIX_VP is not correct, so we will pass our own VP matrix to compute shader
                Camera camera = renderingData.cameraData.camera;
                //camera涉及的那两个矩阵都是列主序的，跟之前图形学学习的知识保持一致
                Matrix4x4 VP = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;
                cb.SetComputeMatrixParam(cs, "_VPMatrix", VP);
                RTHandle _CameraDepthTexture;
                RTHandle _CameraOpaqueTexture;
                _CameraOpaqueTexture = renderingData.cameraData.renderer.cameraColorTargetHandle;
                _CameraDepthTexture = renderingData.cameraData.renderer.cameraDepthTargetHandle;
                if(ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve())
                {
                    //todo:暂时先不管，PC端不走这个分支
                }
                else
                {
                    ////////////////////////////////////////////////
                    //Non-Mobile Path (PC/console)
                    ////////////////////////////////////////////////
                    //kernel NonMobilePathClear
                    int kernel_NonMobilePathClear = cs.FindKernel("NonMobilePathClear");
                    cb.SetComputeTextureParam(cs, kernel_NonMobilePathClear, "HashRT", _SSPR_PackedDataRT_rti);
                    cb.SetComputeTextureParam(cs, kernel_NonMobilePathClear, "ColorRT", _SSPR_ColorRT_rti);
                    cb.DispatchCompute(cs, kernel_NonMobilePathClear, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);
                    //kernel NonMobilePathRenderHashRT
                    int kernel_NonMobilePathRenderHashRT = cs.FindKernel("NonMobilePathRenderHashRT");
                    cb.SetComputeTextureParam(cs, kernel_NonMobilePathRenderHashRT, "HashRT", _SSPR_PackedDataRT_rti);
                    cb.SetComputeTextureParam(cs, kernel_NonMobilePathRenderHashRT, "_CameraDepthTexture", _CameraDepthTexture);

                    cb.DispatchCompute(cs, kernel_NonMobilePathRenderHashRT, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

                    //resolve to ColorRT
                    int kernel_NonMobilePathResolveColorRT = cs.FindKernel("NonMobilePathResolveColorRT");
                    cb.SetComputeTextureParam(cs, kernel_NonMobilePathResolveColorRT, "_CameraOpaqueTexture", _CameraOpaqueTexture);
                    cb.SetComputeTextureParam(cs, kernel_NonMobilePathResolveColorRT, "ColorRT", _SSPR_ColorRT_rti);
                    cb.SetComputeTextureParam(cs, kernel_NonMobilePathResolveColorRT, "HashRT", _SSPR_PackedDataRT_rti);
                    cb.DispatchCompute(cs, kernel_NonMobilePathResolveColorRT, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);
                }
                
                //optional shared pass to improve result only: fill RT hole
                if(settings.ApplyFillHoleFix)
                {
                    int kernel_FillHoles = cs.FindKernel("FillHoles");
                    cb.SetComputeTextureParam(cs, kernel_FillHoles, "ColorRT", _SSPR_ColorRT_rti);
                    cb.SetComputeTextureParam(cs, kernel_FillHoles, "PackedDataRT", _SSPR_PackedDataRT_rti);
                    cb.DispatchCompute(cs, kernel_FillHoles, Mathf.CeilToInt(dispatchThreadGroupXCount / 2f), Mathf.CeilToInt(dispatchThreadGroupYCount / 2f), dispatchThreadGroupZCount);
                }

                
                //send out to global, for user's shader to sample reflection result RT (_MobileSSPR_ColorRT)
                //where _MobileSSPR_ColorRT's rgb is reflection color, a is reflection usage 0~1 for user's shader to lerp with fallback reflection probe's rgb
                cb.SetGlobalTexture(_SSPR_ColorRT_pid, _SSPR_ColorRT_rti);
                cb.EnableShaderKeyword("_MobileSSPR");
            }
            else  //不打开SSPR Render Pass
            {
                //allow user to skip SSPR related code if disabled
                cb.DisableShaderKeyword("_MobileSSPR");
            }
            context.ExecuteCommandBuffer(cb);
            CommandBufferPool.Release(cb);
            //======================================================================
            //draw objects(e.g. reflective wet ground plane) with lightmode "MobileSSPR", which will sample _MobileSSPR_ColorRT
            //感觉使用这种方式就可以对包含"MobileSSPR"这个shaderTagId的物体进行单独的后处理渲染了，对应的shader会采样_MobileSSPR_ColorRT
            DrawingSettings drawingSettings = CreateDrawingSettings(lightMode_SSPR_sti, ref renderingData, SortingCriteria.CommonOpaque);
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_SSPR_ColorRT_pid);
            if(ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve())
                cmd.ReleaseTemporaryRT(_SSPR_PosWSyRT_pid);
            else
                cmd.ReleaseTemporaryRT(_SSPR_PackedDataRT_pid);
        }

        
    }
    
    private CustomSSPRPass m_ScriptablePass;
    public override void Create()
    {
        instance = this;

        m_ScriptablePass = new CustomSSPRPass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;//we must wait until _CameraOpaqueTexture & _CameraDepthTexture is usable
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
