using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HJustTestRipple : MonoBehaviour
{
    public GameObject rippleVFX;
    private Material mat;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Puppet"))
        {
            var ripples = Instantiate(rippleVFX, transform) as GameObject;
            var psr = ripples.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            mat = psr.material;
            mat.SetVector("_SphereMaskCenter", other.contacts[0].point);
            Destroy(ripples, 2f);
        }
    }
}
