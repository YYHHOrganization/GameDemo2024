using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        public RawImage SelectedBtn;
        [ControlBinding]
        public Button UnSelectBtn;
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
                UIScrollView.Select(Index, false);
            });
            UnSelectBtn.onClick.AddListener(() =>  
            {
                UIScrollView.Select(Index, true); //明确减选
            });
            UnSelectBtn.gameObject.SetActive(false);
        }
        

        public override void SetBaseData()
        {
            base.SetBaseData();
        }

        public override void CheckSelect(int index, object data, bool isRemove = false)
        {
            base.CheckSelect(index, data, isRemove);
            //isRemove表示是否在显式减选物体，还要看该物体是不是多选的
            GenshinDemoListData genshinData = data as GenshinDemoListData;
            int selectCount = genshinData.selectCount;
            if(localUserData==null) localUserData = new GenshinUserDataStruct();
            if (isRemove == false)
            {
                bool isShow = (index == Index);
                SelectedBtn.gameObject.SetActive(isShow);
                //if(!isUpdate) SelectedBtn.DOFade(isShow ? 1 : 0, 0.15f);
                UnSelectBtn.gameObject.SetActive(isShow && localUserData.isShowX);
                //ItemCountText.gameObject.SetActive(isShow && selectCount > 0 && genshinData.multiSelectInOneItem);
            }
            else  //是要移除物品
            {
                bool isShow = selectCount > 0 && (index == Index);
                SelectedBtn.gameObject.SetActive(isShow);
                //if(!isUpdate) SelectedBtn.DOFade(isShow ? 1 : 0, 0.15f);
                UnSelectBtn.gameObject.SetActive(isShow && localUserData.isShowX);
                //ItemCountText.gameObject.SetActive(isShow && genshinData.multiSelectInOneItem);
            }
            
            if (!genshinData.multiSelectInOneItem) return;
            if (selectCount >= 1) //以下针对可叠加显示的物品
            {
                ItemCountText.text = genshinData.selectCount.ToString() + '/' + genshinData.count.ToString();
            }
            else
            {
                ItemCountText.text = genshinData.count.ToString();
            }
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
        
        public virtual void CheckSelect(int index, object data, bool isRemove = false)
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
            CheckSelect(UIScrollView.SelectIndex, dataList[index],false);
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
            if(UIScrollView.SelectIndexs.ContainsKey(index))
            {
                CheckSelect(m_Index, dataList[index], false);
            }
            else
            {
                CheckSelect(-1, dataList[index],false);
            }
            OnUpdateData(dataList, index, genshinUserData);
        }
        
        protected virtual void OnUpdateData(IList dataList, int index, object userData)
        {
            
        }
    }
}

