using UnityEngine; 
using System.Collections; 
public class Class_ShopRoomCSVFile { 
	public string ItemRoomTypeID { get; set; }    //道具房间类型ID66660000开始都是道具房的类型
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
	public string ShopCurrencyID { get; set; }    //交易货币ID
	  public string _ShopCurrencyID (){
		string value = ShopCurrencyID;
		return value;
	}
	public string ShopCurrencyCount { get; set; }    //货币数量，从多少到多少随机一个数字
	  public string _ShopCurrencyCount (){
		string value = ShopCurrencyCount;
		return value;
	}
	public string NPC { get; set; }    //NPC作为店长啥的站在那里
	  public string _NPC (){
		string value = NPC;
		return value;
	}
	}
	