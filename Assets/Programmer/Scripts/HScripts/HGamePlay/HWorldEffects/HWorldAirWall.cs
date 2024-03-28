using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HWorldAirWall : MonoBehaviour
{
    private Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mat.SetVector("_PlayerPos",other.transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mat.SetVector("_PlayerPos",other.transform.position);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mat.SetVector("_PlayerPos",Vector3.zero);
        }
    }
    
}
