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
	public string RogueEnemyWanderSpeed { get; set; }    //敌人巡逻速度
	  public float _RogueEnemyWanderSpeed (){
		float value = float.Parse(RogueEnemyWanderSpeed);
		return value;
	}
	public string RogueEnemyWanderBulletLink { get; set; }    //敌人巡逻子弹Prefab的Link
	  public string _RogueEnemyWanderBulletLink (){
		string value = RogueEnemyWanderBulletLink;
		return value;
	}
	public string RogueWanderPlayerSensitivePlayerDis { get; set; }    //敌人巡逻玩家敏感距离
	  public float _RogueWanderPlayerSensitivePlayerDis (){
		float value = float.Parse(RogueWanderPlayerSensitivePlayerDis);
		return value;
	}
	public string RogueEnemyWanderShootInterval { get; set; }    //敌人巡逻射击子弹间隔（不射击的填-1）
	  public float _RogueEnemyWanderShootInterval (){
		float value = float.Parse(RogueEnemyWanderShootInterval);
		return value;
	}
	public string RogueEnemyChaseMaxSpeed { get; set; }    //追击玩家的最大速度
	  public float _RogueEnemyChaseMaxSpeed (){
		float value = float.Parse(RogueEnemyChaseMaxSpeed);
		return value;
	}
	public string RogueEnemyChaseType { get; set; }    //敌人追击类型
	  public string _RogueEnemyChaseType (){
		string value = RogueEnemyChaseType;
		return value;
	}
	public string RogueEnemyChaseShootFunc { get; set; }    //敌人射击子弹的函数(没有填Normal)
	  public string _RogueEnemyChaseShootFunc (){
		string value = RogueEnemyChaseShootFunc;
		return value;
	}
	public string RogueEnemyChaseShootInterval { get; set; }    //敌人追击时的射击间隔
	  public float _RogueEnemyChaseShootInterval (){
		float value = float.Parse(RogueEnemyChaseShootInterval);
		return value;
	}
	public string RogueEnemyChaseBulletPrefab { get; set; }    //敌人追击时射击子弹PrefabLink
	  public string _RogueEnemyChaseBulletPrefab (){
		string value = RogueEnemyChaseBulletPrefab;
		return value;
	}
	public string RogueEnemyStartHealth { get; set; }    //敌人初始生命值
	public int _RogueEnemyStartHealth (){
		int value = int.Parse(RogueEnemyStartHealth);
		return value;
	}
	public string EnemyWanderDamage { get; set; }    //敌人巡逻时伤害
	public int _EnemyWanderDamage (){
		int value = int.Parse(EnemyWanderDamage);
		return value;
	}
	public string EnemyChaseDamage { get; set; }    //敌人追击时伤害
	public int _EnemyChaseDamage (){
		int value = int.Parse(EnemyChaseDamage);
		return value;
	}
	public string IsEnemyReturnWander { get; set; }    //是否追不到敌人就返回Wander
	public int _IsEnemyReturnWander (){
		int value = int.Parse(IsEnemyReturnWander);
		return value;
	}
	public string ReturnWanderTime { get; set; }    //追不到返回Wander的时间
	  public float _ReturnWanderTime (){
		float value = float.Parse(ReturnWanderTime);
		return value;
	}
	public string BeHurtThenToChase { get; set; }    //是否受击进入Chase状态
	public int _BeHurtThenToChase (){
		int value = int.Parse(BeHurtThenToChase);
		return value;
	}
	public string EnemyBulletAttribute { get; set; }    //子弹速度;射程
	  public string _EnemyBulletAttribute (){
		string value = EnemyBulletAttribute;
		return value;
	}
	public string EnemyLevel { get; set; }    //敌人难度等级（10表示最高）
	public int _EnemyLevel (){
		int value = int.Parse(EnemyLevel);
		return value;
	}
	public string EnemyElementType { get; set; }    //敌人的元素类型
	  public string _EnemyElementType (){
		string value = EnemyElementType;
		return value;
	}
	public string isBulletFromPool { get; set; }    //子弹是否采用对象池，如果是则填写对象池表中的id
	public int _isBulletFromPool (){
		int value = int.Parse(isBulletFromPool);
		return value;
	}
	}
	