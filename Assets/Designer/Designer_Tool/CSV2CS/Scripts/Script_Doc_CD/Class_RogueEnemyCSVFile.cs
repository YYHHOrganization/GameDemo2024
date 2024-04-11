using UnityEngine; 
using System.Collections; 
public class Class_RogueEnemyCSVFile { 
	public string enemyID { get; set; }    //敌人ID，从70000000开始，到71000000都是敌人
	public int _enemyID (){
		int value = int.Parse(enemyID);
		return value;
	}
	public string RogueEnemyName { get; set; }    //敌人英文名
	  public string _RogueEnemyName (){
		string value = RogueEnemyName;
		return value;
	}
	public string RogueEnemyChineseName { get; set; }    //敌人中文名
	  public string _RogueEnemyChineseName (){
		string value = RogueEnemyChineseName;
		return value;
	}
	public string addressableLink { get; set; }    //addressable链接
	  public string _addressableLink (){
		string value = addressableLink;
		return value;
	}
	public string RogueEnemyWanderType { get; set; }    //敌人巡逻类型
	  public string _RogueEnemyWanderType (){
		string value = RogueEnemyWanderType;
		return value;
	}
	}
	