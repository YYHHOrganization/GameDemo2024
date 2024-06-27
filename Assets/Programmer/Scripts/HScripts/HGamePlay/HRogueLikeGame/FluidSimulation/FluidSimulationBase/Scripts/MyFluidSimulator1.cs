using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class MyFluidSimulator1
{
    // Info used for input through mouse 
    private Vector2 mouse_previous_pos;
    private Camera main_cam;
    
    [Space(4)]
    [Header("Control Settings")]
    [Space(2)]
    public KeyCode ApplyDyeKey  ;
    
    [Space(4)]
    [Header("Simulation Settings")]
    [Space(2)]
    public uint          canvas_dimension     = 512;          // Resolution of the render target used at the end, this can be lower or higher than the actual simulation grid resoltion
    public uint          simulation_dimension = 256;          // Resolution of the simulation grid
    public float         dye_radius           = 1.0f;         // Exact same  behaviour as the force one
    public float         dye_falloff          = 2.0f;         // Exact same  behaviour as the force one
    
    [Header("Compute Shader Refs")]
    [Space(2)]
    public ComputeShader UserInputShader                 ; // The kernels that add user input (dye or force, through constant stream, images, mouse input etc)
    public ComputeShader StructuredBufferToTextureShader ; // Series of utility kernels to convert structured buffers to textures
    
    [HideInInspector]
    public delegate Vector2 GetMousePositionCallBack(ref bool isInBound);
    
    private GetMousePositionCallBack mousePosOverrider; // If this is NULL it is assumed the calculation is happening in screen space and the screen space pos is used for input position
    private RenderTexture visualization_texture;
    
    private CommandBuffer sim_command_buffer;
    
    // The handles for different kernels, for the documentation of what each kernel does, refer to their definition in the compute shader files
    private int _handle_add_dye;
    private int _handle_dye_st2tx;
    public void Initialize() // This function needs to be called before you start using the fluid engine
    {
        MyComputeShaderUtility1.Initialize();
        mousePosOverrider = null;
        main_cam = Camera.main;
        if (main_cam == null) Debug.LogError("Could not find main camera, make sure the camera is tagged as main");
        mouse_previous_pos = GetCurrentMouseInSimulationSpace();
        
        visualization_texture = new RenderTexture((int) canvas_dimension, (int)canvas_dimension, 0)
        {
            enableRandomWrite = true,
            useMipMap         = false,
        };
        visualization_texture.Create();
        
        //Setting kernel handles
        // Always use the GerKernelHandle Method, this methods uses a reflection system of a sort to make error handling and calling functions easier
        _handle_add_dye = MyComputeShaderUtility1.GetKernelHandle(UserInputShader, "AddDye");
        _handle_dye_st2tx = MyComputeShaderUtility1.GetKernelHandle( StructuredBufferToTextureShader, "DyeStructeredToTextureBillinearRGB8");
        // -----------------------
        // Initialize Kernel Parameters, buffers our bound by the actual shader dispatch functions
        UpdateRuntimeKernelParameters();
        
        sim_command_buffer = new CommandBuffer()
        {
            name = "Simulation_Command_Buffer",
        };
    }

    private void UpdateRuntimeKernelParameters()
    {
        SetFloatOnAllShaders(Time.time, "i_Time");
        // ------------------------------------------------------------------------------
        // USER INPUT ADD DYE 
        float randomHue = Mathf.Abs(Mathf.Sin(Time.time * 0.8f + 1.2f) + Mathf.Sin(Time.time * 0.7f + 2.0f));
        randomHue = randomHue - Mathf.Floor(randomHue);
        UserInputShader.SetVector("_dye_color", Color.HSVToRGB(randomHue, Mathf.Abs(Mathf.Sin(Time.time * 0.8f + 1.2f))*0.2f + 0.8f, Mathf.Abs(Mathf.Sin(Time.time * 0.7f + 2.0f)) * 0.2f + 0.5f));
        UserInputShader.SetFloat ("_mouse_dye_radius", dye_radius);
        UserInputShader.SetFloat ("_mouse_dye_falloff", dye_falloff);
        
        //todo:暂时只处理按下鼠标在屏幕上绘制这种简单的需求
        float mouse_pressed = 0.0f;
        if (Input.GetKey(ApplyDyeKey)) mouse_pressed = 1.0f;
        UserInputShader.SetFloat("_mouse_pressed", mouse_pressed);
        Vector2 mouse_pos_struct_pos = GetCurrentMouseInSimulationSpace();
        
        UserInputShader.SetVector("_mouse_position", mouse_pos_struct_pos); // Pass on the mouse position already in the coordinate system of the structured buffer as 2D coordinates

        mouse_previous_pos = mouse_pos_struct_pos;
    }

    private void SetFloatOnAllShaders(float toSet, string name)
    {
        UserInputShader.SetFloat(name, toSet);
    }
    
    public void Tick(float deltaTime)                                                  // should be called at same rate you wish to update your simulation, usually once a frame in update
    {
        UpdateRuntimeKernelParameters();
    }
    
    public void Release()// Make sure to call this function at the end of your implementation on end play
    {
        visualization_texture.Release();
        MyComputeShaderUtility1.Release();
    }

    private Vector2 GetCurrentMouseInSimulationSpace()
    {
        //暂时不考虑Bound的情况，尽可能缩减整个demo的代码量
        if (mousePosOverrider == null)  //这时计算就是屏幕空间的鼠标位置
        {
            Vector3 mouse_pos_pixel_coord = Input.mousePosition; // todo：后续可以把这个改成角色的位置
            Vector2 mouse_pos_normalized  = main_cam.ScreenToViewportPoint(mouse_pos_pixel_coord);
            mouse_pos_normalized  = new Vector2(Mathf.Clamp01(mouse_pos_normalized.x), Mathf.Clamp01(mouse_pos_normalized.y));
            return new Vector2(mouse_pos_normalized.x * simulation_dimension, mouse_pos_normalized.y * simulation_dimension);
        }
        else
        {
            return Vector2.one; //todo:这种情况随便写的，后面记得改一下
        }
    }
    
    public bool IsValid()
    {
        
        if (StructuredBufferToTextureShader == null) { Debug.LogError("ERROR: The  User Input Compute Shader reference is not set in inspector");                    return false;}
        if (UserInputShader == null) { Debug.LogError("ERROR: The Structured Buffer To Texture Compute Shader reference is not set in inspector");   return false;}

        if (sim_command_buffer == null) { Debug.LogError("ERROR: The Fluid Simulater Object is not correctly initalized. The CommandBuffer is NULL");        return false;}
        if (main_cam == null) { Debug.LogError("ERROR: The Fluid Simulater Object is not correctly initalized. The camera reference is NULL");     return false;}

        return true;
    }
    
    private void SetBufferOnCommandList(CommandBuffer cb, ComputeBuffer buffer, string buffer_name)
    {
        cb.SetGlobalBuffer(buffer_name, buffer);
    }
    
    public void AddDye(ComputeBuffer dye_buffer)
    {
        if (!IsValid()) return;

        SetBufferOnCommandList(sim_command_buffer, dye_buffer, "_dye_buffer");
        DispatchComputeOnCommandBuffer(sim_command_buffer, UserInputShader, _handle_add_dye, simulation_dimension, simulation_dimension, 1);
    }
    
    private void DispatchComputeOnCommandBuffer(CommandBuffer cb, ComputeShader toDispatch, int kernel, uint thread_num_x, uint thread_num_y, uint thread_num_z)
    {
        MyDispatchDimensions1 group_nums = MyComputeShaderUtility1.CheckGetDispatchDimensions(toDispatch, kernel, thread_num_x, thread_num_y, thread_num_z);
        cb.DispatchCompute(toDispatch, kernel, (int) group_nums.dispatch_x, (int) group_nums.dispatch_y, (int) group_nums.dispatch_z);

        // Debug
        Debug.Log(string.Format("Attached the computeshader {0}, at kernel {1}, to the commandbuffer {2}." +
                                "Dispatch group numbers are, in x, y,z respectivly: {3}", 
            toDispatch.name, MyComputeShaderUtility1.GetKernelNameFromHandle(toDispatch, kernel), cb.name,
            group_nums.ToString()));
    }
    
    public bool BindCommandBuffer()
    {
        if (!IsValid()) return false;

        main_cam.AddCommandBuffer(CameraEvent.AfterEverything, sim_command_buffer);
        return true;
    }
    public void Visualize(ComputeBuffer buffer_to_visualize)
    {
        if (!IsValid()) return;
        
        SetBufferOnCommandList(sim_command_buffer, buffer_to_visualize, "_Dye_StructeredToTexture_Source_RBB8");
        StructuredBufferToTextureShader.SetTexture(_handle_dye_st2tx, "_Dye_StructeredToTexture_Results_RBB8", visualization_texture);

        DispatchComputeOnCommandBuffer(sim_command_buffer, StructuredBufferToTextureShader, _handle_dye_st2tx, canvas_dimension, canvas_dimension, 1);
        sim_command_buffer.Blit(visualization_texture, BuiltinRenderTextureType.CameraTarget);
    }
    
}
