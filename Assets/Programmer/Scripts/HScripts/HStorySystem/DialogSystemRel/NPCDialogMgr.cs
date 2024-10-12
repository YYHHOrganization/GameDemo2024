using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCDialogMgr : MonoBehaviour
{
    private string npcName;
    private string npcId;
    private Animator animator;
    private string npcDesc;
    private GameObject getUI;
    
    public void SetNpcBaseInfo(string name, string id, string desc)
    {
        npcName = name;
        npcId = id;
        npcDesc = desc;
        animator = gameObject.GetComponent<Animator>();
        ShowNpcBaseInfo();
    }

    private GameObject missionShowUI;

    public void PrepareForMission()  //调用这个函数，在NPC头上显示一个对应的感叹号
    {
        //todo:后面可以传入不同的任务类型，比如主线显示黄色，支线显示蓝色之类的，等支持功能更多了再说吧
        missionShowUI.gameObject.SetActive(true);
    }

    private void ShowNpcBaseInfo()  //显示NPC的基本信息, 在头顶上
    {
        getUI = transform.Find("ShowCanvas/Panel").gameObject;
        //getUI.gameObject.SetActive(false);  //暂时NPC头顶都显示头衔
        getUI.transform.Find("NPCName").GetComponent<TMP_Text>().text = npcDesc;
        if (missionShowUI == null)
        {
            missionShowUI = getUI.transform.Find("MissionShow").gameObject;
            //missionShowUI.gameObject.SetActive(false);  //设置的时候，第一个任务的NPC显示，后面的不要显示,todo:逻辑有点乱，后面再改
        }
    }
    

    public void ChangeNPCAnimation(string animName)
    {
        if (animator)
        {
            animator.SetTrigger("is" + animName);
        }
    }

    private bool isDialogFinish = false;

    public void HideUI()
    {
        getUI.transform.Find("InteractTip").gameObject.SetActive(false);
        if (missionShowUI)
        {
            missionShowUI.gameObject.SetActive(false);
            //missionShowUI.transform.parent.parent.gameObject.SetActive(false);
        }
        isDialogFinish = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (isDialogFinish) return;
            getUI.transform.Find("InteractTip").gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            getUI.transform.Find("InteractTip").gameObject.SetActive(false);
        }
    }
}
