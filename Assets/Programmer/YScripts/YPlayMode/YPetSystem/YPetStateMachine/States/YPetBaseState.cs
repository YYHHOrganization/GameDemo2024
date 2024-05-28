
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class YPetBaseState 
{
    protected GameObject go;
    protected Transform transform;
  
    public YPetBaseState (GameObject gameObject)
    {
        this.go = gameObject;
        this.transform = gameObject.transform;
    }
    //Enter
    public virtual void OnStateEnter() { } 
    public abstract Type Tick();
}
