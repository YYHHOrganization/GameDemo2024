using UnityEngine; 
using System.Collections; 
public class Class_BornRoomCSVFile { 
	public string bornRoomTypeID { get; set; }    //出生房间类型ID/66670000开始都是born房的类型
	public int _bornRoomTypeID (){
		int value = int.Parse(bornRoomTypeID);
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
	