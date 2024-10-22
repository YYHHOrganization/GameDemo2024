using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace OurGameFramework
{
    public class New_YLoadPanel : UIView
    {
        #region 控件绑定变量声明，自动生成请勿手改
		#pragma warning disable 0649
		[ControlBinding]
		private Button EnterGameButton;
		[ControlBinding]
		private Button SettingButton;
		[ControlBinding]
		private TextMeshProUGUI DescriptionText;
		[ControlBinding]
		private Slider LoadSlider;

		#pragma warning restore 0649
#endregion



        public override void OnInit(UIControlData uIControlData, UIViewController controller)
        {
            base.OnInit(uIControlData, controller);
            EnterGameButton.onClick.AddListener(EnterGameButtonOnClick);
        }
        
        void EnterGameButtonOnClick()
        {
            //进入游戏
            Debug.Log("福报！！");
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
    }
}
