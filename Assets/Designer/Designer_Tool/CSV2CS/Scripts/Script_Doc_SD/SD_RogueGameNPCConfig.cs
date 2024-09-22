using UnityEngine; 
using System.Collections.Generic; 
public static class SD_RogueGameNPCConfig { 
	public static Dictionary<string, Class_RogueGameNPCConfig> Class_Dic = JsonReader.ReadJson<Class_RogueGameNPCConfig> ("Json/Document/RogueGameNPCConfig");
	}
