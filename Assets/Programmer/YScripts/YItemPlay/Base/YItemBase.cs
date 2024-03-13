using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class YItemBase:MonoBehaviour
{
    int id;
    string name;
    string description;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnEnterItem();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            OnExitItem();
        }
    }
    public virtual void OnEnterItem()
    {
        
    }
    
    public virtual void OnExitItem()
    {
        
    }

}
