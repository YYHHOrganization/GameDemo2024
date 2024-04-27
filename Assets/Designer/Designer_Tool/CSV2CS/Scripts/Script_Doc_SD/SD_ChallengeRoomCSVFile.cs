using UnityEngine; 
using System.Collections.Generic; 
public static class SD_ChallengeRoomCSVFile { 
	public static Dictionary<string, Class_ChallengeRoomCSVFile> Class_Dic = JsonReader.ReadJson<Class_ChallengeRoomCSVFile> ("Json/Document/ChallengeRoomCSVFile");
	}
