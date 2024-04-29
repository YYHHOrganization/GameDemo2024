using UnityEngine; 
using System.Collections; 
public class Class_RogueCommonItemCSVFile { 
	public string ID { get; set; }    //物品ID，从30000000开始，到31000000都是物品
	public int _ID (){
		int value = int.Parse(ID);
		return value;
	}
	public string Name { get; set; }    //英文名
	  public string _Name (){
		string value = Name;
		return value;
	}
	public string ChineseName { get; set; }    //中文名
	  public string _ChineseName (){
		string value = ChineseName;
		return value;
	}
	public string addressableLink { get; set; }    //addressable链接
	  public string _addressableLink (){
		string value = addressableLink;
		return value;
	}
	public string GeneratePlace { get; set; }    //出现位置
	  public string _GeneratePlace (){
		string value = GeneratePlace;
		return value;
	}
	public string CouldBrokenAndGenerateItem { get; set; }    //是否可破碎，如果可以，填写其后面的破碎爆出物品和破碎爆出物品的概率
	public int _CouldBrokenAndGenerateItem (){
		int value = int.Parse(CouldBrokenAndGenerateItem);
		return value;
	}
	public string BrokenAndGenerateItemList { get; set; }    //破碎爆出物品
	  public string _BrokenAndGenerateItemList (){
		string value = BrokenAndGenerateItemList;
		return value;
	}
	public string GenerateItemProbabilityList { get; set; }    //爆出物品的概率
	  public string _GenerateItemProbabilityList (){
		string value = GenerateItemProbabilityList;
		return value;
	}
	}
	