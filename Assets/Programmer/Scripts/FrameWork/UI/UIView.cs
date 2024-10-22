using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace OurGameFramework
{
    public class UIView : MonoBehaviour, IBindableUI
    {
        private UIViewController _controller;
        private GameObject _lastSelect;
        private Canvas _canvas;
        public GameObject DefaultSelect;

        public UIViewController Controller => _controller;
        
        public virtual void OnInit(UIControlData uIControlData, UIViewController controller)
        {
            if (uIControlData != null)
            {
                uIControlData.BindDataTo(this);
            }
            _controller = controller;
            _canvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<CanvasScaler>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }
        
        /// <summary>
        /// 恢复
        /// </summary>
        public virtual void OnResume()
        {
            if (DefaultSelect != null)
            {
                //Set the object as selected. Will send an OnDeselect the the old selected object and OnSelect to the new selected object.
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(DefaultSelect);
            }
        }
        
        /// <summary>
        /// 被覆盖
        /// </summary>
        public virtual void OnPause()
        {
        }
        
        /// <summary>
        /// 事件移除
        /// </summary>
        public virtual void OnRemoveListener() { }
        
        /// <summary>
        /// 事件监听
        /// </summary>
        public virtual void OnAddListener() { }

        /// <summary>
        /// 被关闭
        /// </summary>
        public virtual void OnClose()
        {
            OnRemoveListener();

            if (_lastSelect != null && _lastSelect.activeInHierarchy)
            {
                //todo：做这个判断是什么意思？必要性是什么
                //Set the object as selected. Will send an OnDeselect the the old selected object and OnSelect to the new selected object.
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_lastSelect);
            }
        }
        /// <summary>
        /// 被卸载释放
        /// </summary>
        public virtual void OnRelease() { }
        
        /// <summary>
        /// 打开
        /// </summary>
        public virtual void OnOpen(object userData)
        {
            SortOrder();

            _canvas.overrideSorting = true;
            _canvas.sortingOrder = _controller.order;

            OnAddListener();

            _lastSelect = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        }
        
        protected virtual void SortOrder()
        {
            SortOrder(transform, _controller.order + 1);
        }
        
        /// <summary>
        /// 递归的将所有孩子层级设置正确：一些默认摆在UI上的特效等正确分配层级
        /// todo：这个函数的细节暂时不需要太关心，等到后面涉及到特效，SortingGroup之类的再回来看
        /// </summary>
        protected int SortOrder(Transform target, int order)
        {
            var canvas = target.GetComponent<Canvas>();
            if (canvas != null && canvas != _canvas)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = order++;
                canvas.gameObject.layer = Layer.UI;
            }
            var psr = target.GetComponent<ParticleSystemRenderer>();
            if (psr != null)
            {
                psr.sortingOrder = order++;
                psr.gameObject.layer = Layer.UI;
            }
            var sortGroup = target.GetComponent<SortingGroup>();
            if (sortGroup != null)
            {
                sortGroup.sortingOrder = order++;
                sortGroup.gameObject.SetLayerRecursively(Layer.UI);
            }
            var sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = order++;
                sr.gameObject.layer = Layer.UI;
            }
            for (int i = 0; i < target.childCount; i++)
            {
                order = SortOrder(target.GetChild(i), order);
            }
            return order;
        }
    }
}