using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HDialogSystemMgr : MonoBehaviour
{
    public TextAsset dialogDataFile; //csv file
    
    public TMP_Text nameText;

    public TMP_Text titleText; //头衔

    public TMP_Text contentText;

    //public List<Cinemachine.CinemachineVirtualCamera> virtualCameras = new List<CinemachineVirtualCamera>();

    //private Dictionary<string, CinemachineVirtualCamera> cinemachineDict =
        //new Dictionary<string, CinemachineVirtualCamera>();

    private int dialogIndex; //用来保存当前的对话索引值
    private string[] dialogRows; //对话文本按行分割
    public GameObject panel;
    public Button nextButton;
    public GameObject optionButton;
    public Transform buttonGroup; //选项按钮的父节点
    private GameObject triggerToDialog; //开启对话的trigger
    
    private void Awake()
    {
        dialogIndex = 0;
        //panel.gameObject.SetActive(true);
        panel.gameObject.SetActive(false);
        // cinemachineDict["左"] = virtualCameras[0];
        // cinemachineDict["右"] = virtualCameras[1];
    }
    
    public void SetUpAndStartDialog()
    {
        dialogIndex = 0;
        panel.gameObject.SetActive(true);
        ReadText(dialogDataFile);
        ShowDialogRow();
        LockPlayerInput();
    }

    private void LockPlayerInput()
    {
        //锁住角色的移动和鼠标
        YPlayModeController.Instance.LockPlayerInput(true);
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
    }

    public void SetTrigger(GameObject trigger)
    {
        triggerToDialog = trigger;
    }

    // Start is called before the first frame update
    void Start()  //注意：Start方法只会调用一次，后面再把这个脚本挂在的物体setActive也不会调用Start方法了
    {
        // UpdateText("希儿", "下层区地火骨干成员", "绀海组第一!!!");
        // UpdateCinemachine("左");
        //panel.gameObject.SetActive(true);
        panel.gameObject.SetActive(false);
        // cinemachineDict["左"] = virtualCameras[0];
        // cinemachineDict["右"] = virtualCameras[1];
        nextButton.onClick.AddListener(OnClickNext);
        ReadText(dialogDataFile);
    }
    

    public void ResetDialog()
    {
        //CheckFinalLine();
        panel.gameObject.SetActive(true);
        ReadText(dialogDataFile);
        ShowDialogRow();
    }

    public void UpdateText(string strname, string title, string text)
    {
        nameText.text = strname;
        contentText.text = text;
        titleText.text = title;
    }

    public void UpdateCinemachine(string strname)
    {
        // foreach (var cinemachine in virtualCameras)
        // {
        //     cinemachine.gameObject.SetActive(false);
        // }
        // cinemachineDict[strname].gameObject.SetActive(true);
        //为了防止相机突然跳，先很简单的写一个if-else逻辑
        // if (strname == "左")
        // {
        //     cinemachineDict["右"].gameObject.SetActive(false);
        //     cinemachineDict["左"].gameObject.SetActive(true);
        // }
        // else if (strname == "右")
        // {
        //     cinemachineDict["右"].gameObject.SetActive(true);
        //     cinemachineDict["左"].gameObject.SetActive(false);
        // }
    }

    public void ReadText(TextAsset textAsset)
    {
        dialogRows = textAsset.text.Split('\n');
        
    }

    public void ShowDialogRow()
    {
        for (int i = 0;i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0]=="#" && int.Parse(cells[1]) == dialogIndex) //不要直接匹配dialogIndex行,因为可能策划会写一些注释行之类的东西
            {
                UpdateText(cells[2],cells[3],cells[5]);
                UpdateCinemachine(cells[4]);

                dialogIndex = int.Parse(cells[6]);
                nextButton.gameObject.SetActive(true);
                break;
            }
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex) //说明是选项
            {
                nextButton.gameObject.SetActive(false);
                GenerateOptionButton(i);
            }
            else if (cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
            {
                print("剧情结束");
                panel.gameObject.SetActive(false);
                //triggerToDialog.gameObject.GetComponent<TriggerToDialog>().Reset(CheckFinalLine());
                gameObject.SetActive(false);
                
                YPlayModeController.Instance.LockPlayerInput(false);
                YTriggerEvents.RaiseOnMouseLeftShoot(true);
            }
        }
    }

    public string CheckFinalLine()
    {
        for (int i = 0; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0] == "!")
            {
                return cells[1];
            }
        }

        return "default";
    }

    public void OnClickNext()
    {
        ShowDialogRow();
    }

    public void GenerateOptionButton(int index)
    {
        string[] cells = dialogRows[index].Split(',');
        if (cells[0] == "&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            button.GetComponentInChildren<TMP_Text>().text = cells[5];
            button.GetComponent<Button>().onClick.AddListener
                (delegate
                {
                    OnOptionClick(int.Parse(cells[6]));
                    if (cells[7] != "")
                    {
                        string[] effect = cells[7].Split(('@'));
                        cells[8] = Regex.Replace(cells[8], @"[\r\n]", ""); //因为最后一列可能包含\r\n，所以用正则表达式去掉
                        //OptionEffect(effect[0], int.Parse(effect[1]), cells[8]);//这里可以调用一些选项的特殊效果，后面再说
                    }
                });
            GenerateOptionButton(index + 1);
        }
        
    }

    public void OnOptionClick(int id)
    {
        dialogIndex = id;
        ShowDialogRow();
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
