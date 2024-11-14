using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OurGameFramework
{
    public class TestStartNewBagPanel : MonoBehaviour
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
            yield return UIManager.Instance.Preload(UIType.HBagPanelNew);  //这里其实会close掉，但后面会再打开，不用担心
            //yield return UIManager.Instance.Preload(UIType.YLoadPanel);
            //Loading.Instance.StartLoading(EnterGameCor);
            if (Splash != null)
            {
                Splash.SetActive(false);
            }
            UIManager.Instance.Open(UIType.HBagPanelNew);
        }

    }
}

