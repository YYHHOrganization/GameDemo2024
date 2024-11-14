using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.RectTransform;

namespace OurGameFramework
{
    public enum AlignType
    {
        Left,
        Right,
        Top,
        Bottom,
        Center,
    }
    
    public class UIScrollView : MonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
    {
        public ScrollRect m_ScrollRect;
        public RectTransform m_Content;
        public Axis m_AxisType; //滑动类型,水平移动/垂直移动
        public AlignType m_AlignType; //布局
        public PivotPresets m_ItemPivot;  //子物体的中心点，自定义枚举，例如TopLeft,
        
        public int m_HorizontalStartSpace;  //水平开始间隔
        public int m_VerticalStartSpace; //垂直开始间隔
        public int m_HorizontalSpace; //水平每个格子间的间隔
        public int m_VerticalSpace; //垂直每个格子间的间隔
        
        public int m_CountOfOtherAxis = 1; //另一个轴上的格子数量
        private int m_maxChooseCnt = 100000; // 默认不做限制，指的是最多可以同时选择几个格子
        
        private IList m_Datas; //数据源，存放真实的物品数据
        private PrefabPool m_PrefabPool;  //对象池
        private List<UILoopItem> m_LoopItems; //存放所有要复用的格子，假设我们有5000个物品，可能只会利用60个格子复用来显示，性能优化
        private Dictionary<int, int> m_SelectIndexs = new Dictionary<int, int>(); //存放每个物品的选择情况，key是物品的index，value是选择的个数
        
        //Gets and Sets
        public Dictionary<int, int> SelectIndexs => m_SelectIndexs;
        public List<UILoopItem> LoopItems => m_LoopItems;
        public int SelectIndex => m_selectIndex;
        
        //其他的私有变量
        private int m_HorizontalCount;
        private int m_VerticalCount;
        private float m_ChildWidth;
        private float m_ChildHeight;
        
        private Rect parentRect;
        private Type m_ItemType;
        private GenshinUserDataStruct m_UserData;
        private bool m_canRangeSelect = false;
        
        private Tweener m_Tweener;
        private int m_CurrentIndex; //记录当前content中左上角的格子的index
        private int m_selectIndex = -1;

        private void Awake()
        {
            m_LoopItems = new List<UILoopItem>();
            //以下回调函数会在滑动滚动条，拖动滚动条的时候调用，以及由于重新实现了点击重新布局的功能，因此点击内部某个按钮的时候也会回调这个函数
            m_ScrollRect.onValueChanged.AddListener(OnValueChanged);
            if (m_AxisType != Axis.Horizontal)
                m_ScrollRect.horizontal = false;
            if (m_AxisType != Axis.Vertical)
                m_ScrollRect.vertical = false;
        }

        public void SetUpList(IList dataList, GameObject prefab, Type type,
            GenshinUserDataStruct userData = null)
        {
            if (dataList == null || prefab == null)
            {
                Release(); //Release的逻辑就是释放逻辑，回收对象池，清空数据等
                return;
            }
            if (m_PrefabPool != null && m_PrefabPool.Prefab != prefab)
            {
                m_PrefabPool.Destroy();
                m_PrefabPool = null;
            }

            if (m_PrefabPool == null)
            {
                m_PrefabPool = PrefabPool.Create(prefab);  //一个ScrollView管理一个对象池
            }
            
            prefab.SetActive(false); //默认挂一个Prefab到Content下面，设置为false
            m_PrefabPool.RecycleUseList();
            m_LoopItems.Clear();
            m_SelectIndexs.Clear(); //当前选择列表清空
            
            RectTransform rect = prefab.GetComponent<RectTransform>();
            m_ChildWidth = rect.rect.width * rect.transform.localScale.x;
            m_ChildHeight = rect.rect.height * rect.transform.localScale.y;
            
            var parent = m_ScrollRect.transform as RectTransform;
            parentRect = parent.rect;
            //以下两个值的计算方法是根据父物体的宽高，减去开始的间隔，除以格子的宽高加上间隔，得到的是水平和垂直方向上的格子数量
            m_HorizontalCount =
                Mathf.CeilToInt((parentRect.width - m_HorizontalStartSpace) / (m_ChildWidth + m_HorizontalSpace));
            m_VerticalCount =
                Mathf.CeilToInt((parentRect.height - m_VerticalStartSpace) / (m_ChildHeight + m_VerticalSpace));
            
            //存储数据类型，数据源，用户数据（即配置）
            m_ItemType = type;
            m_Datas = dataList;
            m_UserData = userData;
            
            GenshinUserDataStruct data = userData;
            //尝试userData转为GenshinUserDataStruct，这个后面会讲，主要针对比如最多选几个，或者是否可以范围选择的逻辑
            if (userData != null)
            {
                m_maxChooseCnt = data.maxSelectCount;
                m_canRangeSelect = data.canRangeSelect;
            }
            m_Content.SetPivot(m_ItemPivot); //自己扩展UI系统，自己锚定对应位置
            Vector2 oldPos = m_Content.anchoredPosition;
            
            if (m_Tweener != null)  //DoTween的逻辑，如果有正在进行的动画，就Kill掉
            {
                m_Tweener.Kill();
                m_Tweener = null;
            }

            if (m_CountOfOtherAxis == 0)  //可以指定另一个轴的格子数量
            {
                //以下的逻辑是根据父物体的宽高，减去开始的间隔，除以格子的宽高加上间隔，得到的是水平和垂直方向上的格子数量（看另一个轴）
                if (m_AxisType == Axis.Horizontal)
                    m_CountOfOtherAxis = Mathf.FloorToInt((parentRect.height - m_VerticalStartSpace) /
                                                          (m_ChildHeight + m_VerticalSpace));
                else
                    m_CountOfOtherAxis = Mathf.FloorToInt((parentRect.width - m_HorizontalStartSpace) /
                                                          (m_ChildWidth + m_HorizontalSpace));

                m_CountOfOtherAxis = Math.Max(1, m_CountOfOtherAxis);
            }
            
            if (m_AxisType == Axis.Horizontal)
                m_VerticalCount = m_CountOfOtherAxis;
            else
                m_HorizontalCount = m_CountOfOtherAxis; //我们背包系统一般是这个逻辑，竖着放的
            int axisCount = Mathf.CeilToInt(dataList.Count * 1.0f / m_CountOfOtherAxis);
            
            switch (m_AxisType)
            {
                case Axis.Horizontal:
                    if (m_AlignType == AlignType.Right)
                    {
                        m_Content.SetAnchor(AnchorPresets.VertStretchRight);
                    }
                    else
                    {
                        m_Content.SetAnchor(AnchorPresets.VertStretchLeft);
                    }

                    m_Content.sizeDelta =
                        new Vector2(
                            axisCount * m_ChildWidth + (axisCount - 1) * m_HorizontalSpace + m_HorizontalStartSpace * 2,
                            0);
                    if (m_AlignType == AlignType.Center)
                    {
                        var viewPort = m_Content.parent as RectTransform;
                        viewPort.anchorMin = new Vector2(0.5f, 0.5f);
                        viewPort.anchorMax = new Vector2(0.5f, 0.5f);
                        viewPort.pivot = new Vector2(0.5f, 0.5f);
                        viewPort.anchoredPosition = Vector2.zero;
                        viewPort.sizeDelta = new Vector2(m_Content.sizeDelta.x, parentRect.height);
                        int verCount = Mathf.FloorToInt((parentRect.height - m_VerticalStartSpace) /
                                                        (m_ChildHeight + m_VerticalSpace));
                        if (verCount > m_Datas.Count)
                        {
                            viewPort.sizeDelta = new Vector2(m_Content.sizeDelta.x,
                                (m_ChildHeight + m_VerticalSpace) * m_Datas.Count - m_VerticalSpace +
                                m_VerticalStartSpace * 2);
                        }
                    }

                    break;
                case Axis.Vertical:
                    if (m_AlignType == AlignType.Bottom)
                    {
                        m_Content.SetAnchor(AnchorPresets.BottomStretch);
                    }
                    else
                    {
                        m_Content.SetAnchor(AnchorPresets.HorStretchTop);
                    }

                    m_Content.sizeDelta = new Vector2(0,
                        axisCount * m_ChildHeight + (axisCount - 1) * m_VerticalSpace + m_VerticalStartSpace * 2);
                    if (m_AlignType == AlignType.Center)
                    {
                        var viewPort = m_Content.parent as RectTransform;
                        viewPort.anchorMin = new Vector2(0.5f, 0.5f);
                        viewPort.anchorMax = new Vector2(0.5f, 0.5f);
                        viewPort.pivot = new Vector2(0.5f, 0.5f);
                        viewPort.anchoredPosition = Vector2.zero;
                        viewPort.sizeDelta = new Vector2(parentRect.width, m_Content.sizeDelta.y);
                        int horCount = Mathf.CeilToInt((parentRect.width - m_HorizontalStartSpace) /
                                                       (m_ChildWidth + m_HorizontalSpace));
                        if (horCount > m_Datas.Count)
                        {
                            viewPort.sizeDelta =
                                new Vector2(
                                    (m_ChildWidth + m_HorizontalSpace) * m_Datas.Count - m_HorizontalSpace +
                                    m_HorizontalStartSpace * 2, m_Content.sizeDelta.y);
                        }
                    }

                    break;
            } //与布局有关，可以暂时忽略
            m_Content.anchoredPosition = Vector2.zero; //跟参考程序相比，忽略isPage属性
            
            m_CurrentIndex = GetCurrentItemIndex();
            UpdateContent(m_CurrentIndex); //更新content，接下来会说
        }

        public void UpdateContent(int index = 0)
        {
            if (m_Datas == null) return;
            int maxCount = 0; //记录一共需要几个格子，要比能展示的格子多两行/两列，不然会穿帮
            switch (m_AxisType)
            {
                case Axis.Horizontal:
                    maxCount = (m_HorizontalCount + 2) * m_CountOfOtherAxis;
                    break;
                case Axis.Vertical:
                    maxCount = (m_VerticalCount + 2) * m_CountOfOtherAxis;
                    break;
            }

            for (int i = 0; i < maxCount; i++)  //maxCount是总的要用的格子数量
            {
                int listIndex = index + i;  //index是左上角的真实index，因此listIndex指的是当前每个格子的真实index
                if (listIndex < m_Datas.Count) //还没有到真实数据的最后一个
                {
                    if (i < m_LoopItems.Count)
                    {
                        if (m_maxChooseCnt == 1)
                        {
                            m_LoopItems[i].UpdateSingleData(m_Datas, listIndex, m_UserData);
                        }
                        else
                        {
                            m_LoopItems[i].UpdateData(m_Datas, listIndex, m_UserData); //listIndex是比如2000这种值
                        }
                    }
                    else
                    {
                        //说明m_LoopItems.Count不够，格子还没创建，创建格子并赋值
                        var go = m_PrefabPool.Get();
                        RectTransform rectTransform = go.transform as RectTransform;
                        rectTransform.SetPivot(m_ItemPivot);
                        switch (m_ItemPivot)
                        {
                            case PivotPresets.TopLeft:
                            case PivotPresets.TopCenter:
                                rectTransform.SetAnchor(AnchorPresets.TopLeft);
                                break;
                            case PivotPresets.TopRight:
                                rectTransform.SetAnchor(AnchorPresets.TopRight);
                                break;
                            case PivotPresets.MiddleLeft:
                            case PivotPresets.MiddleCenter:
                                rectTransform.SetAnchor(AnchorPresets.MiddleLeft);
                                break;
                            case PivotPresets.MiddleRight:
                                rectTransform.SetAnchor(AnchorPresets.MiddleRight);
                                break;
                            case PivotPresets.BottomLeft:
                            case PivotPresets.BottomCenter:
                                rectTransform.SetAnchor(AnchorPresets.BottomLeft);
                                break;
                            case PivotPresets.BottomRight:
                                rectTransform.SetAnchor(AnchorPresets.BottomRight);
                                break;
                            default:
                                break;
                        }

                        rectTransform.SetParent(m_Content);
                        rectTransform.localScale = m_PrefabPool.Prefab.transform.localScale;
                        UILoopItem loopItem = go.GetOrAddComponent(m_ItemType) as UILoopItem;
                        loopItem.UIScrollView = this;
                        loopItem.SetBaseData();
                        m_LoopItems.Add(loopItem);
                        if (m_maxChooseCnt == 1)
                        {
                            loopItem.UpdateSingleData(m_Datas, listIndex, m_UserData);
                        }
                        else
                        {
                            //todo:还没写
                            loopItem.UpdateData(m_Datas, listIndex, m_UserData);
                        }
                        
                    }
                }
                else if (i < m_LoopItems.Count) //到了真实数据的最后一个，但还有空的格子
                {
                    //直接把空格子移出去，也不用更新
                    m_LoopItems[i].transform.localPosition = new Vector3(-10000, -10000);
                }
            }
            while (m_LoopItems.Count > maxCount) //如果格子数量超过了需要的格子数量，就回收多余的格子
            {
                UILoopItem loopItem = m_LoopItems[m_LoopItems.Count - 1];
                m_PrefabPool.Recycle(loopItem.gameObject);
                m_LoopItems.RemoveAt(m_LoopItems.Count - 1);
            }
        }
        
        private void RefleshData()
        {
            //遍历字典
            foreach (var item in m_SelectIndexs)
            {
                RefleshSingleData(item.Key);
            }
        }
        
        void RefleshSingleData(int index)
        {
            GenshinDemoListData data = m_Datas[index] as GenshinDemoListData;
            data.selectCount = m_SelectIndexs[index];
        }
        
        public void AddSelectIndex(int index, int maxCount, bool multiSelectInOneItem = false)
        {
            //dictionary的key是index，value是点击的次数
            if (!m_SelectIndexs.ContainsKey(index))
            {
                m_SelectIndexs.Add(index, 1);
            }
            else
            {
                if (!multiSelectInOneItem) //本身不能多选
                {
                    m_SelectIndexs[index] = 1;
                    return;
                }
                if (m_SelectIndexs[index] < maxCount)
                {
                    m_SelectIndexs[index]++;
                }
            }
        }
        
        public void RemoveSelectIndex(int index, bool multiSelectInOneItem = false) 
        {
            //dictionary的key是index，value是点击的次数,如果点击次数为0，就删除这个index
            if (m_SelectIndexs.ContainsKey(index))
            {
                m_SelectIndexs[index]--;
                if(!multiSelectInOneItem) m_SelectIndexs[index] = 0; //单选情况，删除即为0
                RefleshSingleData(index);
                if (m_SelectIndexs[index] <= 0)
                {
                    m_SelectIndexs.Remove(index);
                }
            }
        }

        private int m_lastSelectIndex;
        
        public void SingleSelect(int index, bool isDiSelect = false)
        {
            if (m_Datas == null) return;
            if (isDiSelect) //是强制减选
            {
                m_SelectIndexs.Remove(index);
                m_lastSelectIndex = -1;
                foreach (var item in m_LoopItems)
                {
                    item.CheckSelect(index, m_Datas[index],true);
                }
                m_selectIndex = -1;
            }
            else
            {
                if (m_lastSelectIndex == index) return;  //这次跟上次选的一样，不用管了
                m_lastSelectIndex = index;
                m_SelectIndexs.Clear();
                m_SelectIndexs.Add(index, 1);
                foreach (var item in m_LoopItems)
                {
                    item.CheckSelect(index, m_Datas[index]);
                }
                m_selectIndex = index;
            }
        }

        
        public void Select(int index, bool isDiSelect = false)
        {
            //index记录真实的索引，isDiSelect记录是否是取消选择,如果是再点一次取消选择的话isDiSelect不起作用
            if (m_Datas == null) return;
            bool singleChoose = m_UserData.maxSelectCount == 1; //是否只能同时选中一个物体
            if (singleChoose) //只能单选
            {
                SingleSelect(index, isDiSelect);
                return;
            }
            GenshinDemoListData listData = m_Datas[index] as GenshinDemoListData;
            bool multiSelectInOneItem = listData.multiSelectInOneItem; //这个物品是否是可多选的（比如连续点选）
            int maxCount = listData.count; //当前持有多少该物体
            if (m_SelectIndexs.ContainsKey(index)) //在选择的队列当中
            {
                int checkIndex = index - GetCurrentItemIndex(); //checkIndex指的是在背包格子中的索引
                bool addSelect = multiSelectInOneItem && !isDiSelect; //多选，且不是显式减选
                if (addSelect)
                {
                    AddSelectIndex(index, maxCount, multiSelectInOneItem);
                    RefleshData();  //将选择个数同步更新到data当中，这样刷新就能看到
                    if (checkIndex >= 0 && checkIndex < m_LoopItems.Count)
                    {
                        m_LoopItems[checkIndex].CheckSelect(index, m_Datas[index], false);
                    }
                }
                else  //说明要减选
                {
                    RemoveSelectIndex(index, multiSelectInOneItem); //这里会顺便UpdateData
                    if (checkIndex >= 0 && checkIndex < m_LoopItems.Count)
                    {
                        m_LoopItems[checkIndex].CheckSelect(index, m_Datas[index], true);
                    }
                }
            }
            else
            {
                //说明现在字典里没有，认为是加选，因为这时正常也不会出现减选按钮的
                if (m_SelectIndexs.Count >= m_maxChooseCnt)  //这里是说选择的格子数量超过了最大值
                {
                    Debug.Log("选择数量超过最大值");
                    //HMessageShowMgr.Instance.ShowMessage();
                    //todo：应该给一个Message提示
                    return;
                }
                AddSelectIndex(index, maxCount, multiSelectInOneItem);
                RefleshData();
                int checkIndex = index - GetCurrentItemIndex();
                if (checkIndex >= 0 && checkIndex < m_LoopItems.Count)
                {
                    m_LoopItems[checkIndex].CheckSelect(index, m_Datas[index], false);
                }
                
            }
        }
        
        public Vector3 GetLocalPositionByIndex(int index)
        {
            float x, y, z;
            x = y = z = 0.0f;

            int remain = index % m_CountOfOtherAxis;
            index /= m_CountOfOtherAxis;
            switch (m_AxisType)
            {
                case Axis.Horizontal:
                    y = -m_VerticalStartSpace - remain * (m_ChildHeight + m_VerticalSpace);
                    switch (m_AlignType)
                    {
                        case AlignType.Center:
                        case AlignType.Left:
                        case AlignType.Top:
                            x = m_HorizontalStartSpace + index * (m_ChildWidth + m_HorizontalSpace);
                            break;
                        case AlignType.Right:
                        case AlignType.Bottom:
                            x = m_HorizontalStartSpace - index * (m_ChildWidth + m_HorizontalSpace);
                            break;
                        default:
                            break;
                    }

                    break;
                case Axis.Vertical:
                    x = m_HorizontalStartSpace + remain * (m_ChildWidth + m_HorizontalSpace);
                    switch (m_AlignType)
                    {
                        case AlignType.Center:
                        case AlignType.Left:
                        case AlignType.Top:
                            y = -m_VerticalStartSpace - index * (m_ChildHeight + m_VerticalSpace);
                            break;
                        case AlignType.Right:
                        case AlignType.Bottom:
                            y = m_VerticalStartSpace + index * (m_ChildHeight + m_VerticalSpace);
                            break;
                        default:
                            break;
                    }

                    break;
            }

            return new Vector3(x, y, z);
        }
        
        private int GetCurrentItemIndex()
        {
            // 这个index指的是界面的index，比如说我是8个一行，那么index就是比如8，16，24这样的，可以看log的值
            //m_Content.anchoredPosition.y 是 content 的位置，随着滑动会变化
            int index = 0;
            switch (m_AxisType)
            {
                case Axis.Horizontal:
                    if (m_AlignType == AlignType.Left && m_Content.anchoredPosition.x >= 0) return 0;
                    if (m_AlignType == AlignType.Right && m_Content.anchoredPosition.x <= 0) return 0;
                    index = Mathf.FloorToInt((Mathf.Abs(m_Content.anchoredPosition.x) - m_HorizontalStartSpace) /
                                             (m_ChildWidth + m_HorizontalSpace)) * m_CountOfOtherAxis;
                    break;
                case Axis.Vertical:
                    if (m_AlignType == AlignType.Bottom && m_Content.anchoredPosition.y >= 0) return 0;
                    if (m_AlignType == AlignType.Top && m_Content.anchoredPosition.y <= 0) return 0;
                    index = Mathf.FloorToInt((Mathf.Abs(m_Content.anchoredPosition.y) - m_VerticalStartSpace) /
                                             (m_ChildHeight + m_VerticalSpace)) * m_CountOfOtherAxis;
                    break;
            }
            
            return Mathf.Max(0, index);
        }

        private void OnValueChanged(Vector2 vec)
        {
            int index = GetCurrentItemIndex();
            if (m_CurrentIndex != index)
            {
                m_CurrentIndex = index;
                UpdateContent(index);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }
        
        public void Release()
        {
            m_LoopItems.Clear();
            if (m_PrefabPool != null)
                m_PrefabPool.RecycleUseList();
        }
    }
}

