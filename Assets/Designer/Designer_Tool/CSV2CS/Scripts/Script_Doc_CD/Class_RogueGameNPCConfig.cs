using UnityEngine; 
using System.Collections; 
public class Class_RogueGameNPCConfig { 
	public string NPCID { get; set; }    //NPC的ID(唯一)
	  public string _NPCID (){
		string value = NPCID;
		return value;
	}
	public string NPCName { get; set; }    //NPC的名字
	  public string _NPCName (){
		string value = NPCName;
		return value;
	}
	public string NPCAlwaysExits { get; set; }    //NPC是否常驻大世界
	  public string _NPCAlwaysExits (){
		string value = NPCAlwaysExits;
		return value;
	}
	public string NPCExitSituation { get; set; }    //出现时间（all/day/night/special）
	  public string _NPCExitSituation (){
		string value = NPCExitSituation;
		return value;
	}
	public string IsInteractive { get; set; }    //是否为可交互NPC（比如有剧情）
	  public string _IsInteractive (){
		string value = IsInteractive;
		return value;
	}
	public string NPCLocation { get; set; }    //NPC生成的坐标
	  public string _NPCLocation (){
		string value = NPCLocation;
		return value;
	}
	public string NPCAddressable { get; set; }    //NPCAddressable链接
	  public string _NPCAddressable (){
		string value = NPCAddressable;
		return value;
	}
	}
	