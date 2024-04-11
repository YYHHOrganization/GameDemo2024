using UnityEngine; 
using System.Collections; 
public class Class_RogueRoomFile { 
	public string ID { get; set; }    //房间类型ID为66600xxx
	public int _ID (){
		int value = int.Parse(ID);
		return value;
	}
	public string RoomType { get; set; }    //房间类型，对应那个enum，后面还可以引入别的
	  public string _RoomType (){
		string value = RoomType;
		return value;
	}
	public string RoomCount { get; set; }    //个数
	public int _RoomCount (){
		int value = int.Parse(RoomCount);
		return value;
	}
	public string RoomName { get; set; }    //房间名称
	  public string _RoomName (){
		string value = RoomName;
		return value;
	}
	public string SourceType { get; set; }    //房间内容来源表
	  public string _SourceType (){
		string value = SourceType;
		return value;
	}
	public string RoomContent { get; set; }    //房间内容，a:b:c|表示对应后面的物品ID，从索引a到索引a+b，抽取c个作为奖励
	  public string _RoomContent (){
		string value = RoomContent;
		return value;
	}
	public string ItemID { get; set; }    //物品ID，后面用:表示数量
	  public string _ItemID (){
		string value = ItemID;
		return value;
	}
	public string FixedItemID { get; set; }    //房间固定开出物品ID
	public int _FixedItemID (){
		int value = int.Parse(FixedItemID);
		return value;
	}
	public string FixedItemCount { get; set; }    //房间固定开出物品数量
	public int _FixedItemCount (){
		int value = int.Parse(FixedItemCount);
		return value;
	}
	}
	