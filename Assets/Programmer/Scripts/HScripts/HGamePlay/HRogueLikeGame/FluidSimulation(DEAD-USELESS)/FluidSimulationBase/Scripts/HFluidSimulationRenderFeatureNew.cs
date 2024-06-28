using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HFluidSimulationRenderFeatureNew : ScriptableRendererFeature
{ 
      [System.Serializable]
      public class PassSettingNew
      {
          // 安插位置
          public RenderPassEvent m_passEvent = RenderPassEvent.AfterRenderingTransparents;
     
          // 指定compute shader
          public ComputeShader CS = null;
     
          // 明度控制
          [Range(0, 3)] 
          public float m_Brightness = 1;
     
          // 饱和度控制
          [Range(0, 3)]
          public float m_Saturation = 1;
     
          // 对比度控制
          [Range(0, 3)]
          public float m_Contrast = 1;
      }
 
      class HFluidSimulationPassNew : ScriptableRenderPass
      {
          // profiler tag will show up in frame debugger
          private const string m_ProfilerTag = "BSC Pass";
     
          // 用于存储pass setting
          private HFluidSimulationRenderFeatureNew.PassSettingNew m_passSetting;
     
          private ComputeShader m_CS;
          private int kernal;   // compute shader中的kernal Handle
     
          private RenderTargetIdentifier m_SourRT, m_TargetRT;
     
          struct ShaderID
          {
              // int 相较于 string可以获得更好的性能，因为这是预处理的
              public static readonly int m_BrightnessID = Shader.PropertyToID("_Brightness");
              public static readonly int m_SaturationID = Shader.PropertyToID("_Saturation");
              public static readonly int m_ContrastID = Shader.PropertyToID("_Contrast");
              public static readonly int m_TargetRTID = Shader.PropertyToID("_BufferRT1");
          }
     
          // 线程组个数
          struct Dispatch
          {
              public static int ThreadGroupCountX;
              public static int ThreadGroupCountY;
              public static int ThreadGroupCountZ;
          }
     
          public HFluidSimulationPassNew(HFluidSimulationRenderFeatureNew.PassSettingNew passSetting) 
          {
              this.m_passSetting = passSetting;
     
              renderPassEvent = m_passSetting.m_passEvent;
     
              this.m_CS = m_passSetting.CS;
     
              // 查找kernal handle
              kernal = m_CS.FindKernel("CSMain");
          }
     
          public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
          {
              m_SourRT = renderingData.cameraData.renderer.cameraColorTarget;
     
              RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
              descriptor.depthBufferBits = 0;
              descriptor.enableRandomWrite = true;    // 用于D3D的UAV(无序视图)
     
              // 因为是对屏幕进行处理，所以需要使得一个线程对应一个pixel
              Dispatch.ThreadGroupCountX = (int)descriptor.width / 8;
              Dispatch.ThreadGroupCountY = (int)descriptor.height / 8;
              Dispatch.ThreadGroupCountZ = 1;
     
              cmd.GetTemporaryRT(ShaderID.m_TargetRTID, descriptor, FilterMode.Bilinear);
              m_TargetRT = new RenderTargetIdentifier(ShaderID.m_TargetRTID);
          }
     
          public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
          {
              // Grab a command buffer. We put the actual execution of the pass inside of a profiling scope
              CommandBuffer cmd = CommandBufferPool.Get();
     
              using (new ProfilingScope(cmd, new ProfilingSampler(m_ProfilerTag)))
              {
                  // compute shader定义数据的方式和PS的不同
                  cmd.SetComputeFloatParam(this.m_CS, ShaderID.m_BrightnessID, m_passSetting.m_Brightness);
                  cmd.SetComputeFloatParam(this.m_CS, ShaderID.m_SaturationID, m_passSetting.m_Saturation);
                  cmd.SetComputeFloatParam(this.m_CS, ShaderID.m_ContrastID, m_passSetting.m_Contrast);
                  cmd.SetComputeTextureParam(this.m_CS, kernal, "_Result", m_TargetRT);
                  cmd.SetComputeTextureParam(this.m_CS, kernal, "_Sour", m_SourRT);
                  // 分派线程组并执行compute shader
                  cmd.DispatchCompute(this.m_CS, kernal, Dispatch.ThreadGroupCountX, Dispatch.ThreadGroupCountY, Dispatch.ThreadGroupCountZ);
     
                  cmd.Blit(m_TargetRT, m_SourRT);
              }
     
              context.ExecuteCommandBuffer(cmd);
              CommandBufferPool.Release(cmd);
          }
     
          public override void OnCameraCleanup(CommandBuffer cmd)
          {
              if(cmd == null) throw new ArgumentNullException("cmd");
     
              cmd.ReleaseTemporaryRT(ShaderID.m_TargetRTID);
          }
      }
 
  public PassSettingNew m_Setting = new PassSettingNew();
  HFluidSimulationPassNew m_BSCPass;
  public MyFluidSimulator1 fluid_simulator;
    
  private MyFluidGPUResources1 resources;

 
  /// <inheritdoc/>
  public override void Create()
  {
      m_BSCPass = new HFluidSimulationPassNew(m_Setting);
      fluid_simulator.Initialize();
      resources = new MyFluidGPUResources1(fluid_simulator);
      resources.Create();
        
      fluid_simulator.AddDye(resources.dye_buffer);
  }
 
  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
  {
      // can queue up multiple passes after each other
      renderer.EnqueuePass(m_BSCPass);
  }
}
