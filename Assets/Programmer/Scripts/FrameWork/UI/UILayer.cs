using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OurGameFramework
{
    public enum UILayer
    {
        SceneLayer = 1000,
        BackgroundLayer = 2000,
        NormalLayer = 3000,
        InfoLayer = 4000,
        TopLayer = 5000,
        TipLayer = 6000,
        BlackMaskLayer = 7000,
    }
    
    public enum UIEvent
    {
        None,
    }

    public class UILayerLogic
    {
        public UILayer layer;
        public Canvas canvas;
        private int maxOrder;
        private HashSet<int> orders;
        public Stack<UIViewController> openedViews;
        
        public UILayerLogic(UILayer layer, Canvas canvas)
        {
            this.layer = layer;
            this.canvas = canvas;
            maxOrder = (int)layer;
            orders = new HashSet<int>();
            openedViews = new Stack<UIViewController>();
        }
        
        public int PushOrder(UIViewController uIViewController)
        {
            //分配一个Order，放于栈中
            maxOrder += 10;
            orders.Add(maxOrder);
            openedViews.Push(uIViewController);
            return maxOrder;
        }
        
        public void PopOrder(UIViewController closedUI)  //注：这里的push和pop和参考程序是反的，感觉更符合常识
        {
            int order = closedUI.order;
            if (orders.Remove(order))
            {
                // 重新计算最大值
                maxOrder = (int)layer;
                foreach (var item in orders)
                {
                    maxOrder = Mathf.Max(maxOrder, item);
                }

                // 移除界面
                List<UIViewController> list = ListPool<UIViewController>.Get();
                while (openedViews.Count > 0)
                {
                    var view = openedViews.Pop();
                    if (view != closedUI)
                    {
                        list.Add(view);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    openedViews.Push(list[i]);
                }
                ListPool<UIViewController>.Release(list);
            }
        }
        
        public void CloseUI(UIViewController closedUI)
        {
            int order = closedUI.order;
            PopOrder(closedUI);
            closedUI.order = 0;

            if (openedViews.Count > 0)
            {
                var topViewController = openedViews.Peek();
                // 拿到最上层UI，如果被暂停的话，则恢复，
                if (topViewController != null && topViewController.isPause)
                {
                    topViewController.isPause = false;
                    if (topViewController.uiView != null)
                    {
                        topViewController.uiView.OnResume();
                    }
                }
                
                // 暂停和恢复不影响其是否被覆盖隐藏，只要不是最上层UI都应该标记暂停状态
                if (!closedUI.isWindow)  //非窗口界面，如果关闭的是窗口界面，不需要标记暂停状态
                {
                    foreach (var viewController in openedViews)
                    {
                        if (viewController != closedUI
                            && viewController.isOpen
                            && viewController.order < order)
                        {
                            viewController.AddTopViewNum(-1);
                        }
                    }
                }
            }
        }
    }
}
