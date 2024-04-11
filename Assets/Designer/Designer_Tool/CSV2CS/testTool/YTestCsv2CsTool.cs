using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTestCsv2CsTool : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //SD_pp1.Class_Dic["HighContrast"].AttributeNames+"");
        Debug.Log("SD_pp1.Class_Dic[\"HighContrast\"].AttributeNames: " + SD_pp1.Class_Dic["HighContrast"].AttributeNames);
        
        //SD_pp1.Class_Dic["HighContrast"].EffectFieldName+"");
        Debug.Log("SD_pp1.Class_Dic[\"HighContrast\"].EffectFieldName: " + SD_pp1.Class_Dic["HighContrast"].EffectFieldName);
        
        //SD_pp1.Class_Dic["HighContrast"].EffectName+"");
        Debug.Log("SD_pp1.Class_Dic[\"HighContrast\"].EffectName: " + SD_pp1.Class_Dic["HighContrast"].EffectName);
        
        //SD_pp1.Class_Dic["HighContrast"].UIShowName+"");
        Debug.Log("SD_pp1.Class_Dic[\"HighContrast\"].UIShowName: " + SD_pp1.Class_Dic["HighContrast"].UIShowName);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
