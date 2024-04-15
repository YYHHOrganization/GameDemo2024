using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUIRogueQuestionPage : MonoBehaviour
{
    private int childCount;
    private Button nextButton;
    private Button previousButton;
    private Button closeButton;
    private int currentPage = 0;
    private TMP_Text pageText;
    
    void Start()
    {
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
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
        pageText.text = (currentPage + 1) + "/" + childCount;
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        
        playerAnswers = new string[childCount];
        correctAnswers = new string[childCount];
        InitializeQuestions();
    }

    private string[] correctAnswers;

    private void InitializeQuestions()
    {
        //childCount就是问题的数量
        for (int i = 0; i < childCount; i++)
        {
            int j = i;
            Transform question = transform.GetChild(i);
            HQuestionRoomBase questionRoomBase = transform.GetComponent<HQuestionRoomBase>();
            string content;
            string label;
            if (questionRoomBase.GiveAQuestionWithCurrectAnswer(out content, out label))
            {
                correctAnswers[i] = label;
                question.Find("QuestionContent").GetComponent<TMP_Text>().text = content;
                question.Find("angryButton").GetComponent<Button>().onClick.AddListener(()=>ChooseAnAnswer(j, "angry", question));
                question.Find("happyButton").GetComponent<Button>().onClick.AddListener(()=>ChooseAnAnswer(j, "happy", question));
                question.Find("sadButton").GetComponent<Button>().onClick.AddListener(()=>ChooseAnAnswer(j, "sad", question));
                question.Find("surpriseButton").GetComponent<Button>().onClick.AddListener(()=>ChooseAnAnswer(j, "surprise", question));
                question.Find("neutralButton").GetComponent<Button>().onClick.AddListener(()=>ChooseAnAnswer(j, "neutral", question));
                question.Find("fearButton").GetComponent<Button>().onClick.AddListener(()=>ChooseAnAnswer(j, "fear", question));
            }
        }
    }

    private string[] playerAnswers;
    private void ChooseAnAnswer(int questionIndex, string emotion, Transform question)
    {
        if(playerAnswers[questionIndex] != null)
        {
            var buttons = question.GetComponentsInChildren<Image>();
            foreach (var button in buttons)
            {
                button.color = Color.white;
            }
        }
        playerAnswers[questionIndex] = emotion;
        question.Find(emotion + "Button").GetComponent<Image>().color = Color.red;
    }

    private void OnEnable()
    {
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
    }
    
    
    private void NextPage()
    {
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
        transform.GetChild(currentPage).gameObject.SetActive(false);
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = childCount - 1;
        }
        transform.GetChild(currentPage).gameObject.SetActive(true);
        pageText.text = (currentPage + 1) + "/" + childCount;
    }

    private bool CheckAllAnswers(out int cnt)
    {
        cnt = 0;
        bool isAllAnswered = true;
        for (int i = 0; i < childCount; i++)
        {
            if (playerAnswers[i] == null)
            {
                isAllAnswered = false;
            }
            if (playerAnswers[i] == correctAnswers[i])
            {
                cnt++;
            }
        }

        return isAllAnswered;
    }

    private int correctCnt = -1;
    public void CloseTutorial()
    {
        Debug.Log("CloseTutorial");
        int cnt;
        if (CheckAllAnswers(out cnt))
        {
            correctCnt = cnt;
            //完成了所有的题目，直接退出
            ExitPanel();
        }
        else
        {
            correctCnt = cnt;
            HMessageShowMgr.Instance.ShowMessageWithActions("ConfirmExitRogueQuestion",ExitPanel,null,null);
        }
        
    }

    private void ExitPanel()
    {
        HMessageShowMgr.Instance.ShowMessage("ANSWER_RIGHT_QUESTION_CNT", "您答对了" + correctCnt + "道题目！");
        //trigger 宝箱出现，根据correctCnt给内容
        GiveReward();
        
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        gameObject.transform.parent.parent.SetAsFirstSibling();
        gameObject.SetActive(false);
        this.transform.parent.gameObject.SetActive(false);
        Destroy(gameObject, 2f);
        YPlayModeController.Instance.LockPlayerInput(false);
    }
    
    private void GiveReward()
    {
        YTriggerEvents.RaiseOnCompleteRoom(true, correctCnt);
    }
}
