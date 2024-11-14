using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace OurGameFramework
{
    public class UIHonkaiSRItem : UILoopItem
    {
        #region 控件绑定变量声明，自动生成请勿手改
#pragma warning disable 0649
        [ControlBinding]
        public Image BgImage;
        [ControlBinding]
        public Button AnItemInBagPanel;
        [ControlBinding]
        public GridLayoutGroup Stars;
        [ControlBinding]
        public Image ItemIcon;
        [ControlBinding]
        public TextMeshProUGUI ItemCountText;

#pragma warning restore 0649
        #endregion
        
        
        GenshinUserDataStruct localUserData;
        private GenshinDemoListData localData; //在HonkaiStarRail中加入Genshin类，这何尝不是一种。。。

        public override void OnInit()
        {
            base.OnInit();
            AnItemInBagPanel.onClick.AddListener(()=>
            {
                UIScrollView.Select(Index);
            });
        }

        public override void SetBaseData()
        {
            base.SetBaseData();
        }

        protected override void OnUpdateData(IList dataList, int index, object userData)
        {
            base.OnUpdateData(dataList, index, userData);
            GenshinDemoListData data = dataList[index] as GenshinDemoListData;
            localData = data;
            if (userData != null)
            {
                localUserData = userData as GenshinUserDataStruct;
            }
            ShowDataLogic(data);
        }
        
        private void SetStars(int starCount)
        {
            for (int i = 0; i < Stars.transform.childCount; i++)
            {
                Stars.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < starCount; i++)
            {
                Stars.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        private void ShowDataLogic(GenshinDemoListData data)
        {
            //把对应的data显示在屏幕上
            int starCount = data.star;
            switch (starCount)
            {
                case 3:
                    BgImage.color = new Color(0.251f,0.8078f,1,0.8196f);
                    break;
                case 4:
                    BgImage.color = new Color(1, 0.251f, 0.847f, 0.8196f);
                    break;
                case 5:
                    BgImage.color = new Color(1,0.741f,0.251f,0.8196f);
                    break;
            }
            SetStars(starCount);
            ItemCountText.text = data.count.ToString();
        }
    }
    
    public class UILoopItem : UISubView  //继承的UISubView需要实现一些OnOpen，OnInit之类的函数
    {
        protected int m_Index;
        protected RectTransform m_RectTransform;
        public int Index => m_Index;
        public UIScrollView UIScrollView { get; set; }
        
        public override void OnInit()
        {
            base.OnInit();
            m_RectTransform = transform as RectTransform;
        }
        
        public virtual void SetBaseData()  //比如有一个子节点什么的，可以在这里面进行赋值
        {
            
        }
        
        public void UpdateSingleData(IList dataList, int index, object userData)  //只能单选的情况
        {
            if (!isInit)
            {
                OnInit();
            }
            m_Index = index;
            m_RectTransform.localPosition = Vector3.zero;
            m_RectTransform.anchoredPosition = UIScrollView.GetLocalPositionByIndex(index);
            //CheckSelect(UIScrollView.SelectIndex, dataList[index]);
            GenshinUserDataStruct genshinUserData = userData as GenshinUserDataStruct;
            OnUpdateData(dataList, index, genshinUserData);
        }
        
        public void UpdateData(IList dataList, int index, object userData)  //可以多选的情况
        {
            if (!isInit)
            {
                OnInit();
            }
            m_Index = index;
            m_RectTransform.localPosition = Vector3.zero;
            m_RectTransform.anchoredPosition = UIScrollView.GetLocalPositionByIndex(index);
            //CheckSelect(UIScrollView.SelectIndex);
            GenshinUserDataStruct genshinUserData = userData as GenshinUserDataStruct;
            OnUpdateData(dataList, index, genshinUserData);
        }
        
        protected virtual void OnUpdateData(IList dataList, int index, object userData)
        {
            
        }
    }
}

