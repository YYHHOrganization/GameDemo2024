using UnityEngine; 
using System.Collections.Generic; 
public static class SD_GameRoomCSVFile { 
	public static Dictionary<string, Class_GameRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_GameRoomCSVFile> ("Json/Document/GameRoomCSVFile");
	}
