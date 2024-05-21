using UnityEngine; 
using System.Collections.Generic; 
public static class SD_RogueGachaThingCSVFile { 
	public static Dictionary<string, Class_RogueGachaThingCSVFile> Class_Dic = JsonReader.ReadJson<Class_RogueGachaThingCSVFile> ("Json/Document/RogueGachaThingCSVFile");
	}
