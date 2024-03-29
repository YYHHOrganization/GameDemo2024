using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTriggerUITutorial : MonoBehaviour
{
    public string tutorialName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HMessageShowMgr.Instance.ShowMessage(tutorialName);
            Destroy(gameObject);
        }
    }
}
