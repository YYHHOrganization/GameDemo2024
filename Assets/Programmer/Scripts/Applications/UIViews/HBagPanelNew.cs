using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace OurGameFramework
{
    //类型的Enum
    public enum GenshinDemoListType
    {
        Weapon,
        Mineral, //矿石
        MissionItem, //任务物品
        Food, //食物
        Material, //材料
        Artifact, //圣遗物
        Other, //其他
    }
    public class GenshinDemoListData
    {
        public int id;
        public string name;
        public GenshinDemoListType type;

        public float getTime; //获取时间
        public int count; //数量
        public int star; //星级

        public int level;
        //...其他变量，比如获取时间等
        public int selectCount = 0; //当前某个物品选择的数量
        public bool multiSelectInOneItem = false; //是否在一个格子中显示多个数量
    }

    public class GenshinUserDataStruct
    {
        //这个类存储相关打开背包的逻辑，比如可选择物品的个数限制，物体是否要叠加显示，是否能显示X键取消勾选等
        public int maxSelectCount = 1; //最大可选择种类数量
        public bool isShowX = true; //是否显示X键
        public bool isCanOverlap = false; //是否能叠加显示,比如100把武器，要在一个格子中显示还是在多个格子中显示
        public bool canRangeSelect = false; //是否可以范围选择
    }
    public class HBagPanelNew : UIView
    {
        #region 控件绑定变量声明，自动生成请勿手改
		#pragma warning disable 0649
		[ControlBinding]
		private ScrollRect BagScrollView;
		[ControlBinding]
		private TextMeshProUGUI ItemName;
		[ControlBinding]
		private Button ExitButton;

		#pragma warning restore 0649
#endregion
        
        private UIScrollView scrollView;
        public GameObject itemPrefab;


        public override void OnInit(UIControlData uIControlData, UIViewController controller)
        {
            base.OnInit(uIControlData, controller);
            scrollView = BagScrollView.gameObject.GetComponent<UIScrollView>();
            itemPrefab = BagScrollView.transform.Find("Viewport/Content/AnItemInBagPanel").gameObject;
            //绑定一些监听回调函数
        }
        
        private void SetGenshinDataToUIScrollViewWeapon()
        {
            //关于武器的逻辑，可以多选，不会叠加显示
            List<GenshinDemoListData> dataList = new List<GenshinDemoListData>();
            for (int i = 0; i < 5000; i++)
            {
                GenshinDemoListData data = new GenshinDemoListData();
                data.id = i;
                data.name = i.ToString();
                //count 随机100~1000
                data.count = UnityEngine.Random.Range(100, 1000);
                data.star = UnityEngine.Random.Range(3, 6); 
                data.level = 80;
                data.type = GenshinDemoListType.Weapon;
                data.multiSelectInOneItem = false;
                dataList.Add(data);
            }
            //创建对应的UserData，方便格子的逻辑处理
            GenshinUserDataStruct userData = new GenshinUserDataStruct();
            userData.maxSelectCount = 1000; //可以最多选择10000个
            userData.isShowX = true; //显示X键，只做UI上的显示
            userData.isCanOverlap = true; //不叠加显示
            userData.canRangeSelect = true; //可以范围选择
        
            scrollView.SetUpList(dataList, itemPrefab, typeof(UIHonkaiSRItem), userData);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            //理论上我们应该拿到数据库的数据，但为了简单起见，我们直接生成一些数据上去
            SetGenshinDataToUIScrollViewWeapon();
        }

        public override void OnAddListener()
        {
            base.OnAddListener();
        }

        public override void OnRemoveListener()
        {
            base.OnRemoveListener();
        }

        public override void OnClose()
        {
            base.OnClose();
        }
    }
}
