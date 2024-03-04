using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class YTimelineController:MonoBehaviour
{
    public void ChangeCharacter(int index, GameObject character)
    {
        
    }
    public void ChangeEffect(ScriptableRendererFeature effect)
    {
        effect.SetActive(true);
    }

}
