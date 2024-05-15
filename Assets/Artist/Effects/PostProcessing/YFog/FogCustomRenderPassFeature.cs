using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogCustomRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FogSettings//定义一个可以设置的类
    {
        //定义一个渲染事件，这个事件在渲染透明物体之后执行
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        //定义一个材质
        public Material material = null;
    }
    public FogSettings settings = new FogSettings();//实例化一个设置类,用于在Inspector面板中显示
    
    class CustomRenderPass : ScriptableRenderPass
    {
        private FogSettings settings;

        public CustomRenderPass(FogSettings settings)
        {
            this.settings = settings;
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //重建像素的世界坐标：float4 worldPos=_WorldSpaceCameraPos + linearDepth * interpolateRay
            Camera camera = renderingData.cameraData.camera;//获取相机
            Matrix4x4 frustumCorners = Matrix4x4.identity;//4*4的单位矩阵
            float fov = camera.fieldOfView;//相机的fov值（度数）
            float near = camera.nearClipPlane;//相机的近平面
            float aspect = camera.aspect;//相机屏幕的宽长比
//Deg2Rad：角度转弧度
            float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);//计算出近平面高度的一半
            Vector3 toRight = camera.transform.right * halfHeight * aspect;//得到近平面中心点向右的向量（相机空间的右侧，不是世界空间）
            Vector3 toTop = camera.transform.up * halfHeight;//得到近平面中心点向上的向量（相机空间的上方，不是世界空间）
            Vector3 topLeft = camera.transform.forward * near + toTop - toRight;//相机到近平面左上角的向量
            //interpolateRay在顶点片元着色器中根据像素所在象限确定顶点使用哪个，然后片元着色器会自动插值得到相机到每个像素的向量X
            //由于深度图采样得到的深度是线性深度不是相机与点的欧氏距离，所以需要根据相似三角形来计算出欧氏距离：depth/dist=Near/|X|
            //dist =（ |X|  / Near ）* depth
            //所以相机到像素世界坐标点的实际距离是：dist* normalize(X) =（ |X|  / Near ）* depth * normalize(X);
            //Near都相同，所以直接在这里先进行计算: dist* normalize(X) = depth * (（|X|/Near）* normalize(X) );
            float scale = topLeft.magnitude / near;// |X|/Near
            topLeft.Normalize();
            topLeft *= scale;
            Vector3 topRight = camera.transform.forward * near + toRight + toTop;//相机到近平面右上角的向量
            topRight.Normalize();
            topRight *= scale;
            Vector3 bottomLeft = camera.transform.forward * near - toTop - toRight;//相机到近平面左下角的向量
            bottomLeft.Normalize();
            bottomLeft *= scale;
            Vector3 bottomRight = camera.transform.forward * near + toRight - toTop;//相机到近平面右下角的向量
            bottomRight.Normalize();
            bottomRight *= scale;
            frustumCorners.SetRow(0, bottomLeft);
            frustumCorners.SetRow(1, bottomRight);
            frustumCorners.SetRow(2, topRight);
            frustumCorners.SetRow(3, topLeft);
            settings.material.SetMatrix("_FrustumCornersRay", frustumCorners);
            
            
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


