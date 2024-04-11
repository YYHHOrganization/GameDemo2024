using UnityEngine; 
using System.Collections.Generic; 
public static class SD_BattleRoomCSVFile { 
	public static Dictionary<string, Class_BattleRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_BattleRoomCSVFile> ("Json/Document/BattleRoomCSVFile");
	}
