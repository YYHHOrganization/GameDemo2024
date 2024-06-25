using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HSettingPanelBaseLogic : MonoBehaviour
{
    public Button soundSettingButton;
    public Button graphSettingButton;
    public Button gameSettingButton;

    public Transform soundSettingPart;
    public Transform graphSettingPart;
    public Transform gameSettingPart;

    public Button backButton;
    public Slider volumeSizeSlider;
    public TMP_Text volumeSizeText;

    public Slider mouseSensitivitySlider;
    public TMP_Text mouseSensitivityText;
    
    public TMP_Dropdown aimHelperDropdown; 
    
    private float audioVolume = 1f;
    private float mouseSensitive = 1f;
    private bool aimHelperIsOn = true;
    private void Start()
    {
        AddAllListeners();
        SetAllValues();
    }

    private void SetAllValues()
    {
        audioVolume = HAudioManager.Instance.VolumeMultiplier;
        volumeSizeSlider.value = audioVolume;
        volumeSizeText.text = audioVolume.ToString("0.0");

        mouseSensitive = HRoguePlayerAttributeAndItemManager.Instance.MouseSensitive;
        mouseSensitivitySlider.value = mouseSensitive;
        mouseSensitivityText.text = mouseSensitive.ToString("0.0");
        
        aimHelperIsOn = HRoguePlayerAttributeAndItemManager.Instance.AimHelperIsOn;
        aimHelperDropdown.value = aimHelperIsOn ? 0 : 1;
    }
    
    private void AddAllListeners()
    {
        soundSettingButton.onClick.AddListener(() =>
        {
            soundSettingPart.gameObject.SetActive(true);
            graphSettingPart.gameObject.SetActive(false);
            gameSettingPart.gameObject.SetActive(false);
        });

        graphSettingButton.onClick.AddListener(() =>
        {
            soundSettingPart.gameObject.SetActive(false);
            graphSettingPart.gameObject.SetActive(true);
            gameSettingPart.gameObject.SetActive(false);
        });

        gameSettingButton.onClick.AddListener(() =>
        {
            soundSettingPart.gameObject.SetActive(false);
            graphSettingPart.gameObject.SetActive(false);
            gameSettingPart.gameObject.SetActive(true);
        });
        
        backButton.onClick.AddListener(() =>
        {
            soundSettingPart.gameObject.SetActive(true);
            graphSettingPart.gameObject.SetActive(false);
            gameSettingPart.gameObject.SetActive(false);
        });
        
        volumeSizeSlider.onValueChanged.AddListener((value) =>
        {
            HAudioManager.Instance.VolumeMultiplier = value;
            HAudioManager.Instance.UpdateAllAudioVolumes();
            volumeSizeText.text = value.ToString("0.0");
        });
        
        mouseSensitivitySlider.onValueChanged.AddListener((value) =>
        {
            HRoguePlayerAttributeAndItemManager.Instance.MouseSensitive = value;
            HRoguePlayerAttributeAndItemManager.Instance.UpdateMouseSensitive();
            mouseSensitivityText.text = value.ToString("0.0");
        });
        
        aimHelperDropdown.onValueChanged.AddListener((value) =>
        {
            aimHelperIsOn = (value == 0);
            Debug.Log("aimHelperIsOn??" + aimHelperIsOn);
            HRoguePlayerAttributeAndItemManager.Instance.AimHelperIsOn = aimHelperIsOn;
            HRoguePlayerAttributeAndItemManager.Instance.UpdateAimHelper();
        });
        
    }
}
