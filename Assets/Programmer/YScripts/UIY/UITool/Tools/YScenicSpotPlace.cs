using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YScenicSpotPlace : MonoBehaviour
{
    public int placeIndex;
    public int placeID;
    public string placeName;
    public string PlaceName
    {
        get
        {
            // return placeName;
            return SD_ScenicSpotPlaceCSVFile.Class_Dic[placeID.ToString()].ChineseName;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        placeName = SD_ScenicSpotPlaceCSVFile.Class_Dic[placeID.ToString()].ChineseName;
        placeIndex = int.Parse(SD_ScenicSpotPlaceCSVFile.Class_Dic[placeID.ToString()].sequence);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
