using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct MyDispatchDimensions1
{
    public uint dispatch_x;
    public uint dispatch_y;
    public uint dispatch_z;

    public override string ToString() 
    {
        return string.Format("({0}, {1}, {2})", dispatch_x, dispatch_y, dispatch_z);
    }
}

public static class MyComputeShaderUtility1
{
    public static Dictionary<ComputeShader, Dictionary<int, string>> map_kernelsToNames;
    public static void Initialize()
    {
        if (map_kernelsToNames != null)
            map_kernelsToNames.Clear();
        else map_kernelsToNames = new Dictionary<ComputeShader, Dictionary<int, string>>();
    }
    
    public static void Release()
    {
        map_kernelsToNames.Clear();
    }
    
    public static MyDispatchDimensions1 CheckGetDispatchDimensions(ComputeShader shader, int handle, uint desired_threadNum_x, uint desired_threadNum_y, uint desired_threadNum_z)
    {
        uint group_size_x, group_size_y, group_size_z;
        shader.GetKernelThreadGroupSizes(handle, out group_size_x, out group_size_y, out group_size_z);
        string kernelName = GetKernelNameFromHandle(shader, handle);
        
        if (desired_threadNum_x % group_size_x != 0 ||
            desired_threadNum_y % group_size_y != 0 ||
            desired_threadNum_z % group_size_z != 0)
        {
            Debug.LogError(string.Format("ERROR: Shader {0}, on kernel {1}, has mismatched desired thread numbers and thread groupd sizes. The desired thread "   +
                                         "number needs to be a multiply of the group size. Either change the desired thread number or change the " +
                                         "group size in the shader.", shader.name, kernelName));
            Debug.Break();
        }
        
        MyDispatchDimensions1 dp;
       
        dp.dispatch_x = desired_threadNum_x / group_size_x;
        dp.dispatch_y = desired_threadNum_y / group_size_y;
        dp.dispatch_z = desired_threadNum_z / group_size_z;

        uint totalThreadNumber = desired_threadNum_x * desired_threadNum_y * desired_threadNum_z;
        if (totalThreadNumber < 64)
            Debug.LogWarning(string.Format("Total threads number on shader {0}, kernel {1} is: {2}, " +
                                           "this is probably too low and causes under utilization of GPU blocks!", 
                shader.name, kernelName, totalThreadNumber));
       
        return dp;
    }
    
    public static string GetKernelNameFromHandle(ComputeShader cp, int handle)
    {
        if (map_kernelsToNames.ContainsKey(cp))
        {
            if (map_kernelsToNames[cp].ContainsKey(handle))
                return map_kernelsToNames[cp][handle];
        }
        return handle.ToString();
    }
    
    public static int GetKernelHandle(ComputeShader cp, string name)
    {
        int handle = cp.FindKernel(name);
        Dictionary<int, string> cp_kernles;
        if (map_kernelsToNames.ContainsKey(cp)) cp_kernles = map_kernelsToNames[cp];
        else
        {
            cp_kernles = new Dictionary<int, string>();
            map_kernelsToNames.Add(cp, cp_kernles);   // Add this dictionary to the other when you create it. Since this is a reference type, you dont need to add this if you have already done it
        }
        cp_kernles.Add(handle, name);
        
        return handle;
    }
    
}
