using UnityEngine; 
using System.Collections; 
public class Class_ObjectPoolCSVFile { 
	public string adventureRoomTypeID { get; set; }    //对象池存放的物体ID/33300000开始都是
	public int _adventureRoomTypeID (){
		int value = int.Parse(adventureRoomTypeID);
		return value;
	}
	public string Describe { get; set; }    //描述
	  public string _Describe (){
		string value = Describe;
		return value;
	}
	public string addressableLink { get; set; }    //addressable链接
	  public string _addressableLink (){
		string value = addressableLink;
		return value;
	}
	public string originNumber { get; set; }    //场景中对象池存储数量
	public int _originNumber (){
		int value = int.Parse(originNumber);
		return value;
	}
	public string bulletHitId { get; set; }    //若是子弹则这里存放其对应击中特效id
	  public string _bulletHitId (){
		string value = bulletHitId;
		return value;
	}
	public string isRecallable { get; set; }    //是否可以时间回溯
	public int _isRecallable (){
		int value = int.Parse(isRecallable);
		return value;
	}
	}
	