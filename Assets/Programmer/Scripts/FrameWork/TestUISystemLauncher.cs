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
            //yield return UIManager.Instance.InitUIConfig();
        }
    }
}

