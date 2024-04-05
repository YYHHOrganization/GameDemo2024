using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerObjInfo  //要trigger的物体信息,包括物体是什么,出现还是消失,出现的话多久消失
{
    [Header("会被当前trigger激发的物体")]
    public GameObject objectToTrigger;
    [Header("该物体是否会反复出现隐藏(每进入碰撞检测一次就出现/隐藏)")]
    public bool showAndHide = false;  //如果一个物体要反复trigger显示或消失,这个字段设置为true,否则false
    [Header("初始状态下,该物体的状态")]
    public bool isShownNow = false; //一开始的时候是否显示,默认不显示
    [Header("是否要触发显示?(仅在ShowAndHide=false时有效)")]
    public bool isGoingToShow;
    [Header("表示触发出现后多久自动消失(负数-永不消失)(仅在ShowAndHide=false时有效)")]
    public float disappearTime;  //<0表示永远不消失
    [Header("是否是只能触发一次的物体")]
    public bool neverShowAgain = false;
    [HideInInspector]
    public bool hasShown = false;//当neverShowAgain为true时触发,用以指示是否已经出现过了
}


public class YTriggerToMakeSth : MonoBehaviour
{
    public List<TriggerObjInfo> triggerObjList = new List<TriggerObjInfo>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (TriggerObjInfo triggerObjInfo in triggerObjList)
        {
            triggerObjInfo.objectToTrigger.gameObject.SetActive(triggerObjInfo.isShownNow);
        }
    }

    IEnumerator SetDisappear(TriggerObjInfo target,float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        target.objectToTrigger.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        //对other进行一些判定处理
        foreach(TriggerObjInfo triggerObjInfo in triggerObjList)
        {
            if (triggerObjInfo.neverShowAgain)
            {
                if (triggerObjInfo.hasShown == false)
                    triggerObjInfo.hasShown = true;
                else
                    continue;
            }
            if (triggerObjInfo.showAndHide)
            {
                triggerObjInfo.isShownNow = !triggerObjInfo.isShownNow;
                triggerObjInfo.objectToTrigger.gameObject.SetActive(triggerObjInfo.isShownNow);
            }
            else
            {
                if (triggerObjInfo.isGoingToShow)
                {
                    triggerObjInfo.objectToTrigger.gameObject.SetActive(true);
                    if (triggerObjInfo.disappearTime > 0)
                    {
                        StartCoroutine(SetDisappear(triggerObjInfo, triggerObjInfo.disappearTime));
                    }
                }
                else
                    triggerObjInfo.objectToTrigger.gameObject.SetActive(false);

            }
            
        }
    }


}
