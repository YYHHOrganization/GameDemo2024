using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class HGMPanelBaseLogic : MonoBehaviour
{
    public TMP_InputField commandInputField;

    public Button commandExecuteBtn;

    public Button clearCommandBtn;

    public Button giveMeItemBtn;

    public Button giveMeCatcakeBtn;

    public Button addSthBtn;

    public Button summonSthBtn;

    public Button showDesignerTableBtn;
    
    [SerializeField] private List<string> autoCompleteOptions;

    public TMP_Dropdown commandCompleteDropDown;

    private void AddToAutoCompleteOptions()
    {
        autoCompleteOptions = new List<string>();
        autoCompleteOptions.Add("give me catcake @name = RuanmeiCatcake");
        autoCompleteOptions.Add("give me catcake @name = RenCatcake");
        autoCompleteOptions.Add("give me catcake @name = DanhengCatcake");
        autoCompleteOptions.Add("give me catcake @name = XingCatcake");
        autoCompleteOptions.Add("give me catcake @name = KafkaCatcake");
        autoCompleteOptions.Add("give me catcake @name = March7thCatcake");
        autoCompleteOptions.Add("give me catcake @name = RobinCatcake");
        autoCompleteOptions.Add("add sth @name = xingqiong, number = 50000");
        autoCompleteOptions.Add("add sth @name = xinyongdian, number = 100000");
        autoCompleteOptions.Add("add sth @name = RogueMoveSpeed, number = 2"); 
        autoCompleteOptions.Add("add sth @name = RogueShootRate, number = 2");
        autoCompleteOptions.Add("add sth @name = RogueBulletDamage, number = 2");
        autoCompleteOptions.Add("add sth @name = RogueShootRange, number = 2");
        autoCompleteOptions.Add("add sth @name = RogueBulletSpeed, number = 2");
        autoCompleteOptions.Add("add sth @name = RogueCharacterHealthUpperBound, number = 2");
        autoCompleteOptions.Add("add sth @name = RogueCharacterHealth, number = 2");
        autoCompleteOptions.Add("add sth @name = RogueCharacterShield, number = 2");
        autoCompleteOptions.Add("set me invincible @true"); //无敌
        autoCompleteOptions.Add("set me invincible @false"); //不无敌
        autoCompleteOptions.Add("summon enemy @id = 4, number = 2");
        
        autoCompleteOptions.Add("test add recall object @id = 33310000, number = 8");
    }
    // Start is called before the first frame update
    void Start()
    {
        AddToAutoCompleteOptions();
        commandExecuteBtn.onClick.AddListener(() =>
        {
            string command = commandInputField.text;
            ExecuteCommand(command);
        });
        
        clearCommandBtn.onClick.AddListener(() =>
        {
            commandInputField.text = "";
        });
        
        giveMeItemBtn.onClick.AddListener(() =>
        {
            commandInputField.text = "give me item @id = ";
        });
        
        giveMeCatcakeBtn.onClick.AddListener(() =>
        {
            commandInputField.text = "give me catcake @name = ";
        });
        
        addSthBtn.onClick.AddListener(() =>
        {
            commandInputField.text = "add sth @name = ";
        });
        
        summonSthBtn.onClick.AddListener(() =>
        {
            commandInputField.text = "summon enemy @id = ";
        });
        
        commandInputField.onSubmit.AddListener((string command) =>
        {
            ExecuteCommand(command);
        });
        
        showDesignerTableBtn.onClick.AddListener(() =>
        {
            ShowDesignerTable();
        });

        commandInputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void ShowDesignerTable() //展示策划表
    {
        //默认打开的是item表
        string command = commandInputField.text;
        if (command == "")
        {
            OpenFileWithRelativePath("Designer/CsvTable/RogueLike/RogueItemCSVFile.csv");
        }
        else
        {
            string[] commandParts = command.Split('@');
            string commandType = commandParts[0];
            switch (commandType)
            {
                case "summon enemy ":
                    OpenFileWithRelativePath("Designer/Designer_Tool/CSV2CS/Document/RogueEnemyCSVFile.csv");
                    break;
                case "give me item ":
                    OpenFileWithRelativePath("Designer/CsvTable/RogueLike/RogueItemCSVFile.csv");
                    break;
            }

        }
    }

    private void OpenFileWithRelativePath(string path)
    {
        string fullPath = Application.dataPath + "/" + path;
        //string fullPath = path;
        try
        {
            StreamReader reader = new StreamReader(fullPath);
            Debug.Log("File opened successfully: " + fullPath);

            // 在这里可以读取文件内容或者进行其他操作
            // 打开文件
            Process.Start(fullPath);
            reader.Close();
            // 在Windows窗口中显示文件内容
            // ShowFileInWindows(fullPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open file at path: " + fullPath);
            Debug.LogError(e.Message);
        }
    }
    
    
    
    private void OnInputValueChanged(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            // 清空自动补全选项
            ClearAutoCompleteOptions();
            return;
        }

        GetFilteredOptions(text);
        ShowAutoCompleteOptions(filteredOptions);
    }

    private List<string> filteredOptions = new List<string>();
    
    private List<string> GetFilteredOptions(string inputText)
    {
        filteredOptions = new List<string>();

        foreach (string option in autoCompleteOptions)
        {
            if (option.StartsWith(inputText))
            {
                filteredOptions.Add(option);
            }
        }

        return filteredOptions;
    }

    private void ShowAutoCompleteOptions(List<string> options)
    {
        // 在这里实现显示自动补全选项的逻辑，例如在UI中显示一个下拉列表或建议框
        //Debug.Log("Filtered options: " + string.Join(", ", options));
        InitializeDropdown();
    }
    
    private void InitializeDropdown()
    {
        commandCompleteDropDown.ClearOptions();
        commandCompleteDropDown.onValueChanged.RemoveAllListeners();
        commandCompleteDropDown.AddOptions(filteredOptions);
        commandCompleteDropDown.onValueChanged.AddListener(OnDropdownValueChanged);
    }
    
    private void OnDropdownValueChanged(int index)
    {
        commandInputField.text = filteredOptions[index];
    }

    private bool tabToComplete = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (filteredOptions.Count > 0)
            {
                commandInputField.text = filteredOptions[0];
            }
        }
    }

    private void ClearAutoCompleteOptions()
    {
        // 清空并隐藏自动补全选项UI
        Debug.Log("Clearing auto complete options.");
    }

    private void ExecuteCommand(string command)
    {
        //对比@符号之前的内容
        string[] commandParts = command.Split('@');
        string commandType = commandParts[0];
        switch (commandType)
        {
            case "give me item ":
                string idPart = commandParts[1]; //id = 2
                int id = int.Parse(idPart.Split('=')[1].Trim()) + 80000000;
                string finalId = id.ToString();
                Debug.Log("give me item with id: " + finalId);
                GiveOutItemWithId(finalId);
                break;
            case "give me catcake ":
                string namePart = commandParts[1]; //name = "catcake"
                string name = namePart.Split('=')[1].Trim();
                Debug.Log("give me catcake with name: " + name);
                GiveOutCatcakeWithName(name);
                break;
            case "add sth ":
                string addPart = commandParts[1]; //name = "xingqiong, number = 50000"
                string[] addParts = addPart.Split(',');
                string name2 = addParts[0].Split('=')[1].Trim();
                int number = int.Parse(addParts[1].Split('=')[1].Trim());
                Debug.Log("add sth with name: " + name2 + ", number: " + number);
                AddSomething(name2, number);
                break;
            case "set me invincible ":
                Debug.Log("set me invincible");
                string onOrOff = commandParts[1];
                HRoguePlayerAttributeAndItemManager.Instance.SetCharacterInvincible(onOrOff.Trim() == "true");
                break;
            case "summon enemy ":
                string addPart2 = commandParts[1]; 
                string[] addParts2 = addPart2.Split(',');
                string id2 = addParts2[0].Split('=')[1].Trim();
                int number2 = int.Parse(addParts2[1].Split('=')[1].Trim());
                Debug.Log("add sth with id: " + id2 + ", number: " + number2);
                SummonEnemy(id2, number2);
                break;
            case "test add recall object ":
                string addPart3 = commandParts[1]; 
                string[] addParts3 = addPart3.Split(',');
                string id3 = addParts3[0].Split('=')[1].Trim();
                int number3 = int.Parse(addParts3[1].Split('=')[1].Trim());
                Debug.Log("add sth with id: " + id3 + ", number: " + number3);
                TestAddRecallObject(id3, number3);
                break;
        }
    }

    private void TestAddRecallObject(string id, int number)
    {
        Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        for (int j = 0; j < number; j++)
        {
            GameObject go = YObjectPool._Instance.Spawn(id);
            go.transform.position = player.position+new Vector3(Random.Range(-4,4),Random.Range(1,5),Random.Range(-4,4));
            go.SetActive(true);
        }
    }
    
    private void SummonEnemy(string id, int number)
    {
        string finalId = (70000000 + int.Parse(id)).ToString();
        for (int j = 0; j < number; j++)
        {
            string EnemyAddressLink = SD_RogueEnemyCSVFile.Class_Dic[finalId].addressableLink;
            Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
            GameObject enemy = Addressables.InstantiateAsync(EnemyAddressLink).WaitForCompletion();
            enemy.transform.position = player.transform.position + new Vector3(Random.Range(-7, 7), 0, Random.Range(-7, 7));
                
            //生成怪物
            // HRougeAttributeManager.Instance.GenerateEnemy(enemyIDs[randomEnemyIndex], transform);
        }
    }

    private void AddSomething(string name, int number)
    {
        switch (name)
        {
            case "xingqiong":
                HRogueItemFuncUtility.Instance.AddMoney("RogueXingqiong;" + number);
                break;
            case "xinyongdian":
                HRogueItemFuncUtility.Instance.AddMoney("RogueXinyongdian;" + number);
                break;
            case "RogueMoveSpeed":case "RogueShootRate":case "RogueBulletDamage":case "RogueShootRange":case "RogueBulletSpeed": case "RogueCharacterHealthUpperBound": case "RogueCharacterHealth": case "RogueCharacterShield":
                HRogueItemFuncUtility.Instance.UseNegativeItem("AddAttributeValue",name + ";" + number);
                break;
        }
    }

    private void GiveOutItemWithId(string itemId)
    {
        HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(itemId);
    }

    private void GiveOutCatcakeWithName(string name)
    {
        YPlayModeController.Instance.SetCatcake(name);
    }
}
