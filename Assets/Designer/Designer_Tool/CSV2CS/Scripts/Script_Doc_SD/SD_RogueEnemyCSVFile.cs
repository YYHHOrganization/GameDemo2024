using UnityEngine; 
using System.Collections.Generic; 
public static class SD_RogueEnemyCSVFile { 
	public static Dictionary<string, Class_RogueEnemyCSVFile> Class_Dic = JsonReader.ReadJson<Class_RogueEnemyCSVFile> ("Json/Document/RogueEnemyCSVFile");
	}
