using UnityEngine; 
using System.Collections.Generic; 
public static class SD_RogueCommonItemCSVFile { 
	public static Dictionary<string, Class_RogueCommonItemCSVFile> Class_Dic = JsonReader.ReadJson<Class_RogueCommonItemCSVFile> ("Json/Document/RogueCommonItemCSVFile");
	}
