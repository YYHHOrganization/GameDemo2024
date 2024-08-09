using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrassBendingRTPrePass : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        //_GrassBendingRT_pid：这是一个整数，它是通过调用Shader.PropertyToID方法得到的。
        //这个方法会将一个字符串（在这里是"_GrassBendingRT"）转换为一个唯一的ID，
        //这个ID可以用于在着色器中引用一个属性。这样做的好处是，使用ID来引用属性比使用字符串更快，更有效率。
        static readonly int _GrassBendingRT_pid = Shader.PropertyToID("_GrassBendingRT");
        //用于标识一个渲染目标   这个渲染目标标识符对应的是_GrassBendingRT这个属性。
        static readonly RenderTargetIdentifier _GrassBendingRT_rti = new RenderTargetIdentifier(_GrassBendingRT_pid);
        ShaderTagId GrassBending_stid = new ShaderTagId("GrassBending");//着色器标签

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //512*512 is big enough for this demo's max grass count, can use a much smaller RT in regular use case
            //TODO: make RT render pos follow main camera view frustrum, allow using a much smaller size RT
            //创建一个临时的渲染目标纹理（Render Texture）
            cmd.GetTemporaryRT(_GrassBendingRT_pid, new RenderTextureDescriptor(512, 512, RenderTextureFormat.R8,0));
            //设置渲染目标
            ConfigureTarget(_GrassBendingRT_rti);
            //设置清除状态
            ConfigureClear(ClearFlag.All, Color.white);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!InstancedIndirectGrassRenderer.instance)
            {
                //Debug.LogWarning("InstancedIndirectGrassRenderer not found, abort GrassBendingRTPrePass's Execute");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("GrassBendingRT");

            //视图矩阵（位于视图空间/摄像机空间）：从世界空间到这个假想摄像机视图空间的变换矩阵
            //创建一个新的视图矩阵，这个视图矩阵与一个想象中的摄像机相同，这个摄像机位于草地中心的上方1个单位处，并且朝向草地（鸟瞰视图）
            //make a new view matrix that is the same as an imaginary camera above grass center 1 units and looking at grass(bird view)
            //scale.z is -1 because view space will look into -Z while world space will look into +Z
            //camera transform's local to world's inverse means camera's world to view = world to local
            //这里的scale.z被设置为-1，是因为在视图空间中，Z轴的方向是朝向屏幕内的，而在世界空间中，Z轴的方向是朝向屏幕外的。
            //所以，需要将Z轴的缩放因子设置为-1，来翻转Z轴的方向
            Matrix4x4 viewMatrix = Matrix4x4.TRS(
                InstancedIndirectGrassRenderer.instance.transform.position + new Vector3(0, 1, 0),
                Quaternion.LookRotation(-Vector3.up), 
                new Vector3(1,1,-1)).inverse;

            //ortho camera with 1:1 aspect, size = 50 此处正交投影，视口大小为50
            float sizeX = InstancedIndirectGrassRenderer.instance.transform.localScale.x;
            float sizeZ = InstancedIndirectGrassRenderer.instance.transform.localScale.z;
            Matrix4x4 projectionMatrix = Matrix4x4.Ortho(-sizeX,sizeX, -sizeZ, sizeZ, 0.5f, 1.5f);

            //override view & Projection matrix
            cmd.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
            context.ExecuteCommandBuffer(cmd);
            
            //draw all trail renderer using SRP batching
            var drawSetting = CreateDrawingSettings(GrassBending_stid, ref renderingData, SortingCriteria.CommonTransparent);
            var filterSetting = new FilteringSettings(RenderQueueRange.all);
            context.DrawRenderers(renderingData.cullResults, ref drawSetting, ref filterSetting);

            //restore camera matrix
            //把原来的摄像机的视图矩阵和投影矩阵设置回去
            cmd.Clear();
            cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

            //set global RT
            cmd.SetGlobalTexture(_GrassBendingRT_pid, new RenderTargetIdentifier(_GrassBendingRT_pid));

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_GrassBendingRT_pid);
        }
    }

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses; //don't do RT switch when rendering _CameraColorTexture, so use AfterRenderingPrePasses
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


