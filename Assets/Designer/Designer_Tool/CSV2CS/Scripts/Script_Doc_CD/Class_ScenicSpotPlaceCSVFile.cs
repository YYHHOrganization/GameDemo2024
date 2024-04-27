using UnityEngine; 
using System.Collections; 
public class Class_ScenicSpotPlaceCSVFile { 
	public string ScenicSpotPlaceID { get; set; }    //景点ID/44440000开始都是景点的类型
	public int _ScenicSpotPlaceID (){
		int value = int.Parse(ScenicSpotPlaceID);
		return value;
	}
	public string Describe { get; set; }    //描述
	  public string _Describe (){
		string value = Describe;
		return value;
	}
	public string ChineseName { get; set; }    //中文名
	  public string _ChineseName (){
		string value = ChineseName;
		return value;
	}
	public string sequence { get; set; }    //顺序
	  public string _sequence (){
		string value = sequence;
		return value;
	}
	}
	