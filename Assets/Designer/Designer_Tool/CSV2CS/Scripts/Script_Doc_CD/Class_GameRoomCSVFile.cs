using UnityEngine; 
using System.Collections; 
public class Class_GameRoomCSVFile { 
	public string gameRoomTypeID { get; set; }    //game房间类型ID/66630000开始都是game房的类型
	public int _gameRoomTypeID (){
		int value = int.Parse(gameRoomTypeID);
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
	