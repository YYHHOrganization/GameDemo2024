using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTestGiveOutRandomItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("now player in trigger");
            GiveOutARandomItem();
        }
    }

    private void GiveOutARandomItem()
    {
        HRougeAttributeManager.Instance.RollingARandomItem(this.transform.GetChild(0).transform);
    }
}
