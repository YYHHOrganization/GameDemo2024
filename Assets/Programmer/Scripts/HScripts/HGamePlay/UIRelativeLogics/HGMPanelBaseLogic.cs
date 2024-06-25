using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HGMPanelBaseLogic : MonoBehaviour
{
    public TMP_InputField commandInputField;

    public Button commandExecuteBtn;

    public Button clearCommandBtn;

    public Button giveMeItemBtn;

    public Button giveMeCatcakeBtn;

    public Button addSthBtn;

    public Button summonSthBtn;
    
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
        autoCompleteOptions.Add("add sth @name = xingqiong, @number = 50000");
        autoCompleteOptions.Add("add sth @name = xinyongdian, @number = 100000");
        
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
            commandInputField.text = "add sth @name = , @number = ";
        });
        
        summonSthBtn.onClick.AddListener(() =>
        {
            commandInputField.text = "summon sth @id =  , @number = ";
        });
        
        commandInputField.onSubmit.AddListener((string command) =>
        {
            ExecuteCommand(command);
        });

        commandInputField.onValueChanged.AddListener(OnInputValueChanged);
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
