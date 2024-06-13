using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HWorldUIShowManager : MonoBehaviour
{
    private string vaporizePrefabUILink = "VaporizeReactionUI";
    private GameObject vaporizeUI;

    private void Start()
    {
        vaporizeUI = Addressables.LoadAssetAsync<GameObject>(vaporizePrefabUILink).WaitForCompletion();
    }

    public void ShowElementReactionWorldUIToParent(ElementReaction reaction, Transform parent)
    {
        GameObject showUI = null;
        switch (reaction)
        {
            case ElementReaction.Vaporize:
                showUI = Instantiate(vaporizeUI, parent);
                showUI.transform.DOLocalMove(new Vector3(0, 2.5f, 0), 0.5f).onComplete = () =>
                {
                    Destroy(showUI);
                };
                break;
        }
    }
}
