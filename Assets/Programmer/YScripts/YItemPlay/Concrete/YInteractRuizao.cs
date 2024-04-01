using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class YInteractRuizao : YIInteractiveGroup
{
    Transform showRuiZaoCorePlace;
    GameObject RuizaoCoreCubeShow;
    GameObject RuizaoCoreCubeShowOver;
    private GameObject RuizaoMainGO;
    public void Start()
    {
        base.Start();
        showRuiZaoCorePlace = this.transform.Find("RuiZaoPlace");
        RuizaoCoreCubeShow = this.transform.Find("RuizaoCoreCubeShow").gameObject;
        RuizaoCoreCubeShow.SetActive(true);
        RuizaoCoreCubeShowOver = this.transform.Find("RuizaoCoreCubeShowOver").gameObject;
        RuizaoCoreCubeShowOver.SetActive(false);

        YTriggerEvents.OnEnterNewLevel += EnterNewLevel;
    }
    
    public override void SetResultOn()
    {
        YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, triggers[0],null);//如果是靠近宝箱打开 只会有一个trigger
        Debug.Log("开启");

        RuizaoCoreCubeShow.SetActive(false);

        string ruizaoLink = "YRuiZaoLiuHeInDesert";//""YRuiZaoLiuHe";
        var op = Addressables.InstantiateAsync(ruizaoLink, showRuiZaoCorePlace);
        RuizaoMainGO = op.WaitForCompletion();
        
        YRuiZaoScripts ruizaoScripts = RuizaoMainGO.GetComponentInChildren<YRuiZaoScripts>();
        ruizaoScripts.SetRuiZaoOn(this);
    }

    public void ExitAndWin()
    {
        if (RuizaoMainGO != null)
        {
            // RuizaoMainGO.SetActive(false);
            Destroy(RuizaoMainGO);
        }
        if (RuizaoCoreCubeShowOver != null)
        {
            RuizaoCoreCubeShowOver.SetActive(true);
        }
        
        isOnce = true;
        
        //triggers[0].SetActive(false);
    }

    public void ExitAndNoWin()
    {
        //应该是让他回归原样 可以重新开始的状态
        if (RuizaoMainGO != null)
        {
            //RuizaoMainGO.SetActive(false);
            Destroy(RuizaoMainGO);
        }
        if (RuizaoCoreCubeShow != null)
        {
            RuizaoCoreCubeShow.SetActive(true);
        }
        
    }
    void EnterNewLevel(object sender, YTriggerEventArgs e)
    {
        if (e.activated)
        {
            ExitAndNoWin();
            isOnce = false;
            if (RuizaoCoreCubeShowOver != null)
            {
                RuizaoCoreCubeShowOver.SetActive(false);
            }
        }
    }
    public override void SetResultOff()
    {
        
    }
    
    public override void EnterField(bool isEnter, GameObject TriggergameObject,Transform showUIPlace)
    {
        base.EnterField(isEnter, TriggergameObject,showUIPlace);
        if (isEnter)
        {
            Debug.Log("进入区域");
        }
        else
        {
            Debug.Log("离开区域");
        }
    
    }
    
}
