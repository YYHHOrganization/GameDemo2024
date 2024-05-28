using UnityEngine; 
using System.Collections.Generic; 
public static class SD_RoguePetCSVFile { 
	public static Dictionary<string, Class_RoguePetCSVFile> Class_Dic = JsonReader.ReadJson<Class_RoguePetCSVFile> ("Json/Document/RoguePetCSVFile");
	}
