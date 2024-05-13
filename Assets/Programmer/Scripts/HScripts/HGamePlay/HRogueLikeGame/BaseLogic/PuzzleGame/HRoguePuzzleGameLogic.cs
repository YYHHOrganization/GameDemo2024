using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HRoguePuzzleGameLogic : MonoBehaviour
{
    public int puzzleSizeWidth = 6;

    public int puzzleSizeHeight = 4;

    public int puzzleToPickNumber = 8; //从所有的碎片中选出一部分碎片放到拼图区域

    public Transform puzzleFragmentsIrea; //拼图碎片所在的区域，会随机抽出一部分碎片放到拼图区域
    private List<Transform> puzzleFragements = new List<Transform>(); //所有的拼图碎片
    private Dictionary<string, Vector3> puzzleCorrectPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, int> correctRotationCnts = new Dictionary<string, int>();
    private Dictionary<string, int> currentRotationCnts = new Dictionary<string, int>();

    private List<Transform> puzzleToBeSet = new List<Transform>();

    private string puzzleRootName = "Puzzle";
    private int correctPuzzleCnt = 0;
    private Button startGameButton;
    private GameObject gameScorePanel;

    private void Start()
    {
        LoadGame();
    }

    private void LoadGame()
    {
        SetTextureForPuzzle();
        PrepareForGame();
        puzzleRootName = "Puzzle" + UnityEngine.Random.Range(0, 29999);
    }

    private void PrepareForGame()
    {
        SetExitPanelFalse();
        YPlayModeController.Instance.LockPlayerInput(true);
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
        //要显示开始游戏的UI
        Invoke("SetGamePanelActive", 2.5f);
    }

    private void SetTextureForPuzzle()
    {
        int randomIndex = UnityEngine.Random.Range(0, 5);
        string texturePath = "Textures/RogueLikeGame/PuzzleGame/Normal/PuzzleGameNormal" + randomIndex;
        if (yPlanningTable.Instance == null || yPlanningTable.Instance.isCultural)
        {
            texturePath = "Textures/RogueLikeGame/PuzzleGame/Cultural/PuzzleGameCultural" + randomIndex;
        }

        Texture texture = Resources.Load<Texture>(texturePath);
        foreach (Transform child in transform)
        {
            child.GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
    }

    //拼图游戏的基本逻辑，包含拼图的生成，拼图的拼接，拼图的判断等等
    private void SetAPuzzleGame()
    {
        InitializePuzzleFragments();
        RollingPuzzleFragments();
    }

    private List<Transform> countDownNumbers = new List<Transform>();

    private void SetGamePanelActive()
    {
        gameScorePanel = Addressables.LoadAssetAsync<GameObject>("GameScorePanel").WaitForCompletion();
        gameScorePanel = Instantiate(gameScorePanel, GameObject.Find("Canvas").transform);
        startGameButton = gameScorePanel.transform.Find("StartGameButton").GetComponent<Button>();
        Transform scoreText = gameScorePanel.transform.Find("ScoreText");
        Transform addOrMinusText = gameScorePanel.transform.Find("AddOrMinusText");
        Transform comboText = gameScorePanel.transform.Find("ComboText");
        Transform tip = gameScorePanel.transform.Find("ScoreTextTip");
        scoreText.gameObject.SetActive(false);
        addOrMinusText.gameObject.SetActive(false);
        comboText.gameObject.SetActive(false);
        tip.gameObject.SetActive(false);
        
        Transform countDownArea = gameScorePanel.transform.Find("CountDownArea");
        for (int i = 0; i < countDownArea.childCount; i++)
        {
            countDownNumbers.Add(countDownArea.GetChild(i));
        }

        startGameButton.onClick.AddListener(StartPuzzleGameCountDown);
    }

    private void StartPuzzleGameCountDown()
    {
        StartCoroutine(TickCountDown(3));
        //开启三秒钟的倒计时，然后开始游戏
        DOVirtual.DelayedCall(4f, () =>
        {
            startGameButton.gameObject.SetActive(false);
            SetAPuzzleGame();
            InitializeGameTickLogic();
        });
    }

    IEnumerator TickCountDown(int number)
    {
        for (int i = 0; i < number; i++)
        {
            countDownNumbers[i % 3].gameObject.SetActive(true);
            countDownNumbers[i % 3].DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce);
            countDownNumbers[i % 3].GetComponentInChildren<TMP_Text>().text = (number - i).ToString();
            yield return new WaitForSeconds(1f);
            countDownNumbers[i % 3].gameObject.SetActive(false);
        }

        countDownNumbers[3].gameObject.SetActive(true);
        countDownNumbers[3].DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);
        countDownNumbers[3].gameObject.SetActive(false);
    }

    private void SetExitPanelFalse()
    {
        //主要是角色的属性面板要保持关闭，还有小地图
        HRoguePlayerAttributeAndItemManager.Instance.SetAttributePanel(false);
        HCameraLayoutManager.Instance.SetLittleMapCamera(false);
    }

    private void InitializeGameTickLogic()
    {
        //游戏开始计时，如果时间到了，那么就结束游戏
        HMessageShowMgr.Instance.ShowTickMessage("拼图游戏倒计时：", 60, ShowGameOverMessage);
    }

    private void InitializePuzzleFragments()
    {
        // 把拼图的碎片加入到拼图碎片区域
        int allPieceNums = puzzleSizeWidth * puzzleSizeHeight;
        for (int i = 0; i < allPieceNums; i++)
        {
            puzzleFragements.Add(transform.GetChild(i));
            transform.GetChild(i).GetComponent<Collider>().enabled = false;
        }
    }

    private void RollingPuzzleFragments()
    {
        //随机选出一部分碎片放到拼图区域
        for (int i = 0; i < puzzleToPickNumber; i++)
        {
            int randomIndex = Random.Range(0, puzzleFragements.Count);
            puzzleToBeSet.Add(puzzleFragements[randomIndex]);
            puzzleCorrectPositions.Add(puzzleRootName + i,
                new Vector3(puzzleFragements[randomIndex].position.x, puzzleFragements[randomIndex].position.y,
                    puzzleFragements[randomIndex].position.z));
            puzzleFragements.RemoveAt(randomIndex);
        }

        float localStartPosX = 0;
        float localStartPosZ = 0;
        //把选出来的碎片放到待拼图区域，按照每行两个向下进行排列
        for (int i = 0; i < puzzleToBeSet.Count; i++)
        {
            puzzleToBeSet[i].SetParent(puzzleFragmentsIrea);
            localStartPosX = (i % 2 == 0) ? 0 : -1f;
            puzzleToBeSet[i].localPosition = new Vector3(localStartPosX, 0, localStartPosZ);
            if (i % 2 == 1)
            {
                localStartPosZ += 0.7f;
            }

            //随机旋转碎片
            int rotateCnt = Random.Range(0, 3);
            puzzleToBeSet[i].gameObject.name = puzzleRootName + i;

            puzzleToBeSet[i].transform.rotation = Quaternion.Euler(new Vector3(
                puzzleToBeSet[i].transform.rotation.eulerAngles.x,
                puzzleToBeSet[i].transform.rotation.eulerAngles.y - 90 * rotateCnt,
                puzzleToBeSet[i].transform.rotation.eulerAngles.z));

            correctRotationCnts.Add(puzzleRootName + i, rotateCnt);
            currentRotationCnts.Add(puzzleRootName + i, 0);
            puzzleToBeSet[i].tag = "DragThing";
            puzzleToBeSet[i].gameObject.GetComponent<Collider>().enabled = true;
            //puzzleToBeSet[i].gameObject.AddComponent<HRogueGrabberLogic>().SetCorrectAnswer(puzzleCorrectPositions[i], rotateCnt);
        }
    }

    public bool CheckIfAnswerIsRight(GameObject selectedObject)
    {
        if (!selectedObject) return false;
        string name = selectedObject.name;
        if (correctRotationCnts.ContainsKey(name))
        {
            if (correctRotationCnts[name] != currentRotationCnts[name])
            {
                return false;
            }

            //检查位置是否是正确的
            if (Vector3.Distance(selectedObject.transform.position, puzzleCorrectPositions[name]) < 0.2f)
            {
                Debug.Log("拼图正确");
                return true;
            }
        }

        return false;
    }

    private bool isFinishedPuzzleGame = false;

    public void SetPuzzleToCorrectPlace(GameObject selectedObject)
    {
        if (!selectedObject) return;
        selectedObject.transform.DOMove(puzzleCorrectPositions[selectedObject.name], 0.5f);
        correctPuzzleCnt++;
        selectedObject.GetComponent<Collider>().enabled = false;
        if (correctPuzzleCnt == puzzleToPickNumber)
        {
            Debug.Log("拼图完成");
            isFinishedPuzzleGame = true;
            ShowGameOverMessage();
        }
    }

    private void ShowGameOverMessage()
    {
        if (isEndGame) return;
        string overrideContent = "拼图游戏结束，点击确定退出";
        HMessageShowMgr.Instance.ShowMessageWithActions("ROGUE_MUSICGAME_Grade", EndGame, EndGame,EndGame, null, overrideContent);
    }

    private bool isEndGame = false;
    private void EndGame()
    {
        if (isEndGame) return;
        isEndGame = true;
        HRoguePlayerAttributeAndItemManager.Instance.SetAttributePanel(true);
        HCameraLayoutManager.Instance.SetLittleMapCamera(true);
        Destroy(gameScorePanel);
        YPlayModeController.Instance.LockPlayerInput(false);
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        SetGameOverAndGiveoutTreasure();
        //HAudioManager.Instance.Play("StartRogueAudio", HAudioManager.Instance.gameObject);
        HMessageShowMgr.Instance.RemoveTickMessage("拼图游戏倒计时：");
        Destroy(transform.parent.gameObject, 1f);
    }

    private void SetGameOverAndGiveoutTreasure()
    {
        //这里写给出宝箱的相关逻辑,根据拼图完成的难度和时间综合给出不同的奖励
        string chestID = "10000013";
        if (!isFinishedPuzzleGame) // 时间到了，还没有完成拼图
        {
            if(correctPuzzleCnt < puzzleToPickNumber / 2)
            {
                Debug.Log("拼图失败，给出一个小宝箱");
            }
            else
            {
                Debug.Log("拼图失败，给出一个中宝箱");
                chestID = "10000011";
            }
        }
        else //完成了拼图游戏
        {
            chestID = "10000012";
        }
        
        Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        Vector3 treasurePos = player.position;
        
        HOpenWorldTreasureManager.Instance.InstantiateATreasureAndSetInfoWithTypeId(chestID, treasurePos, transform.parent.parent);
        
    }

    public void SetRotationCnt(string name, int cnt)
    {
        if (currentRotationCnts.ContainsKey(name))
        {
            currentRotationCnts[name] = cnt;
        }
    }
    
    public void DebugRotationCnt(string name)
    {
        // Debug.Log("================================");
        // Debug.Log("currentRotationCnt: " + currentRotationCnts[name]);
        // Debug.Log("correctRotationCnt: " + correctRotationCnts[name]);
    }
    
    public int GetCurrentRotationCnt(string name)
    {
        if (currentRotationCnts.ContainsKey(name))
        {
            return currentRotationCnts[name];
        }
        return 0;
    }
    
}
