using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HJustTestRipple : MonoBehaviour
{
    public GameObject rippleVFX;
    private Material mat;
    private GameObject ripples;
    private ParticleSystemRenderer psr;

    private float checkTime = 0.3f;
    private float checkTimer = 0f;
    bool isBeShot = false;

    private void Awake()
    {
        ripples = Instantiate(rippleVFX, transform) as GameObject;
        psr = ripples.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
        mat = psr.material;
    }

    private void Update()
    {
        if (!isBeShot)
        {
            return;
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkTime)
            {
                checkTimer = 0f;
                isBeShot = false;
                SetRipple(Vector3.zero);
            }
        }
        
    }


    public void SetRipple(Vector3 contactPoint)
    {
        if (ripples && mat)
        {
            isBeShot = true;
            checkTimer = 0f;
            mat.SetVector("_SphereMaskCenter", contactPoint);
            //Debug.Log(contactPoint + " ssssssssssssssssssss");
        }
        
    }
    
}
