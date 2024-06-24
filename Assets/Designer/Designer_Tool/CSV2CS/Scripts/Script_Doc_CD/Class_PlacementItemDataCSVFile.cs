using UnityEngine; 
using System.Collections; 
public class Class_PlacementItemDataCSVFile { 
	public string placementItemDataID { get; set; }    //放置物体ID/37200000开始都是放置物体的类型
	public int _placementItemDataID (){
		int value = int.Parse(placementItemDataID);
		return value;
	}
	public string Describe { get; set; }    //描述
	  public string _Describe (){
		string value = Describe;
		return value;
	}
	public string threeModelAddressableLink { get; set; }    //对应的3d模型的addressable的链接
	  public string _threeModelAddressableLink (){
		string value = threeModelAddressableLink;
		return value;
	}
	public string SizeX { get; set; }    //物品大小的x
	public int _SizeX (){
		int value = int.Parse(SizeX);
		return value;
	}
	public string SizeY { get; set; }    //物品大小的y
	public int _SizeY (){
		int value = int.Parse(SizeY);
		return value;
	}
	public string placementType { get; set; }    //放置类型
	  public string _placementType (){
		string value = placementType;
		return value;
	}
	public string addOffset { get; set; }    //是否添加偏移量，可以有效防止物品阻塞路径
	public int _addOffset (){
		int value = int.Parse(addOffset);
		return value;
	}
	public string minQuantity { get; set; }    //最小数量
	public int _minQuantity (){
		int value = int.Parse(minQuantity);
		return value;
	}
	public string maxQuantity { get; set; }    //最大数量
	public int _maxQuantity (){
		int value = int.Parse(maxQuantity);
		return value;
	}
	public string useProbility { get; set; }    //是否使用概率
	public int _useProbility (){
		int value = int.Parse(useProbility);
		return value;
	}
	public string ProbilityIn100 { get; set; }    //有多少概率出现这个东西
	public int _ProbilityIn100 (){
		int value = int.Parse(ProbilityIn100);
		return value;
	}
	}
	