using System;
using System.Collections;
using UnityEngine;
namespace OurGameFramework
{
    public class LoadingData
    {
        public LoadingFunc loadingFunc;
        public bool isCleanupAsset = false;
    }
    public delegate IEnumerator LoadingFunc(Action<float, string> loadingRefresh);

    /// <summary>
    /// 实际游戏中的loading
    /// </summary>
    public class Loading : SingletonMono<Loading>
    {
        private LoadingData _loadingData;
        private Coroutine _cor;
        public void StartLoading(LoadingFunc loadingFunc, bool isCleanupAsset = false)
        {
            StartLoading(new LoadingData { loadingFunc = loadingFunc, isCleanupAsset = isCleanupAsset });
        }

        public void StartLoading(LoadingData loadingData)
        {
            //开启UI
            UIManager.Instance.Open(UIType.UILoadingView);
            if (loadingData.loadingFunc != null)
            {
                _loadingData = loadingData;
                if (_cor != null)
                {
                    StopCoroutine(_cor);
                }
                _cor = StartCoroutine(CorLoading());
            }
            else
            {
                Debug.LogError("加载错误,没有参数LoadingData！");
            }
        }

        private IEnumerator CorLoading()
        {
            yield return StartCoroutine(_loadingData.loadingFunc(RefreshLoading));
        }
        
        private void RefreshLoading(float loading, string desc)
        {
            // 刷新
            var view = UIManager.Instance.GetView<UILoadingView>(UIType.UILoadingView);
            if (view != null)
            {
                view.SetLoading(loading, desc);
            }
            if (!string.IsNullOrEmpty(desc))
            {
                Debug.Log(desc);
            }
        }
    }
}