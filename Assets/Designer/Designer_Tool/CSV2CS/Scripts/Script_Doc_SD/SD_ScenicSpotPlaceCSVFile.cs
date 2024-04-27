using UnityEngine; 
using System.Collections.Generic; 
public static class SD_ScenicSpotPlaceCSVFile { 
	public static Dictionary<string, Class_ScenicSpotPlaceCSVFile> Class_Dic = JsonReader.ReadJson<Class_ScenicSpotPlaceCSVFile> ("Json/Document/ScenicSpotPlaceCSVFile");
	}
