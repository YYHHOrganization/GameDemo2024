using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OurGameFramework
{
    public class ButtonExtendEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Button button;
        private UIScrollView scrollView;
        private UILoopItem loopItem;
        private bool isDiSelect = false;

        public void SetScrollViewAndIndex(UIScrollView scrollView, UILoopItem loopItem, bool isDiSelect)
        {
            this.scrollView = scrollView;
            this.loopItem = loopItem;
            this.isDiSelect = isDiSelect;
        }
    
        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown  sss");
            button.OnPointerDown(eventData);
            //scrollView.NewSelect(loopItem.Index);
            //长按Button的逻辑，要通知ScrollView，开始长按连续选择
            scrollView.StartLongPressSelect(loopItem.Index, isDiSelect);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp  sss");
            button.OnPointerUp(eventData);
            //长按Button的逻辑，要通知ScrollView，结束长按连续选择
            scrollView.EndLongPressSelect();
        }
    }
}

