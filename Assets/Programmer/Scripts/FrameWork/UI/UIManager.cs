using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace OurGameFramework
{
    public enum UIBlackType
    {
        None,       // 无黑边，全适应
        Height,     // 保持高度填满，两边黑边
        Width,      // 保持宽度填满, 上下黑边
        AutoBlack,  // 自动黑边(选中左右或上下黑边最少的一方)
    }

    public class UIManager : SingletonMono<UIManager>
    {
        public int width = 1920;
        public int height = 1080;
        public UIBlackType uiBlackType = UIBlackType.None;
        private Transform _root;
        private Camera _worldCamera;
        private Camera _uiCamera;
        /// <summary>
        /// 屏幕渐变遮罩
        /// </summary>
        private CanvasGroup _blackMask;
        private CanvasGroup _backgroundMask;
        private Tweener _fadeTweener;
        /// <summary>
        /// 黑边
        /// </summary>
        private RectTransform[] _blacks = new RectTransform[2];

        private Dictionary<UIType, UIViewController> _viewControllers;
        private Dictionary<UILayer, UILayerLogic> _layers;
        private HashSet<UIType> _openViews;
        private HashSet<UIType> _residentViews;
        
        public EventSystem EventSystem { get; private set; }
        public EventController<UIEvent> Event { get; private set; }
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _layers = new Dictionary<UILayer, UILayerLogic>();
            _viewControllers = new Dictionary<UIType, UIViewController>();
            _openViews = new HashSet<UIType>();
            _residentViews = new HashSet<UIType>();
            Event = new EventController<UIEvent>();
            
            //todo:现在worldCamera可能不是Camera.main，后面可以统一一下用Camera.main
            _worldCamera = Camera.main;
            _worldCamera.cullingMask &= int.MaxValue ^ (1 << Layer.UI); //这行代码的意思是让WorldCamera不再渲染UI元素
            
            var root = GameObject.Find("UIRoot");
            if (root == null)
            {
                root = new GameObject("UIRoot");
            }
            
            root.layer = Layer.UI;
            GameObject.DontDestroyOnLoad(root);
            _root = root.transform;
            
            var camera = GameObject.Find("UICamera");
            if (camera == null)
            {
                camera = new GameObject("UICamera");
            }
            _uiCamera = camera.GetOrAddComponent<Camera>();
            _uiCamera.cullingMask = 1 << Layer.UI;  //uiCamera只渲染UI层
            _uiCamera.transform.SetParent(_root);
            _uiCamera.orthographic = true;
            _uiCamera.clearFlags = CameraClearFlags.Depth;    //Clear only the depth buffer.This will leave colors from the previous frame or whatever was displayed before.
            
            EventSystem = EventSystem.current;
            var layers = Enum.GetValues(typeof(UILayer));
            foreach (UILayer layer in layers)
            {
                bool is3d = (layer == UILayer.SceneLayer);
                //新建一个Canvas在UIRoot的下面
                Canvas layerCanvas = UIExtension.CreateLayerCanvas(layer, is3d, _root, is3d ? _worldCamera : _uiCamera, width, height);
                UILayerLogic uILayerLogic = new UILayerLogic(layer, layerCanvas);
                _layers.Add(layer, uILayerLogic);
            }
            //以下的两行用于创建黑边，暂时不需要太过关注
            _blackMask = UIExtension.CreateBlackMask(_layers[UILayer.BlackMaskLayer].canvas.transform);
            _backgroundMask = UIExtension.CreateBlackMask(_layers[UILayer.BackgroundLayer].canvas.transform);
        }
        
        private void Update()
        {
            // TODO 不应该Update设置应该放在屏幕状态变动事件里
            ChangeOrCreateBlack();
        }

        /// <summary>
        /// 创建或者调整黑边，需间隔触发，由于有些设备屏幕是可以转动，是动态的
        /// </summary>
        private void ChangeOrCreateBlack()
        {
            if (_layers == null) return;
            var parent = _layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
            var uIBlackType = GetUIBlackType();
            switch (uIBlackType)
            {
                case UIBlackType.Height:
                    // 高度适配时的左右黑边
                    var rect = _blacks[0];
                    if (rect == null)
                    {
                        _blacks[0] = rect = UIExtension.CreateBlackMask(parent, 1, "right").transform as RectTransform;
                    }
                    else if (Mathf.Abs(rect.anchoredPosition.x * 2 + parent.rect.width - width) < 1)
                    {
                        return;
                    }
                    rect.pivot = new Vector2(0, 0.5f);
                    rect.anchorMin = new Vector2(1, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.sizeDelta = new Vector2(Mathf.Abs(width - parent.rect.width), 0);
                    rect.anchoredPosition = new Vector2((width - parent.rect.width) / 2, 0);

                    rect = _blacks[1];
                    if (rect == null)
                    {
                        _blacks[1] = rect = UIExtension.CreateBlackMask(parent, 1, "left").transform as RectTransform;
                    }
                    rect.pivot = new Vector2(1, 0.5f);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(0, 1);
                    rect.sizeDelta = new Vector2(Mathf.Abs(width - parent.rect.width), 0);
                    rect.anchoredPosition = new Vector2(-(width - parent.rect.width) / 2, 0);
                    break;
                case UIBlackType.Width:
                    // 宽度适配时的上下黑边
                    rect = _blacks[0];
                    if (rect == null)
                    {
                        _blacks[0] = rect = UIExtension.CreateBlackMask(parent, 1, "top").transform as RectTransform;
                    }
                    else if (Mathf.Abs(rect.anchoredPosition.y * 2 + parent.rect.height - height) < 1)
                    {
                        return;
                    }
                    rect.pivot = new Vector2(0.5f, 0);
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.sizeDelta = new Vector2(0, Mathf.Abs(height - parent.rect.height));
                    rect.anchoredPosition = new Vector2(0, (height - parent.rect.height) / 2);

                    rect = _blacks[1];
                    if (rect == null)
                    {
                        _blacks[1] = rect = UIExtension.CreateBlackMask(parent, 1, "bottom").transform as RectTransform;
                    }
                    rect.pivot = new Vector2(0.5f, 1);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 0);
                    rect.sizeDelta = new Vector2(0, Mathf.Abs(height - parent.rect.height));
                    rect.anchoredPosition = new Vector2(0, -(height - parent.rect.height) / 2);
                    break;
                default:
                    break;
            }
        }
        
        public UIBlackType GetUIBlackType()
        {
            var uIBlackType = uiBlackType;
            if (uIBlackType == UIBlackType.AutoBlack)
            {
                var parent = _layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
                float widthDis = Mathf.Abs(width - parent.rect.width);
                float heightDis = Mathf.Abs(height - parent.rect.height);

                if (widthDis < 1 && heightDis < 1)
                    uIBlackType = UIBlackType.None;
                else if (widthDis > heightDis)
                    uIBlackType = UIBlackType.Height;
                else
                    uIBlackType = UIBlackType.Width;
            }
            return uIBlackType;
        }

        public Rect GetSafeArea()
        {
            Rect rect = Screen.safeArea;
            if (uiBlackType == UIBlackType.Width)
            {
                var parent = _layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
                float blackArea = Mathf.Abs(height - parent.rect.height) / 2;
                rect.yMin = Mathf.Max(0, rect.yMin - blackArea);
                rect.yMax = Mathf.Min(rect.yMax + blackArea, Screen.height);
            }
            else if (uiBlackType == UIBlackType.Height)
            {
                var parent = _layers[UILayer.BackgroundLayer].canvas.transform as RectTransform;
                float blackArea = Mathf.Abs(width - parent.rect.width) / 2;
                rect.xMin = Mathf.Max(0, rect.xMin - blackArea);
                rect.xMax = Mathf.Min(rect.xMax + blackArea, Screen.width);
            }
            return rect;
        }
        
        public void EnableBackgroundMask(bool enable)
        {
            _backgroundMask.alpha = enable ? 1 : 0;
        }
        
        public AsyncOperationHandle Preload(UIType type)
        {
            if (!_viewControllers.TryGetValue(type, out var controller))
            {
                Debug.LogErrorFormat("未配置uiType:{0}， 请检查UIConfig.cs！", type.ToString());
                return default;
            }
            return controller.Load();
        }

        public AsyncOperationHandle InitUIConfig()
        {
            // 初始化需要加载所有UI的配置
            return UIConfig.GetAllConfigs((list) =>
            {
                foreach (var cfg in list)
                {
                    if (_viewControllers.ContainsKey(cfg.uiType))
                    {
                        Debug.LogErrorFormat("存在相同的uiType:{0}， 请检查UIConfig是否重复！", cfg.uiType.ToString());
                        continue;
                    }

                    _viewControllers.Add(cfg.uiType, new UIViewController
                    {
                        uiPath = cfg.path,
                        uiType = cfg.uiType,
                        uiLayer = _layers[cfg.uiLayer],
                        uiViewType = cfg.viewType,
                        isWindow = cfg.isWindow,
                    });
                }
            });
        }
    }
}
