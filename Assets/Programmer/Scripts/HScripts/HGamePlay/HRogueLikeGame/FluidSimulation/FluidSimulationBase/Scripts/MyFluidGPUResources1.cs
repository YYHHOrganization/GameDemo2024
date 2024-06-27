using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFluidGPUResources1
{
    [Header("Compute Buffers")]
    [Space(2)]
    public ComputeBuffer dye_buffer; // Contains the amount of dye per cell across the whole field. For a colored Dye, this is a RGB value
    
    private int simulation_dimensions; // The resolution of the simulation grid. This is recieved from the fluid simulator
    public MyFluidGPUResources1(MyFluidSimulator1 fso)  // The actual contructor used by the code
    {
        simulation_dimensions = (int) fso.simulation_dimension;
    }
    
    public void Create()
    {
        dye_buffer = new ComputeBuffer(simulation_dimensions * simulation_dimensions, sizeof(float) * 4);
    }
    
    public void Release()
    {
        dye_buffer.Release();
    }
}
