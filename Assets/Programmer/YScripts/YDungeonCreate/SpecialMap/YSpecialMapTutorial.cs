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
    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
        portalToSomeWhere_ExitPortal.OnPlayerPortal += playerExitTutorial;
    }
    public void playerEnterTutorial()
    {
        
        
        GameObject tutorialPanel = Addressables.LoadAssetAsync<GameObject>(addLink).WaitForCompletion();
        //寻找并挂在Canvas下
        
        tutorialPanelInstance = Instantiate(tutorialPanel, canvas);
        GenerateMonster();
    }
    public void playerExitTutorial()
    {
        
        
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
