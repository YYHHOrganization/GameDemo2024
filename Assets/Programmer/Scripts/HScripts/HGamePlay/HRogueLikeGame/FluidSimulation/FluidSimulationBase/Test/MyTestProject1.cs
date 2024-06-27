using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestProject1 : MonoBehaviour
{
    public MyFluidSimulator1 fluid_simulator;
    public Texture2D source_texture;
    
    private MyFluidGPUResources1 resources;

    private void Start()
    {
        fluid_simulator.Initialize();
        resources = new MyFluidGPUResources1(fluid_simulator);
        resources.Create();
        
        fluid_simulator.AddDye(resources.dye_buffer);
        fluid_simulator.Visualize(resources.dye_buffer);
        fluid_simulator.BindCommandBuffer();
    }
    
    void OnDisable()
    {
        fluid_simulator.Release();
        resources.Release();
    }
    
    void Update()
    {
        fluid_simulator.Tick(Time.deltaTime);
    }
}
