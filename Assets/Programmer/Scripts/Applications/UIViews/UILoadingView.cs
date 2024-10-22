using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace OurGameFramework
{
    public class UILoadingView : UIView
    {
        #region 控件绑定变量声明，自动生成请勿手改
		#pragma warning disable 0649
		[ControlBinding]
		private Slider Slider;
		[ControlBinding]
		private TextMeshProUGUI TextDes;
		[ControlBinding]
		private TextMeshProUGUI TextValue;

		#pragma warning restore 0649
#endregion

        public override void OnInit(UIControlData uIControlData, UIViewController controller)
        {
            base.OnInit(uIControlData, controller);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
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
        
        public void SetLoading(float value, string desc)
        {
            Slider.DOValue(value, 0.3f);
            TextDes.text = desc;
        }
    }
}
