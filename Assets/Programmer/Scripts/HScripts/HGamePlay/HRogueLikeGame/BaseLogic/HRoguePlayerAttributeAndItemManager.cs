using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HRoguePlayerAttributeAndItemManager : MonoBehaviour
{
    //单例模式
    private static HRoguePlayerAttributeAndItemManager _instance;
    private HRoguePlayerAttributePanel attributePanel;
    private int characterIndex;
    private Sprite characterIcon;
    private GameObject player;
    private Camera camera;
    private List<GameObject> bulletPrefabs = new List<GameObject>();
    private int bulletPrefabLength = 0;
    private GameObject curBulletPrefab;
    public GameObject CurBulletPrefab => curBulletPrefab;
    public static HRoguePlayerAttributeAndItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HRoguePlayerAttributeAndItemManager>();
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
    public List<string> attributesWithNoMoney = new List<string>();

    public void ResetEveryAttributeWithCharacter(RogueCharacterBaseAttribute characterBaseAttribute, int index, GameObject player)
    {
        characterValueAttributes.Clear();
        characterValueAttributes.Add("RogueMoveSpeed", characterBaseAttribute.rogueMoveSpeed);
        attributesWithNoMoney.Add("RogueMoveSpeed");
        characterValueAttributes.Add("RogueShootRate", characterBaseAttribute.rogueShootRate);
        attributesWithNoMoney.Add("RogueShootRate");
        characterValueAttributes.Add("RogueShootRange", characterBaseAttribute.rogueShootRange);
        attributesWithNoMoney.Add("RogueShootRange");
        characterValueAttributes.Add("RogueBulletDamage", characterBaseAttribute.rogueBulletDamage);
        attributesWithNoMoney.Add("RogueBulletDamage");
        characterValueAttributes.Add("RogueBulletSpeed", characterBaseAttribute.rogueBulletSpeed);
        attributesWithNoMoney.Add("RogueBulletSpeed");
        characterValueAttributes.Add("RogueCharacterHealth", characterBaseAttribute.rogueCharacterHealth);
        characterValueAttributes.Add("RogueCharacterShield", characterBaseAttribute.rogueCharacterShield);
        characterValueAttributes.Add("RogueXingqiong", characterBaseAttribute.rogueStartXingqiong);
        characterValueAttributes.Add("RogueXinyongdian", characterBaseAttribute.rogueStartXinyongdian);
        characterValueAttributes.Add("RogueCharacterHealthUpperBound", characterBaseAttribute.rogueCharacterHealthUpperBoundBase);
        characterValueAttributes.Add("RogueCharacterCurDamage", characterBaseAttribute.rogueBulletDamage);
        attributesWithNoMoney.Add("RogueCharacterCurDamage");
        characterBaseWeapon = characterBaseAttribute.RogueCharacterBaseWeapon;
        characterIndex = index;
        this.player = player;
        characterIcon = Addressables.LoadAssetAsync<Sprite>(characterBaseAttribute.rogueCharacterIconLink)
            .WaitForCompletion();

        curBulletPrefab = Addressables.LoadAssetAsync<GameObject>("BasicBullet").WaitForCompletion();
        GiveOutOriginThing(characterBaseAttribute);
    }

    public void ShowHeartAndShield(bool show)
    {
        if (attributePanel != null)
        {
            attributePanel.uiTool.Get<HRogueAttributeBaseLogic>().ShowHeartAndShield(show);
        }
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
            //Debug.Log("attributePanel1!!!!!");
            attributePanel.uiTool.Get<HRogueAttributeBaseLogic>().SetRogueAttributeText();
            attributePanel.uiTool.Get<HRogueAttributeBaseLogic>().SetHealthAndShieldOnUI();
        }
        UpdateCharacterStateMachine();
    }

    private void UpdateCharacterStateMachine()
    {
        player.GetComponent<HPlayerStateMachine>().SetRunMultiplierSpeed(characterValueAttributes["RogueMoveSpeed"]);
    }
    
    public Sprite GetCharacterIcon()
    {
        return characterIcon;
    }

    public void AddAttributeValue(string name, float value)
    {
        characterValueAttributes[name] += value;
        if(characterValueAttributes[name] < 1.0f)  //角色的各种普通属性的最小值是1
        {
            characterValueAttributes[name] = 1.0f;
        }

        //todo:未来的可扩展性，比如说有的角色的伤害是根据当前的血量来决定的，那么这里就要做一些特残的处理
        if (name == "RogueBulletDamage")
        {
            characterValueAttributes["RogueCharacterCurDamage"] = characterValueAttributes[name];
        }
        UpdateEverythingInAttributePanel();
    }

    # region test Area
    public string thisname;
    public int thisvalue;
    public bool testHeartAndShield = false;
    public bool testGivemeFixedItem = false;
    public string testFixedItemId;
    public void TestHeartAndShield()
    {
        AddHeartOrShield(thisname, thisvalue);
    }

    private void Update()
    {
        if (testHeartAndShield)
        {
            TestHeartAndShield();
            testHeartAndShield = false;
        }

        if (testGivemeFixedItem)
        {
            GiveOutAnFixedItem(testFixedItemId);
            testGivemeFixedItem = false;
        }
    }
    
    #endregion


    public void ChangeHealth(int value)
    {
        if (value < 0)
        {
            HRogueCameraManager.Instance.ShakeCamera(10,0.2f);
        }
        AddHeartOrShield("Health", value);
    }
    public bool AddHeartOrShield(string name, int value)
    {
        //暂时逻辑简单起见，道具直接销毁，不考虑血量到上限的时候捡不起来，默认都能捡起来
        if (name == "Heart")
        {
            characterValueAttributes["RogueCharacterHealth"] += value;
            //clamp
            characterValueAttributes["RogueCharacterHealth"] = Mathf.Clamp(characterValueAttributes["RogueCharacterHealth"], 1, characterValueAttributes["RogueCharacterHealthUpperBound"]);
            //Debug.Log("Heart!!!!" + characterValueAttributes["RogueCharacterHealth"]);
        }
        else if (name == "Shield")
        {
            characterValueAttributes["RogueCharacterShield"] += value;
            characterValueAttributes["RogueCharacterHealth"] = Mathf.Clamp(characterValueAttributes["RogueCharacterHealth"], 0, 24);
        }
        else if(name == "HeartUpperBound")
        {
            characterValueAttributes["RogueCharacterHealthUpperBound"] += value;
            characterValueAttributes["RogueCharacterHealthUpperBound"] = Mathf.Clamp(characterValueAttributes["RogueCharacterHealthUpperBound"], 2, 24);
            characterValueAttributes["RogueCharacterHealth"] = Mathf.Clamp(characterValueAttributes["RogueCharacterHealth"], 2, characterValueAttributes["RogueCharacterHealthUpperBound"]);
        }
        else if (name == "Health")
        {
            //直接对Health值修改，如果value>0，则加RogueCharacterHealth，否则如果value<0，优先扣除RogueCharacterShield，没有的话再扣除RogueCharacterHealth
            if (value > 0)
            {
                characterValueAttributes["RogueCharacterHealth"] += value;
                characterValueAttributes["RogueCharacterHealth"] = Mathf.Clamp(characterValueAttributes["RogueCharacterHealth"], 0, characterValueAttributes["RogueCharacterHealthUpperBound"]);
            }
            else
            {
                if (characterValueAttributes["RogueCharacterShield"] > 0) //简单点的逻辑，护盾直接可以抵挡致命伤害，比如说有3点盾，但是受到了20点伤害，也是先只扣三点盾
                {
                    characterValueAttributes["RogueCharacterShield"] += value;
                    characterValueAttributes["RogueCharacterShield"] = Mathf.Clamp(characterValueAttributes["RogueCharacterShield"], 0, 24);
                }
                else  //没有盾，开始扣心
                {
                    characterValueAttributes["RogueCharacterHealth"] += value;
                    characterValueAttributes["RogueCharacterHealth"] = Mathf.Clamp(characterValueAttributes["RogueCharacterHealth"], -1, characterValueAttributes["RogueCharacterHealthUpperBound"]);
                }
            }
        }
        UpdateEverythingInAttributePanel();
        if(characterValueAttributes["RogueCharacterHealth"] <= 0 || characterValueAttributes["RogueCharacterHealthUpperBound"] <= 0)
        {
            SetPlayerDie();
        }
        
        return true;
    }

    private void SetPlayerDie()
    {
        if (player.GetComponent<HPlayerStateMachine>().IsDie) return;
        player.GetComponent<HPlayerStateMachine>().IsDie = true;
        player.GetComponent<HTestCharacterShoot>().SetPlayerDie();
    }

    //随机roll出一个道具，指定其父节点（也就算是生成位置）
    public void RollingARandomItem(Transform transform)
    {
        int index = UnityEngine.Random.Range(0, yPlanningTable.Instance.rogueItemBases.Count);
        string itemId = yPlanningTable.Instance.rogueItemKeys[index];
        RogueItemBaseAttribute rogueItemBaseAttribute = yPlanningTable.Instance.rogueItemBases[itemId];
        string itemPrefabLink = rogueItemBaseAttribute.rogueItemPrefabLink;
        GameObject item = Addressables.InstantiateAsync(itemPrefabLink, transform).WaitForCompletion();
        item.GetComponent<HRogueItemBase>().SetItemIDAndShow(itemId, rogueItemBaseAttribute);
    }
    public GameObject RollingARandomItem(Transform transform,Vector3 biasposition)
    {
        int index = UnityEngine.Random.Range(0, yPlanningTable.Instance.rogueItemBases.Count);
        string itemId = yPlanningTable.Instance.rogueItemKeys[index];
        RogueItemBaseAttribute rogueItemBaseAttribute = yPlanningTable.Instance.rogueItemBases[itemId];
        string itemPrefabLink = rogueItemBaseAttribute.rogueItemPrefabLink;
        GameObject item = Addressables.InstantiateAsync(itemPrefabLink, transform).WaitForCompletion();
        item.transform.position += biasposition;
        item.GetComponent<HRogueItemBase>().SetItemIDAndShow(itemId, rogueItemBaseAttribute);
        return item;
    }
    
    public void RollingARandomItem(Transform transform,Vector3 biasposition,bool isShop, string buyCurrency, int howMuch)
    {
        int index = UnityEngine.Random.Range(0, yPlanningTable.Instance.rogueItemBases.Count);
        string itemId = yPlanningTable.Instance.rogueItemKeys[index];
        RogueItemBaseAttribute rogueItemBaseAttribute = yPlanningTable.Instance.rogueItemBases[itemId];
        string itemPrefabLink = rogueItemBaseAttribute.rogueItemPrefabLink;
        GameObject item = Addressables.InstantiateAsync(itemPrefabLink, transform).WaitForCompletion();
        item.transform.position += biasposition;
        //item.GetComponent<HRogueItemBase>().SetItemIDAndShow(itemId, rogueItemBaseAttribute);
        item.GetComponent<HRogueItemBase>().SetItemIDAndShow(itemId, rogueItemBaseAttribute,isShop,buyCurrency,howMuch);
    }
    

    public void GiveOutAnFixedItem(string itemId)
    {
        RogueItemBaseAttribute rogueItemBaseAttribute = yPlanningTable.Instance.rogueItemBases[itemId];
        string itemPrefabLink = rogueItemBaseAttribute.rogueItemPrefabLink;
        GameObject item = Addressables.InstantiateAsync(itemPrefabLink, player.transform).WaitForCompletion();
        item.GetComponent<HRogueItemBase>().SetItemIDAndShow(itemId, rogueItemBaseAttribute);
    }
    public GameObject GiveOutAnFixedItem(string itemId,Transform transform,Vector3 biasposition)
    {
        RogueItemBaseAttribute rogueItemBaseAttribute = yPlanningTable.Instance.rogueItemBases[itemId];
        string itemPrefabLink = rogueItemBaseAttribute.rogueItemPrefabLink;
        GameObject item = Addressables.InstantiateAsync(itemPrefabLink, transform).WaitForCompletion();
        item.transform.position += biasposition;
        item.GetComponent<HRogueItemBase>().SetItemIDAndShow(itemId, rogueItemBaseAttribute);
        return item;
    }
    public void GiveOutAnFixedItem(string itemId,Transform transform,Vector3 biasposition,bool isShop, string buyCurrency, int howMuch)
    {
        RogueItemBaseAttribute rogueItemBaseAttribute = yPlanningTable.Instance.rogueItemBases[itemId];
        string itemPrefabLink = rogueItemBaseAttribute.rogueItemPrefabLink;
        GameObject item = Addressables.InstantiateAsync(itemPrefabLink, transform).WaitForCompletion();
        item.transform.position += biasposition;
        item.GetComponent<HRogueItemBase>().SetItemIDAndShow(itemId, rogueItemBaseAttribute,isShop,buyCurrency,howMuch);
    }


    public void UsePositiveItem(string id)
    {
        var positiveItem = yPlanningTable.Instance.rogueItemBases[id];
        string funcName = positiveItem.rogueItemFunc;
        string funcParams = positiveItem.rogueItemFuncParams;
        //找到HRogueItemBase类型的类（不要用new的语法），调用funcName的函数，把funcParams传入进去
        System.Reflection.MethodInfo method = this.GetType().GetMethod(funcName);
        method.Invoke(this, new object[] {funcParams});
    }

    public void GiveOutRuanmeiItem(string funcParams)
    {
        StartCoroutine(GiveOutItemForRuanmei(funcParams));
    }

    public void RollingAnItemThenUseImmediately(string id)
    {
        GameObject itemTmp = GiveOutAnFixedItem(id,player.transform,new Vector3(0,1000f,0));
        HRogueItemBase itemBase = itemTmp.GetComponent<HRogueItemBase>();
        itemBase.GetToBagAndShowEffects();
    }

    private IEnumerator GiveOutItemForRuanmei(string funcParams)
    {
        string[] paramList = funcParams.Split('!');
        for (int i = 0; i < paramList.Length; i++)
        {
            string xingshen = paramList[i];
            foreach(var item in yPlanningTable.Instance.rogueItemBases)
            {
                if (item.Value.rogueItemFollowXingshen == xingshen) 
                {
                    GameObject itemTmp = GiveOutAnFixedItem(item.Key,player.transform,new Vector3(0,1000f,0));
                    HRogueItemBase itemBase = itemTmp.GetComponent<HRogueItemBase>();
                    itemBase.GetToBagAndShowEffects();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
    
    public void ShowAllNegativeItemName(string funcParams)
    {
        Debug.Log("now we are in ShowAllNegativeItemNameFunc");
        foreach (var item in yPlanningTable.Instance.rogueItemBases)
        {
            if (item.Value.rogueItemKind == "Negative")
            {
                item.Value.rogueItemNameShowDefault = true;
            }
        }
    }
    
    public void ShowEffectWithNameAndTime(string effectNameAndTime)
    {
        string[] effectNameAndTimeArray = effectNameAndTime.Split(';');
        string effectName = effectNameAndTimeArray[0];
        float effectTime = float.Parse(effectNameAndTimeArray[1]);
        //用反射找到effectName对应的函数
        System.Reflection.MethodInfo method = this.GetType().GetMethod(effectName);
        StopCoroutine((IEnumerator)method.Invoke(this, new object[] {effectTime}));
        StartCoroutine((IEnumerator)method.Invoke(this, new object[] {effectTime}));
    }

    public void Heiyuanbaihua(string funcParams)
    {
        //todo:还未完成
        Debug.Log("Heiyuanbaihua!!!!");
    }

    public void HurtEveryEnemyInRoom(string funcParams)
    {
        //todo:还未完成
        Debug.Log("HurtEveryEnemyInRoom!!!!");
    }
    
    public void FrozenRoomEnemy(string funcParams)
    {
        //todo:还未完成
        Debug.Log("FrozenRoomEnemy!!!!");
    }

    public IEnumerator RotatePlayer(float lastTime)
    {
        Debug.Log("RotatePlayer!!!!");
        //保存Player的旋转参数，然后旋转180度，过lastTime之后复原
        player.transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(lastTime);
        player.transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
    }

    public void ReplaceCurBulletType(string type)
    {
        var op = Addressables.LoadAssetAsync<GameObject>(type).WaitForCompletion();
        curBulletPrefab = op;
    }

    public void AddBulletType(string type)
    {
        var op = Addressables.LoadAssetAsync<GameObject>(type).WaitForCompletion();
        bulletPrefabs.Add(op);
        bulletPrefabLength++;
    }

    public GameObject GetRandomCurBulletPrefab()
    {
        //利用random的特性，有90%的基础概率直接是curcurBulletPrefab，10%的概率是bulletPrefabs中的随机一个
        if (bulletPrefabLength==0 || UnityEngine.Random.Range(0, 100) < 80)
        {
            return curBulletPrefab;
        }
        else
        {
            return bulletPrefabs[UnityEngine.Random.Range(0, bulletPrefabLength)];
        }
    }

    

}
