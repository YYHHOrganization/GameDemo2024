using UnityEngine; 
using System.Collections.Generic; 
public static class SD_ObjectPoolCSVFile { 
	public static Dictionary<string, Class_ObjectPoolCSVFile> Class_Dic = JsonReader.ReadJson<Class_ObjectPoolCSVFile> ("Json/Document/ObjectPoolCSVFile");
	}
