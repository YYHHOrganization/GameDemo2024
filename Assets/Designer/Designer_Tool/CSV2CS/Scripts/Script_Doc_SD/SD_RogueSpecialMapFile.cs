using UnityEngine; 
using System.Collections.Generic; 
public static class SD_RogueSpecialMapFile { 
	public static Dictionary<string, Class_RogueSpecialMapFile> Class_Dic = JsonReader.ReadJson<Class_RogueSpecialMapFile> ("Json/Document/RogueSpecialMapFile");
	}
