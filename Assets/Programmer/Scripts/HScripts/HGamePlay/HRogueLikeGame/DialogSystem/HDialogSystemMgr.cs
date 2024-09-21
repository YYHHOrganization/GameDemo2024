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
        YPlayModeController.Instance.LockEveryInputKey = true;
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);

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
        //ReadText(dialogDataFile);
    }
    

    public void ResetDialog()
    {
        //CheckFinalLine();
        panel.gameObject.SetActive(true);
        ReadText(dialogDataFile);
        ShowDialogRow();
    }

    private Coroutine textShowCoroutine;
    public void UpdateText(string strname, string title, string text)
    {
        nameText.text = strname;
        //1 秒的时间，文本依次出现
        contentText.text = "";
        if (textShowCoroutine != null)
        {
            StopCoroutine(textShowCoroutine);
        }
        textShowCoroutine = StartCoroutine(ShowText(text));
        titleText.text = title;
    }
    
    IEnumerator ShowText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            contentText.text += text[i];
            yield return new WaitForSeconds(0.04f);
        }
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
        string asset = ReadTextsFromPath("Designer/CsvTable/DialogQuestions/");
        //找到textAsset对应的目录下的所有的文件
        dialogRows = asset.Split('\n');
        
    }

    private string ReadTextsFromPath(string path)
    {
        string fullPath = Application.dataPath + "/" + path;
        //获取这个路径下的所有文件
        string[] files = System.IO.Directory.GetFiles(fullPath, "*.csv");
        //随机选一个文件
        int randomIndex = Random.Range(0, files.Length);
        //test
        //randomIndex = 2;
        //randomIndex = 3;
        string file = files[randomIndex];
        //读取这个文件
        string content = System.IO.File.ReadAllText(file);
        return content;
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
                //gameObject.SetActive(false);
                GameAPI.Broadcast(new GameMessage(GameEventType.CompleteDialogue, "testDialog09"));
                YPlayModeController.Instance.LockPlayerInput(false);
                YTriggerEvents.RaiseOnMouseLeftShoot(true);
                YPlayModeController.Instance.LockEveryInputKey = false;
                YTriggerEvents.RaiseOnMouseLockStateChanged(true);
                if (triggerToDialog)
                {
                    triggerToDialog.gameObject.GetComponent<HTriggerDialogSystem>().SetInteracted(true);
                }
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
                        string[] effect = cells[7].Split((';'));
                        ShowOptionEffects(effect[0], effect[1]);
                    }
                });
            GenerateOptionButton(index + 1);
        }
        
    }

    private void ShowOptionEffects(string funcName, string funcParams)
    {
        //Debug.Log("ShowOptionEffects" + funcName + "   " + funcParams);
        switch (funcName)
        {
            case "GiveItemWithId":
                HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(funcParams);
                break;
            case "AddXingqiong":
                Debug.Log("now we should add xingqiong");
                HRogueItemFuncUtility.Instance.AddMoney("RogueXingqiong;" + funcParams);
                break;
            case "AddCatcake":
                YPlayModeController.Instance.SetCatcake(funcParams);
                break;
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
