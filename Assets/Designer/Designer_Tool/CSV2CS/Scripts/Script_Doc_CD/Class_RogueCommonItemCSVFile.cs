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
	}
	