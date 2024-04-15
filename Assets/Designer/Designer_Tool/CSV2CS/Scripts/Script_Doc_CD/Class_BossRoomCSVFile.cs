using UnityEngine; 
using System.Collections; 
public class Class_BossRoomCSVFile { 
	public string BossRoomTypeID { get; set; }    //boss房间类型ID/66621000开始都是boss房的类型
	public int _BossRoomTypeID (){
		int value = int.Parse(BossRoomTypeID);
		return value;
	}
	public string Describe { get; set; }    //描述
	  public string _Describe (){
		string value = Describe;
		return value;
	}
	public string RoomDifficultyLevel { get; set; }    //房间难度等级
	public int _RoomDifficultyLevel (){
		int value = int.Parse(RoomDifficultyLevel);
		return value;
	}
	public string EnemyIDField { get; set; }    //怪物类型可以出现的分号表示这几个都可能出现
	  public string _EnemyIDField (){
		string value = EnemyIDField;
		return value;
	}
	public string EnemyCountField { get; set; }    //怪物个数/对应前面那个/0.4;9.15的意思是这种怪可能会出现从0-4的个数
	  public string _EnemyCountField (){
		string value = EnemyCountField;
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
	public string DropItemIDField { get; set; }    //有概率掉落的道具，也可能不掉
	  public string _DropItemIDField (){
		string value = DropItemIDField;
		return value;
	}
	public string DropItemIDProbabilityField { get; set; }    //有概率掉落的道具，也可能不掉
	  public string _DropItemIDProbabilityField (){
		string value = DropItemIDProbabilityField;
		return value;
	}
	}
	