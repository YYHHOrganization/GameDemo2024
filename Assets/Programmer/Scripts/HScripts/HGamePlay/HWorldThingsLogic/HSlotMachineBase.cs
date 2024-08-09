using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HSlotMachineBase : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Transform> slots = new List<Transform>();
    public bool isPlaying = false;
    private GameObject slotsRoot;
    private GameObject rotateHand;
    private int shakeHash;
    private Animator animator;
    private HItemShowPanel shouldGivePanel;
    public CinemachineVirtualCamera camera;
    private string newContent;
    
    void Start()
    {
        slotsRoot = transform.Find("Slots").gameObject;
        rotateHand = transform.Find("RotateHand").gameObject;
        animator = GetComponent<Animator>();
        shakeHash = Animator.StringToHash("isShaking");
        for(int i = 0; i < slotsRoot.gameObject.transform.childCount; i++)
        {
            slots.Add(slotsRoot.gameObject.transform.GetChild(i));
        }
        YTriggerEvents.OnGiveOutItemInBagForSlotMachine += SetIdAndCount;
        camera.gameObject.SetActive(false);
    }

    private string chooseItemID = "-1";
    private int chooseItemCount;
    
    void SetIdAndCount(object sender, YTriggerGivingItemEventArgs e)
    {
        chooseItemCount = e.itemCount;
        chooseItemID = e.itemId;
    }
    void CalculateTheResult()
    {
        Debug.Log(chooseItemCount + " " + chooseItemID);
        
        if (chooseItemID != "-1")
        {
            if(!yPlanningTable.Instance.worldItems.ContainsKey(chooseItemID))
            {
                HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg","想要不劳而获？");
                return;
            }
            string itemName = yPlanningTable.Instance.worldItems[chooseItemID].chineseName;
            HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg","你失去了" + chooseItemCount + "个" + itemName + ", 换取了一次抽奖机会！");
            int num = Random.Range(0, 10000);
            if (num > 5000)
            {
                HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg","这是一场豪赌，朋友！");
            }
            else
                HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg","害怕醒来吗？");
        }
        if (camera)
        {
            camera.gameObject.SetActive(true);
        }
        
        List<int> rollResults = new List<int>();
        //随机生成一个结果
        for(int i=0; i<slots.Count; i++)
        {
            rollResults.Add(Random.Range(0, 4));
        }
        StartCoroutine(PlayTheGame(rollResults));
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            GiveOutSomethingAndPlay();
            isPlaying = false;
        }
    }
    
    public void GiveOutSomethingAndPlay()
    {
        //调出提交物品的菜单, 用Message表的新类型
        HMessageShowMgr.Instance.ShowMessageWithActions("SubmitSthInSlotMachineMsg", CalculateTheResult,null,null);

    }

    private void CheckResultAndGiveOutTreasure(List<int> rollResults)
    {
        //debug,2:砂金，0：希儿 1：布洛妮娅 3：星
        int[] resultsCnt = new int[4];  //results[n]表示第n种图案出现的次数
        for(int i=0;i<rollResults.Count;i++)
        {
            resultsCnt[rollResults[i]]++;
        }
        //如果有五个一样的角色，给非常多的奖励（先Debug一下）
        if(resultsCnt[0]==5 || resultsCnt[1]==5 || resultsCnt[2]==5 || resultsCnt[3]==5)
        {
            string itemName = yPlanningTable.Instance.worldItems[chooseItemID].chineseName;
            newContent = new string("五个相同的角色！你得到了五倍奖励！获得了" + chooseItemCount * 5 + "个" + itemName + "！");
            HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg",newContent);
            HItemCounter.Instance.AddItem(chooseItemID, chooseItemCount*5);
        }
        //如果每种角色至少都有一个，给双倍奖励，并给出钥匙
        else if(resultsCnt[0]>=1 && resultsCnt[1]>=1 && resultsCnt[2]>=1 && resultsCnt[3]>=1)
        {
            string itemName = yPlanningTable.Instance.worldItems[chooseItemID].chineseName;
            newContent = new string("每种角色都至少出现了一次，你获得了双倍奖励！获得了" + chooseItemCount * 2 + "个" + itemName + "！");
            HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg",newContent);
            HItemCounter.Instance.AddItem(chooseItemID, chooseItemCount*2);
        }
        //只有0（希儿）和1（布洛妮娅）的话，给出三倍奖励，并给出爱心特效
        // else if(resultsCnt[0]>=1 && resultsCnt[1]>=1 && resultsCnt[2]==0 && resultsCnt[3]==0)
        // {
        //     string itemName = yPlanningTable.Instance.worldItems[chooseItemID].chineseName;
        //     newContent = new string("只出现了希儿和布洛妮娅！你获得了三倍奖励！获得了" + chooseItemCount * 3 + "个" + itemName + "！");
        //     HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg",newContent);
        //     HItemCounter.Instance.AddItem(chooseItemID, chooseItemCount*3);
        // }
        //某一种图案出现了三次及以上
        else if(resultsCnt[0]>=3 || resultsCnt[1]>=3 || resultsCnt[2]>=3 || resultsCnt[3]>=3)
        {
            string itemName = yPlanningTable.Instance.worldItems[chooseItemID].chineseName;
            newContent = new string("某一种图案出现了三次及以上！你获得了双倍奖励！获得了" + chooseItemCount * 2  + "个" + itemName + "！");
            HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg",newContent);
            HItemCounter.Instance.AddItem(chooseItemID, chooseItemCount * 2);
        }
        //并没有四种图案都出现，扣除全部的钱
        else
        {
            string itemName = yPlanningTable.Instance.worldItems[chooseItemID].chineseName;
            newContent = new string("啊哦，你失去了全部的"+ itemName + "。");
            HMessageShowMgr.Instance.ShowMessage("SlotMachineSubmitMsg",newContent);
        }
        
        // for(int i=0;i<rollResults.Count;i++)
        // {
        //     Debug.Log("Slot " + i + " result is " + rollResults[i]);
        // }
    }

    IEnumerator PlayTheGame(List<int> rollResults)
    {
        //把手旋转， dotween rotate x
        rotateHand.transform.DOLocalRotate(new Vector3(60, 0, 90), 1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);
        if (animator)
        {
            animator.SetBool(shakeHash, true);
        }

        yield return new WaitForSeconds(1f);
        for(int i=0;i<slots.Count;i++)
        {
            slots[i].DOLocalRotate(new Vector3(0, 0, -360*5 + rollResults[i] * 90), 3f, RotateMode.LocalAxisAdd)
                            .SetEase(Ease.InCirc);
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(4.5f);
        CheckResultAndGiveOutTreasure(rollResults);
        
        Invoke("ResetEverything", 3.5f);
    }

    private void ResetEverything()
    {
        rotateHand.transform.DOLocalRotate(new Vector3(0, 0, 90), 1f).SetEase(Ease.InCirc);
        for (int i=0;i<slots.Count;i++)
        {
            slots[i].DOLocalRotate(new Vector3(0, -90, 140), 3f, RotateMode.Fast)
                .SetEase(Ease.InCirc);
        }
        if (animator)
        {
            animator.SetBool(shakeHash, false);
        }

        if (camera)
        {
            camera.gameObject.SetActive(false);
        }
    }

}
