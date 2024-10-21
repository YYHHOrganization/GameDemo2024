using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OurGameFramework
{
    public class TestUISystemLauncher : MonoBehaviour
    {
        public GameObject Splash;

        void Start()
        {
            if (Splash == null)
            {
                Splash = GameObject.Find(nameof(Splash));
            }

            StartCoroutine(StartCor());
        }

        IEnumerator StartCor()
        {
            yield return StartCoroutine(ResourceManager.Instance.InitializeAsync());
            yield return UIManager.Instance.InitUIConfig();
            //todo：这里还没实现完，明天再继续弄
            yield return UIManager.Instance.Preload(UIType.UILoadingView);  //这里其实会close掉，但后面会再打开，不用担心
        }
    }
}

