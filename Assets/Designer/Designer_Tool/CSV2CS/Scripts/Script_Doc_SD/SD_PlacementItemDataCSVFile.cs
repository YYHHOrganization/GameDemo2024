using UnityEngine; 
using System.Collections.Generic; 
public static class SD_PlacementItemDataCSVFile { 
	public static Dictionary<string, Class_PlacementItemDataCSVFile> Class_Dic = JsonReader.ReadJson<Class_PlacementItemDataCSVFile> ("Json/Document/PlacementItemDataCSVFile");
	}
