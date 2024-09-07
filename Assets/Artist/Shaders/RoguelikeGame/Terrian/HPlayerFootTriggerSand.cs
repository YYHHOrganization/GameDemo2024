using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HPlayerFootTriggerSand : MonoBehaviour
{
    private HRGPlayerRutBrush brush;
    private void Start()
    {
        brush = this.GetComponent<HRGPlayerRutBrush>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enter Sand Trigger");
        if (other.gameObject.CompareTag("Ground"))
        {
            if (brush)
            {
                brush.Paint();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (brush)
            {
                brush.Paint();
            }
        }
    }
}
