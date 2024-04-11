using UnityEngine; 
using System.Collections.Generic; 
public static class SD_RogueRoomFile { 
	public static Dictionary<string, Class_RogueRoomFile> Class_Dic = JsonReader.ReadJson<Class_RogueRoomFile> ("Json/Document/RogueRoomFile");
	}
