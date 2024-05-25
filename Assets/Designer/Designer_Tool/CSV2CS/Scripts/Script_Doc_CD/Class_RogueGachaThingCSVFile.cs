using UnityEngine; 
using System.Collections; 
public class Class_RogueGachaThingCSVFile { 
	public string RogueGachaItemId { get; set; }    //"抽卡物品对应的ID
	public int _RogueGachaItemId (){
		int value = int.Parse(RogueGachaItemId);
		return value;
	}
	public string Describe { get; set; }    //从89900000开始"
	  public string _Describe (){
		string value = Describe;
		return value;
	}
	public string DescribeInLife { get; set; }    //描述
	  public string _DescribeInLife (){
		string value = DescribeInLife;
		return value;
	}
	public string RogueGachaItemStar { get; set; }    //描述（isLife，生活中的抽卡物品，玩积分用的，比如山姆）
	public int _RogueGachaItemStar (){
		int value = int.Parse(RogueGachaItemStar);
		return value;
	}
	public string RogueGachaItemType { get; set; }    //道具对应的星级（3，4，5）
	  public string _RogueGachaItemType (){
		string value = RogueGachaItemType;
		return value;
	}
	public string RogueGachaItemFindID { get; set; }    //道具实际类型，会对应去哪个表去找，比如RogueItem
	  public string _RogueGachaItemFindID (){
		string value = RogueGachaItemFindID;
		return value;
	}
	public string IsCurrentGachaItem { get; set; }    //道具实际类型对应的ID，会对应去哪个表去找，比如RogueItem
	public int _IsCurrentGachaItem (){
		int value = int.Parse(IsCurrentGachaItem);
		return value;
	}
	public string IsCurrentGachaUpItem { get; set; }    //是否为当期卡池中有的东西（1表示是，0表示不是）
	public int _IsCurrentGachaUpItem (){
		int value = int.Parse(IsCurrentGachaUpItem);
		return value;
	}
	public string IsChangzhuThing { get; set; }    //是否为当期卡池Up的物品（1表示是）
	public int _IsChangzhuThing (){
		int value = int.Parse(IsChangzhuThing);
		return value;
	}
	public string ItemLink { get; set; }    //是否为常驻池的物品（是：1）
	  public string _ItemLink (){
		string value = ItemLink;
		return value;
	}
	}
	