using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YSpecialMapTutorial : MonoBehaviour
{
    public Transform playerBornPlace;
    public Transform enemyBornPlace;
    [SerializeField]YPortalToSomeWhere portalToSomeWhere_ExitPortal;
    private string addLink = "TutorialPanelMoveHit";
    Transform canvas;
    GameObject tutorialPanelInstance;
    private GameObject enemy;
    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        portalToSomeWhere_ExitPortal.OnPlayerPortal += playerExitTutorial;
        enemy = Addressables.LoadAssetAsync<GameObject>("SlimeVeryCommonNoMove").WaitForCompletion();
    }
    public void playerEnterTutorial()
    {
        
        HLoadScriptManager.Instance.isInTutorial = true;
        GameObject tutorialPanel = Addressables.LoadAssetAsync<GameObject>(addLink).WaitForCompletion();
        //寻找并挂在Canvas下
        
        tutorialPanelInstance = Instantiate(tutorialPanel, canvas);
        GenerateMonster();
    }

    public void SummonEnemyForMission(int summonCnt)
    {
        //创建一个新的enemyBornPlace，空物体即可，和enemyBornPlace的位置一样
        //Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        GameObject enemyInstance = Instantiate(enemy, enemyBornPlace.position, Quaternion.identity, enemyBornPlace);
        enemyInstance.transform.localPosition = new Vector3(0, 5, 0);
        for (int i = 0; i < summonCnt - 1; i++)
        {
            DOVirtual.DelayedCall(3f, () =>
            {
                GameObject enemyInstance2 = Instantiate(enemy, enemyBornPlace.position, Quaternion.identity, enemyBornPlace);
                enemyInstance2.transform.localPosition = new Vector3(0, 5, 0);
            });

            //enemyInstance.transform.localPosition = Vector3.zero + new Vector3(-0.4f + 0.2f * i, 0, -0.4f + 0.2f * i);
            //enemyInstance.gameObject.AddComponent<EnemyDieAndSendMsg>();
        }
    }
    public void playerExitTutorial()
    {
        HLoadScriptManager.Instance.isInTutorial = false;
        Destroy(tutorialPanelInstance);
    }
    
    //生成怪物
    public void GenerateMonster()
    {
        GameObject enemy = Addressables.InstantiateAsync("SlimeVeryCommonNoMove").WaitForCompletion();
        GameObject enemyInstance = Instantiate(enemy, enemyBornPlace);
        enemyInstance.transform.localPosition = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
