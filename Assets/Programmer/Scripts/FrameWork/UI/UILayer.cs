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
    }
}
