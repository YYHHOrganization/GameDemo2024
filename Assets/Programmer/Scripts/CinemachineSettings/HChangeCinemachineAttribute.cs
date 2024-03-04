using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class HChangeCinemachineAttribute : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public void ChangeBodyFollowOffset()
    {
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z += 1.2f;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
