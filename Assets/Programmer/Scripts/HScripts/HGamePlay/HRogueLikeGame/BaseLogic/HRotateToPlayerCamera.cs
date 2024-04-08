using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRotateToPlayerCamera : MonoBehaviour
{
    private Camera playerCamera;
 
    void Awake() 
    {
        
    }

    private void OnEnable()
    {
        playerCamera = GameObject.FindWithTag("PlayerCamera").gameObject.GetComponent<Camera>();
    }

    void Update() {
        if(!playerCamera) {
            playerCamera = GameObject.FindWithTag("PlayerCamera").gameObject.GetComponent<Camera>();
            return;
        }
        transform.LookAt(playerCamera.transform, Vector3.up);
    }
}
