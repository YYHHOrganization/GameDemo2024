using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HRogueAttributeBaseLogic : MonoBehaviour
{
    public GameObject DetailPanel;

    public TMP_Text detailTip;

    public TMP_Text shootRangeText;

    public TMP_Text shootRateText;

    public TMP_Text bulletDamageText;

    public TMP_Text moveSpeedText;

    public Transform heartAndShieldPanel;

    public TMP_Text bulletSpeed;
    
    public TMP_Text xingqiongText;
    public TMP_Text xinyongdianText;

    private bool isDetailOpen = false;
    public Image characterIcon;

    private void Start()
    {
        if (HRoguePlayerAttributeAndItemManager.Instance.GetCharacterIcon())
        {
            characterIcon.sprite = HRoguePlayerAttributeAndItemManager.Instance.GetCharacterIcon();
        }
        
    }

    public void UpdateText()
    {
        
    }

    public void SetRogueAttributeText()
    {
        shootRangeText.text = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueShootRange"].ToString();
        shootRateText.text = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueShootRate"].ToString();
        bulletDamageText.text = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueBulletDamage"].ToString();
        moveSpeedText.text = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueMoveSpeed"].ToString();
        bulletSpeed.text = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueBulletSpeed"].ToString();
        xingqiongText.text = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueXingqiong"].ToString();
        xinyongdianText.text = HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueXinyongdian"].ToString();
    }

    public void SetHealthAndShieldOnUI()
    {
        int health = (int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueCharacterHealth"];
        int shield = (int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueCharacterShield"];
        int healthUpperBound =
            (int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueCharacterHealthUpperBound"];
        //all set false
        for (int i = 0; i < heartAndShieldPanel.childCount; i++)
        {
            heartAndShieldPanel.GetChild(i).GetComponent<Image>().fillAmount = 1;
            if (i < 12)
            {
                heartAndShieldPanel.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = 1;
                heartAndShieldPanel.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            heartAndShieldPanel.GetChild(i).gameObject.SetActive(false);
        }
        
        for (int i = 0; i < healthUpperBound / 2; i++)
        {
            heartAndShieldPanel.GetChild(i).gameObject.SetActive(true);
        }
        for(int i = 0; i< health / 2; i++)
        {
            heartAndShieldPanel.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
        if(health % 2==1)
        {
            heartAndShieldPanel.GetChild(health / 2).GetChild(0).gameObject.SetActive(true);
            heartAndShieldPanel.GetChild(health / 2).GetChild(0).gameObject.GetComponent<Image>().fillAmount = 0.5f;
        }
        
        for (int i = 0; i < shield/2; i++)
        {
            heartAndShieldPanel.GetChild(i+12).gameObject.SetActive(true);
        }
        if(shield % 2==1)
        {
            heartAndShieldPanel.GetChild(shield / 2+12).gameObject.SetActive(true);
            heartAndShieldPanel.GetChild(shield / 2+12).gameObject.GetComponent<Image>().fillAmount = 0.5f;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isDetailOpen=!isDetailOpen;
            DetailPanel.SetActive(isDetailOpen);
            if (isDetailOpen)
            {
                SetRogueAttributeText();
                detailTip.text = "I键关闭详细面板";
            }
            else
            {
                detailTip.text = "I键展开详细面板";
            }
        }
    }
}
