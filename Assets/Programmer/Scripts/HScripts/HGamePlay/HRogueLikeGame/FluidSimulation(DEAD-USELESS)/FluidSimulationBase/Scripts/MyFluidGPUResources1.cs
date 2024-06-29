using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFluidGPUResources1
{
    [Header("Compute Buffers")]
    [Space(2)]
    public ComputeBuffer dye_buffer; // Contains the amount of dye per cell across the whole field. For a colored Dye, this is a RGB value
    public ComputeBuffer velocity_buffer;
    public static ComputeBuffer buffer_ping; // Used for solver loops, by ping ponging back and forth between these two, you can execture a loop
    public static ComputeBuffer buffer_pong; // Used for solver loops, by ping ponging back and forth between these two, you can execture a loop
    
    private int simulation_dimensions; // The resolution of the simulation grid. This is recieved from the fluid simulator
    public MyFluidGPUResources1(MyFluidSimulator1 fso)  // The actual contructor used by the code
    {
        simulation_dimensions = (int) fso.simulation_dimension;
    }
    
    public void Create()
    {
        dye_buffer = new ComputeBuffer(simulation_dimensions * simulation_dimensions, sizeof(float) * 4);
        velocity_buffer = new ComputeBuffer(simulation_dimensions * simulation_dimensions, sizeof(float) * 4);
        buffer_pong = new ComputeBuffer(simulation_dimensions * simulation_dimensions, sizeof(float) * 4);
        buffer_ping = new ComputeBuffer(simulation_dimensions * simulation_dimensions, sizeof(float) * 4);
    }
    
    public void Release()
    {
        dye_buffer.Release();
        velocity_buffer.Release();
        buffer_ping.Release();
        buffer_pong.Release();
    }
    
    public static bool StaticResourcesCreated()
    {
        if (!buffer_ping.IsValid() || !buffer_pong.IsValid()) { Debug.LogError("Static Resources are still not created. Should not be accessed!"); return false; }
        return true;
    }
}
