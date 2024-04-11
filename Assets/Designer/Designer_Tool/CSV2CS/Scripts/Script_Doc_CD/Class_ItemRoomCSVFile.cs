using UnityEngine; 
using System.Collections; 
public class Class_ItemRoomCSVFile { 
	public string ItemRoomTypeID { get; set; }    //道具房间类型ID66610000开始都是道具房的类型
	public int _ItemRoomTypeID (){
		int value = int.Parse(ItemRoomTypeID);
		return value;
	}
	public string Describe { get; set; }    //描述
	  public string _Describe (){
		string value = Describe;
		return value;
	}
	public string ItemCount { get; set; }    //道具个数
	public int _ItemCount (){
		int value = int.Parse(ItemCount);
		return value;
	}
	public string ItemIDField { get; set; }    //道具类型可以出现的分号表示这几个都可能出现
	  public string _ItemIDField (){
		string value = ItemIDField;
		return value;
	}
	}
	