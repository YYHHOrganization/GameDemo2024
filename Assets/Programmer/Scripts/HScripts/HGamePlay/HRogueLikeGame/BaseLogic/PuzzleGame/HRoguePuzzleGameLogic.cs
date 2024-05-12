using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HRoguePuzzleGameLogic : MonoBehaviour
{
    public int puzzleSizeWidth = 6;

    public int puzzleSizeHeight = 4;

    public int puzzleToPickNumber = 8; //从所有的碎片中选出一部分碎片放到拼图区域

    public Transform puzzleFragmentsIrea; //拼图碎片所在的区域，会随机抽出一部分碎片放到拼图区域
    private List<Transform> puzzleFragements = new List<Transform>();  //所有的拼图碎片
    private Dictionary<string, Vector3> puzzleCorrectPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, int> correctRotationCnts = new Dictionary<string, int>();
    private Dictionary<string, int> currentRotationCnts = new Dictionary<string, int>();
    
    private List<Transform> puzzleToBeSet = new List<Transform>();
    
    private string puzzleRootName = "Puzzle";
    private int correctPuzzleCnt = 0;
    
    private void Start()
    {
        SetTextureForPuzzle();
        SetAPuzzleGame();
        puzzleRootName = "Puzzle" + UnityEngine.Random.Range(0, 29999);
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
            puzzleCorrectPositions.Add(puzzleRootName + i, new Vector3(puzzleFragements[randomIndex].position.x, puzzleFragements[randomIndex].position.y, puzzleFragements[randomIndex].position.z));
            puzzleFragements.RemoveAt(randomIndex);
        }

        float localStartPosX = 0;
        float localStartPosZ = 0;
        //把选出来的碎片放到待拼图区域，按照每行两个向下进行排列
        for (int i = 0; i < puzzleToBeSet.Count; i++)
        {
            puzzleToBeSet[i].SetParent(puzzleFragmentsIrea);
            localStartPosX = (i % 2 == 0)? 0: -1f;
            puzzleToBeSet[i].localPosition = new Vector3(localStartPosX, 0, localStartPosZ);
            if(i % 2 == 1)
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

    public void SetPuzzleToCorrectPlace(GameObject selectedObject)
    {
        if (!selectedObject) return;
        selectedObject.transform.DOMove(puzzleCorrectPositions[selectedObject.name], 0.5f);
        correctPuzzleCnt++;
        selectedObject.GetComponent<Collider>().enabled = false;
        if(correctPuzzleCnt == puzzleToPickNumber)
        {
            Debug.Log("拼图完成");
        }
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
