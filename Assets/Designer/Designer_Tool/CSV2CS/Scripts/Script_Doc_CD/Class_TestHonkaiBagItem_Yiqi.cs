using UnityEngine; 
using System.Collections; 
public class Class_TestHonkaiBagItem_Yiqi { 
	public string RogueItemID { get; set; }    //"道具ID（从110000开始，到190000都是道具
	public int _RogueItemID (){
		int value = int.Parse(RogueItemID);
		return value;
	}
	public string RogueItemName { get; set; }    //可以进行各种道具的分类）"
	  public string _RogueItemName (){
		string value = RogueItemName;
		return value;
	}
	public string RogueItemChineseName { get; set; }    //道具英文名
	  public string _RogueItemChineseName (){
		string value = RogueItemChineseName;
		return value;
	}
	public string CanBeOverlapped { get; set; }    //道具中文名
	public int _CanBeOverlapped (){
		int value = int.Parse(CanBeOverlapped);
		return value;
	}
	public string ItemDescription { get; set; }    //是否可以堆叠(不能堆叠的将会以多个格子显示)
	  public string _ItemDescription (){
		string value = ItemDescription;
		return value;
	}
	public string ItemFunc { get; set; }    //物品简介
	  public string _ItemFunc (){
		string value = ItemFunc;
		return value;
	}
	public string ItemGetPath { get; set; }    //功效（两件套/四件套）
	  public string _ItemGetPath (){
		string value = ItemGetPath;
		return value;
	}
	public string ItemMaxLevel { get; set; }    //获得途径
	public int _ItemMaxLevel (){
		int value = int.Parse(ItemMaxLevel);
		return value;
	}
	public string ItemCanBeLocked { get; set; }    //道具最高等级
	public int _ItemCanBeLocked (){
		int value = int.Parse(ItemCanBeLocked);
		return value;
	}
	public string ItemCanBeThrown { get; set; }    //道具是否可以上锁
	public int _ItemCanBeThrown (){
		int value = int.Parse(ItemCanBeThrown);
		return value;
	}
	public string ItemAddressableLink { get; set; }    //道具是否可以弃置
	  public string _ItemAddressableLink (){
		string value = ItemAddressableLink;
		return value;
	}
	public string ItemPart { get; set; }    //道具的Addressable链接
	  public string _ItemPart (){
		string value = ItemPart;
		return value;
	}
	public string ItemOverlapMax { get; set; }    //道具所属部位
	public int _ItemOverlapMax (){
		int value = int.Parse(ItemOverlapMax);
		return value;
	}
	public string multiSelectInOneItem { get; set; }    //道具叠加上限
	public int _multiSelectInOneItem (){
		int value = int.Parse(multiSelectInOneItem);
		return value;
	}
	public string ItemStarLevel { get; set; }    //能否在一个格子内加选
	public int _ItemStarLevel (){
		int value = int.Parse(ItemStarLevel);
		return value;
	}
	public string ItemBagKind { get; set; }    //道具星级
	  public string _ItemBagKind (){
		string value = ItemBagKind;
		return value;
	}
	public string showInBagPanel { get; set; }    //道具所属背包种类
	public int _showInBagPanel (){
		int value = int.Parse(showInBagPanel);
		return value;
	}
	public string CanBeRangeSelected { get; set; }    //是否会在背包当中显示
	public int _CanBeRangeSelected (){
		int value = int.Parse(CanBeRangeSelected);
		return value;
	}
	}
	