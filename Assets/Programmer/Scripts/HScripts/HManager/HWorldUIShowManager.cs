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
    private string electroChargedPrefabUILink = "ElectroChargedReactionUI";
    private GameObject electroChargedUI;
    //免疫
    private string immunePrefabUILink = "ImmuneReactionUI";
    private GameObject immuneUI;

    private void Start()
    {
        vaporizeUI = Addressables.LoadAssetAsync<GameObject>(vaporizePrefabUILink).WaitForCompletion();
        electroChargedUI = Addressables.LoadAssetAsync<GameObject>(electroChargedPrefabUILink).WaitForCompletion();
        immuneUI = Addressables.LoadAssetAsync<GameObject>(immunePrefabUILink).WaitForCompletion();
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
            case ElementReaction.ElectroCharged: //感电反应，一共持续5s，每秒出一个感电的UI
                var sequence = DOTween.Sequence();
                for (int i = 0; i < 5; i++)
                {
                    sequence.AppendCallback(() =>
                    {
                        if (parent != null)
                        {
                            GameObject aElectroChargedUI = Instantiate(this.electroChargedUI, parent);
                            aElectroChargedUI.transform.DOLocalMove(new Vector3(-1.5f, 2f, 0), 0.5f).onComplete = () =>
                            {
                                Destroy(aElectroChargedUI);
                            };
                        }
                    });
                    sequence.AppendInterval(1f);
                }

                break;
            case ElementReaction.Immune:
                showUI = Instantiate(immuneUI, parent);
                showUI.transform.DOLocalMove(new Vector3(0, 2.5f, 0), 0.5f).onComplete = () =>
                {
                    Destroy(showUI);
                };
                break;
        }
    }
}
