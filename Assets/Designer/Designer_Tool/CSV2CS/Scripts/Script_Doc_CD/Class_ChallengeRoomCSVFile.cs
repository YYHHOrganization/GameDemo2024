using UnityEngine; 
using System.Collections; 
public class Class_ChallengeRoomCSVFile { 
	public string challengeRoomTypeID { get; set; }    //挑战房间类型ID/66610000开始都是challenge房的类型
	public int _challengeRoomTypeID (){
		int value = int.Parse(challengeRoomTypeID);
		return value;
	}
	public string Describe { get; set; }    //描述
	  public string _Describe (){
		string value = Describe;
		return value;
	}
	public string OtherItemIDField { get; set; }    //其他物品
	  public string _OtherItemIDField (){
		string value = OtherItemIDField;
		return value;
	}
	public string OtherItemCountField { get; set; }    //其他物品个数
	  public string _OtherItemCountField (){
		string value = OtherItemCountField;
		return value;
	}
	}
	