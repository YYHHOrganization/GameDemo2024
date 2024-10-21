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
    }
}