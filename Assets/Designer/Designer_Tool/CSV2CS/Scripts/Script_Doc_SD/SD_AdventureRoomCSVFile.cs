using UnityEngine; 
using System.Collections.Generic; 
public static class SD_AdventureRoomCSVFile { 
	public static Dictionary<string, Class_AdventureRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_AdventureRoomCSVFile> ("Json/Document/AdventureRoomCSVFile");
	}
