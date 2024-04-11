using UnityEngine; 
using System.Collections.Generic; 
public static class SD_ItemRoomCSVFile { 
	public static Dictionary<string, Class_ItemRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_ItemRoomCSVFile> ("Json/Document/ItemRoomCSVFile");
	}
