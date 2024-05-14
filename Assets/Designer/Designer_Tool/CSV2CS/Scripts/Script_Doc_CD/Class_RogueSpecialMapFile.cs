using UnityEngine; 
using System.Collections; 
public class Class_RogueSpecialMapFile { 
	public string ID { get; set; }    //SpecialMapID特殊地图id
	  public string _ID (){
		string value = ID;
		return value;
	}
	public string RoomName { get; set; }    //房间名称
	  public string _RoomName (){
		string value = RoomName;
		return value;
	}
	public string ItemIDField { get; set; }    //道具类型可以出现的分号表示这几个都可能出现
	  public string _ItemIDField (){
		string value = ItemIDField;
		return value;
	}
	public string ItemCount { get; set; }    //道具个数（目前最大6）
	public int _ItemCount (){
		int value = int.Parse(ItemCount);
		return value;
	}
	}
	