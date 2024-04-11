using UnityEngine; 
using System.Collections; 
public class Class_pp1 { 
	public string EffectName { get; set; }    //EffectName注释
	  public string _EffectName (){
		string value = EffectName;
		return value;
	}
	public string UIShowName { get; set; }    //UIShowName注释
	  public string _UIShowName (){
		string value = UIShowName;
		return value;
	}
	public string EffectFieldName { get; set; }    //EffectFieldName注释
	  public string _EffectFieldName (){
		string value = EffectFieldName;
		return value;
	}
	public string AttributeNames { get; set; }    //AttributeNames注释
	public int _AttributeNames (){
		int value = int.Parse(AttributeNames);
		return value;
	}
	}
	