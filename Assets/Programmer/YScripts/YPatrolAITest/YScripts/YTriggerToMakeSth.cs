using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerObjInfo  //Ҫtrigger��������Ϣ,����������ʲô,���ֻ�����ʧ,���ֵĻ������ʧ
{
    [Header("�ᱻ��ǰtrigger����������")]
    public GameObject objectToTrigger;
    [Header("�������Ƿ�ᷴ����������(ÿ������ײ���һ�ξͳ���/����)")]
    public bool showAndHide = false;  //���һ������Ҫ����trigger��ʾ����ʧ,����ֶ�����Ϊtrue,����false
    [Header("��ʼ״̬��,�������״̬")]
    public bool isShownNow = false; //һ��ʼ��ʱ���Ƿ���ʾ,Ĭ�ϲ���ʾ
    [Header("�Ƿ�Ҫ������ʾ?(����ShowAndHide=falseʱ��Ч)")]
    public bool isGoingToShow;
    [Header("��ʾ�������ֺ����Զ���ʧ(����-������ʧ)(����ShowAndHide=falseʱ��Ч)")]
    public float disappearTime;  //<0��ʾ��Զ����ʧ
    [Header("�Ƿ���ֻ�ܴ���һ�ε�����")]
    public bool neverShowAgain = false;
    [HideInInspector]
    public bool hasShown = false;//��neverShowAgainΪtrueʱ����,����ָʾ�Ƿ��Ѿ����ֹ���
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
        //��other����һЩ�ж�����
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
