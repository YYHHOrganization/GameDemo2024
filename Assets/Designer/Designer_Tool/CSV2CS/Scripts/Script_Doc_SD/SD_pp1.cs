using UnityEngine; 
using System.Collections.Generic; 
public static class SD_pp1 { 
	public static Dictionary<string, Class_pp1> Class_Dic = JsonReader.ReadJson<Class_pp1> ("Json/Document/pp1");
	}
