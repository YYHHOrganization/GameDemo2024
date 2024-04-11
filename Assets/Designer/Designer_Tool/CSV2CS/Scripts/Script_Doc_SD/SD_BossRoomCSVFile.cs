using UnityEngine; 
using System.Collections.Generic; 
public static class SD_BossRoomCSVFile { 
	public static Dictionary<string, Class_BossRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_BossRoomCSVFile> ("Json/Document/BossRoomCSVFile");
	}
