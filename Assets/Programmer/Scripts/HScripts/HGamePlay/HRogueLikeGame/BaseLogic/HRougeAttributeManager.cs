using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HRougeAttributeManager : MonoBehaviour
{
    //单例模式
    private static HRougeAttributeManager _instance;
    private HRoguePlayerAttributePanel attributePanel;
    private int characterIndex;
    private Sprite characterIcon;
    public static HRougeAttributeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HRougeAttributeManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Dictionary<string, float> characterValueAttributes = new Dictionary<string, float>();
    public MyShootEnum characterBaseWeapon;

    public void ResetEveryAttributeWithCharacter(RogueCharacterBaseAttribute characterBaseAttribute, int index)
    {
        characterValueAttributes.Clear();
        characterValueAttributes.Add("RogueMoveSpeed", characterBaseAttribute.rogueMoveSpeed);
        characterValueAttributes.Add("RogueShootRate", characterBaseAttribute.rogueShootRate);
        characterValueAttributes.Add("RogueShootRange", characterBaseAttribute.rogueShootRange);
        characterValueAttributes.Add("RogueBulletDamage", characterBaseAttribute.rogueBulletDamage);
        characterValueAttributes.Add("RogueBulletSpeed", characterBaseAttribute.rogueBulletSpeed);
        characterValueAttributes.Add("RogueCharacterHealth", characterBaseAttribute.rogueCharacterHealth);
        characterValueAttributes.Add("RogueCharacterShield", characterBaseAttribute.rogueCharacterShield);
        characterValueAttributes.Add("RogueXingqiong", characterBaseAttribute.rogueStartXingqiong);
        characterValueAttributes.Add("RogueXinyongdian", characterBaseAttribute.rogueStartXinyongdian);
        characterBaseWeapon = characterBaseAttribute.RogueCharacterBaseWeapon;
        characterIndex = index;
        characterIcon = Addressables.LoadAssetAsync<Sprite>(characterBaseAttribute.rogueCharacterIconLink)
            .WaitForCompletion();
        GiveOutOriginThing(characterBaseAttribute);
    }

    //给出初始的物品
    private void GiveOutOriginThing(RogueCharacterBaseAttribute characterBaseAttribute)
    {
        string xingqiongId = "20000012";
        string xinyongdianId = "20000013";
        HItemCounter.Instance.AddItem(xingqiongId, characterBaseAttribute.rogueStartXingqiong);
        HItemCounter.Instance.AddItem(xinyongdianId, characterBaseAttribute.rogueStartXinyongdian);
        UpdateEverythingInAttributePanel();
    }

    public void PushAttributePanel()
    {
        attributePanel = new HRoguePlayerAttributePanel();
        YGameRoot.Instance.Push(attributePanel);
    }

    public void UpdateWithKeyAndValue(string key, float value)
    {
        
        UpdateEverythingInAttributePanel();
    }

    public void UpdateEverythingInAttributePanel()
    {
        if (attributePanel!=null)
        {
            attributePanel.uiTool.Get<HRogueAttributeBaseLogic>().SetRogueAttributeText();
            attributePanel.uiTool.Get<HRogueAttributeBaseLogic>().SetHealthAndShield();
        }
    }
    
    public Sprite GetCharacterIcon()
    {
        return characterIcon;
    }

}
