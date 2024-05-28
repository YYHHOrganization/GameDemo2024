using UnityEngine; 
using System.Collections; 
public class Class_RoguePetCSVFile { 
	public string petID { get; set; }    //宠物id
	  public string _petID (){
		string value = petID;
		return value;
	}
	public string PetNum { get; set; }    //宠物编号
	  public string _PetNum (){
		string value = PetNum;
		return value;
	}
	public string ChineseName { get; set; }    //敌人中文名
	  public string _ChineseName (){
		string value = ChineseName;
		return value;
	}
	public string addressableLink { get; set; }    //addressable链接
	  public string _addressableLink (){
		string value = addressableLink;
		return value;
	}
	public string AttackType { get; set; }    //宠物攻击类型
	  public string _AttackType (){
		string value = AttackType;
		return value;
	}
	public string WanderSpeed { get; set; }    //敌人巡逻速度
	  public float _WanderSpeed (){
		float value = float.Parse(WanderSpeed);
		return value;
	}
	public string WanderBulletLink { get; set; }    //敌人巡逻子弹Prefab的Link
	  public string _WanderBulletLink (){
		string value = WanderBulletLink;
		return value;
	}
	public string WanderPlayerSensitivePlayerDis { get; set; }    //敌人巡逻玩家敏感距离
	  public float _WanderPlayerSensitivePlayerDis (){
		float value = float.Parse(WanderPlayerSensitivePlayerDis);
		return value;
	}
	public string WanderShootInterval { get; set; }    //敌人巡逻射击子弹间隔（不射击的填-1）
	  public float _WanderShootInterval (){
		float value = float.Parse(WanderShootInterval);
		return value;
	}
	public string ChaseMaxSpeed { get; set; }    //追击玩家的最大速度
	  public float _ChaseMaxSpeed (){
		float value = float.Parse(ChaseMaxSpeed);
		return value;
	}
	public string ChaseType { get; set; }    //敌人追击类型
	  public string _ChaseType (){
		string value = ChaseType;
		return value;
	}
	public string ChaseShootFunc { get; set; }    //敌人射击子弹的函数(没有填Normal)
	  public string _ChaseShootFunc (){
		string value = ChaseShootFunc;
		return value;
	}
	public string ChaseShootInterval { get; set; }    //敌人追击时的射击间隔
	  public float _ChaseShootInterval (){
		float value = float.Parse(ChaseShootInterval);
		return value;
	}
	public string ChaseBulletPrefab { get; set; }    //敌人追击时射击子弹PrefabLink
	  public string _ChaseBulletPrefab (){
		string value = ChaseBulletPrefab;
		return value;
	}
	public string StartHealth { get; set; }    //敌人初始生命值
	public int _StartHealth (){
		int value = int.Parse(StartHealth);
		return value;
	}
	public string WanderDamage { get; set; }    //敌人巡逻时伤害
	public int _WanderDamage (){
		int value = int.Parse(WanderDamage);
		return value;
	}
	public string ChaseDamage { get; set; }    //敌人追击时伤害
	public int _ChaseDamage (){
		int value = int.Parse(ChaseDamage);
		return value;
	}
	public string IsReturnWander { get; set; }    //是否追不到敌人就返回Wander
	public int _IsReturnWander (){
		int value = int.Parse(IsReturnWander);
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
	public string BulletAttribute { get; set; }    //子弹速度;射程
	  public string _BulletAttribute (){
		string value = BulletAttribute;
		return value;
	}
	public string Level { get; set; }    //敌人难度等级（10表示最高）
	}
	