using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChangeTimeScaleItem : HRogueItemBase
{
    protected override void UseNegativeItem(string funcName, string funcParams)
    {
        if (funcName != "UseInSelfFunction")  //应该是填错了，此时报错
        {
            Console.Error.WriteLine("UseNegativeItem: funcName is not UseInSelfFunction");
            return;
        }
        string[] param = funcParams.Split(';');
        float timeScale = float.Parse(param[0]);
        float duration = float.Parse(param[1]);
        HRogueItemFuncUtility.Instance.SetTimeScaleTime(timeScale);
        DOVirtual.DelayedCall(duration, () =>
        {
            HRogueItemFuncUtility.Instance.ResetTimeScale();
        }).SetUpdate(true);
    }
}
