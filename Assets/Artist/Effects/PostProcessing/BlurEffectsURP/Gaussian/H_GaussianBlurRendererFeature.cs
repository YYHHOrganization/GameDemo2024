using UnityEngine.Rendering.Universal;

public class H_GaussianBlurRendererFeature : ScriptableRendererFeature
{
    private H_GaussianBlurRenderPass blurRenderPass;
    public override void Create()
    {
        blurRenderPass = new H_GaussianBlurRenderPass();
        name = "GaussianBlur";
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //将该pass添加到渲染管线中
        renderer.EnqueuePass(blurRenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        blurRenderPass.SetUp(renderer.cameraColorTarget);
        blurRenderPass.Setup(renderer);
    }
}
