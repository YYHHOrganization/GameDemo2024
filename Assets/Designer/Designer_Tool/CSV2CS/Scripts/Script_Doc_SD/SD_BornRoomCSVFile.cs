using UnityEngine; 
using System.Collections.Generic; 
public static class SD_BornRoomCSVFile { 
	public static Dictionary<string, Class_BornRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_BornRoomCSVFile> ("Json/Document/BornRoomCSVFile");
	}
