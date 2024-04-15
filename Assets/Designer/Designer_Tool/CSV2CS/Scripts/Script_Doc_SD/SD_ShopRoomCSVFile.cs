using UnityEngine; 
using System.Collections.Generic; 
public static class SD_ShopRoomCSVFile { 
	public static Dictionary<string, Class_ShopRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_ShopRoomCSVFile> ("Json/Document/ShopRoomCSVFile");
	}
