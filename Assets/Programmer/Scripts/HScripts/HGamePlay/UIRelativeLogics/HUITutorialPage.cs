using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUITutorialPage : MonoBehaviour
{
    private int childCount;
    private Button nextButton;
    private Button previousButton;
    private Button closeButton;
    private int currentPage = 0;
    private TMP_Text pageText;
    
    void Start()
    {
        childCount = transform.childCount - 4; //-4 是因为有四个额外的东西
        Debug.Log(childCount + "ssssssss");
        nextButton = transform.Find("RightButton").GetComponent<Button>();
        previousButton = transform.Find("LeftButton").GetComponent<Button>();
        closeButton = transform.Find("CloseButton").GetComponent<Button>();
        pageText = transform.Find("PageCount").GetComponent<TMP_Text>();
        
        nextButton.onClick.AddListener(NextPage);
        previousButton.onClick.AddListener(PreviousPage);
        closeButton.onClick.AddListener(CloseTutorial);
        currentPage = 0;
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
    }

    private void OnEnable()
    {
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
    }

    private void NextPage()
    {
        Debug.Log("NextPage");
        transform.GetChild(currentPage).gameObject.SetActive(false);
        currentPage++;
        if (currentPage >= childCount)
        {
            currentPage = 0;
        }
        transform.GetChild(currentPage).gameObject.SetActive(true);
        pageText.text = (currentPage + 1) + "/" + childCount;
    }
    
    private void PreviousPage()
    {
        Debug.Log("PreviousPage");
        transform.GetChild(currentPage).gameObject.SetActive(false);
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = childCount - 1;
        }
        transform.GetChild(currentPage).gameObject.SetActive(true);
        pageText.text = (currentPage + 1) + "/" + childCount;
    }
    
    public void CloseTutorial()
    {
        Debug.Log("CloseTutorial");
        gameObject.transform.parent.parent.SetAsFirstSibling();
        gameObject.SetActive(false);
        this.transform.parent.gameObject.SetActive(false);
        Destroy(gameObject, 2f);
        YPlayModeController.Instance.LockPlayerInput(false);
        Invoke("GiveOutAXingqiong", 1f);
    }

    private void GiveOutAXingqiong()
    {
        HOpenWorldTreasureManager.Instance.JustGiveoutSomeTreasures("20000000",1);
    }
}
