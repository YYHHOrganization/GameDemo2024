using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class HRogueCharacterInMusicGameVer1 : MonoBehaviour
{
    private int jumpHash;
    private int runHash;
    private int fallHash;

    private Animator animator;
    private bool isUp = false; //是否在上面的台阶上

    private GameObject goodPickupEffect;
    private GameObject badPickupEffect;

    private GameObject gameScorePanel;
    private Button startGameButton;
    private TMP_Text scoreText;
    private TMP_Text addOrMinusText;
    private TMP_Text comboText;
    private Transform countDownArea;
    private List<Transform> countDownNumbers = new List<Transform>();

    private HRogueMusicGame1Logic gameLogicScript;
    private int goodPickupCount = 0;  //玩家收集到的好道具的数量
    public int GoodPickupCount => goodPickupCount;
    private int scoreMultiplier = 1;  //分数的倍数
    private bool isInvincible = false;  //是否无敌
    
    public void SetScoreMultiplier(int value)
    {
        scoreMultiplier = value;
    }
    
    public void SetVincible(bool value)
    {
        isInvincible = value;
    }

    public void InstantiateGameScorePanel(HRogueMusicGame1Logic logic)
    {
        gameScorePanel = Instantiate(gameScorePanel,GameObject.Find("Canvas").transform);
        scoreText = gameScorePanel.transform.Find("ScoreText").GetComponent<TMP_Text>();
        addOrMinusText = gameScorePanel.transform.Find("AddOrMinusText").GetComponent<TMP_Text>();
        comboText = gameScorePanel.transform.Find("ComboText").GetComponent<TMP_Text>();
        startGameButton = gameScorePanel.transform.Find("StartGameButton").GetComponent<Button>();
        countDownArea = gameScorePanel.transform.Find("CountDownArea");
        for (int i = 0; i < countDownArea.childCount; i++)
        {
            countDownNumbers.Add(countDownArea.GetChild(i));
        }
        startGameButton.onClick.AddListener(StartMusicGame);
        gameLogicScript = logic;
    }

    public void DestroyGameScorePanel()
    {
        Destroy(gameScorePanel);
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        jumpHash = Animator.StringToHash("isJumping");
        runHash = Animator.StringToHash("isRunning");
        fallHash = Animator.StringToHash("isFalling");
        isUp = false;
        
        goodPickupEffect = Addressables.LoadAssetAsync<GameObject>("Lihua1New").WaitForCompletion();
        badPickupEffect = Addressables.LoadAssetAsync<GameObject>("BaozhaSmall").WaitForCompletion();
        
        gameScorePanel = Addressables.LoadAssetAsync<GameObject>("GameScorePanel").WaitForCompletion();
    }

    private void StartMusicGame()
    {
        StartCoroutine(TickCountDown(3));
        //开启三秒钟的倒计时，然后开始游戏
        DOVirtual.DelayedCall(4f, () =>
        {
            animator.SetBool(runHash, true);
            YTriggerEvents.OnInterruptCombo += CancelCombo;
            startGameButton.gameObject.SetActive(false);
            gameLogicScript.StartGameLogic();
        });
    }

    IEnumerator TickCountDown(int number)
    {
        for (int i = 0; i < number; i++)
        {
            countDownNumbers[i%3].gameObject.SetActive(true);
            countDownNumbers[i%3].DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce);
            countDownNumbers[i%3].GetComponentInChildren<TMP_Text>().text = (number - i).ToString();
            yield return new WaitForSeconds(1f);
            countDownNumbers[i%3].gameObject.SetActive(false);
        }
        countDownNumbers[3].gameObject.SetActive(true);
        countDownNumbers[3].DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);
        countDownNumbers[3].gameObject.SetActive(false);
    }
    

    private void CheckPlayerUpOrDown()
    {
        if (Input.GetKeyDown(KeyCode.Space))  //按下一次空格键，切换到另一条轨道
        {
            isUp = !isUp;
            MovePlayer(isUp);
        }
    }

    private void MovePlayer(bool isUp)
    {
        if (isUp)
        {
            transform.DOLocalJump(new Vector3(0, 1.65f, 0), 1f, 1, 0.1f);
            animator.SetTrigger(jumpHash);
        }
        else
        {
            transform.DOLocalJump(new Vector3(0, 0f, 0), 1f, 1, 0.1f);
            animator.SetTrigger(fallHash);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        CheckPlayerUpOrDown();
    }

    private void CancelCombo(object sender, YTriggerEventArgs e)  //取消Combo的显示
    {
        HPostProcessingFilters.Instance.SetPostProcessingWithNameAndValue("Vignette", 0);
        //Debug.Log("CancelCombo!!");
        comboNum = 0;
        comboText.text = "";
    }

    private int ComboNumToScore()
    {
        int score = 1;
        if(comboNum>15) score = 2 + (comboNum - 15) / 15;
        return score * scoreMultiplier;
    }

    private int comboNum = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  //没有miss，加分
        {
            GameObject tmpVFX = Instantiate(goodPickupEffect, transform.position, Quaternion.identity);
            tmpVFX.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            comboNum++;
            goodPickupCount++;
            HRogueCameraManager.Instance.ShakeCamera(2f, 0.1f);
            HAudioManager.Instance.Play("RogueMusicGamePickUpAudio", gameObject);
            int score = ComboNumToScore();
            //Debug.Log("thisItem!!!!" + other.gameObject.name);
            UpdateScoreAndShowInUI(score);
            Destroy(other.gameObject);
            Destroy(tmpVFX, 10f);
        }
        else if (other.gameObject.CompareTag("Enemy"))  //碰到类似障碍物的东西，减分；如果错过了加分项，就不加不减
        {
            GameObject tmpVFX = Instantiate(badPickupEffect, transform.position, Quaternion.identity);
            HRogueCameraManager.Instance.ShakeCamera(3f, 0.2f);
            Destroy(other.gameObject);
            Destroy(tmpVFX, 10f);
            
            if (isInvincible) return;
            HPostProcessingFilters.Instance.SetPostProcessingWithNameAndValue("Vignette", 0);
            comboNum=0;
            UpdateScoreAndShowInUI(-1);
        }
        else if (other.gameObject.CompareTag("CouldHit")) //这种对应长条，不播放音效，不震动屏幕
        {
            GameObject tmpVFX = Instantiate(goodPickupEffect, transform.position, Quaternion.identity);
            tmpVFX.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            comboNum++;
            goodPickupCount++;
            int score = ComboNumToScore();
            //Debug.Log("thisItem!!!!" + other.gameObject.name);
            UpdateScoreAndShowInUI(score);
            Destroy(other.gameObject);
            Destroy(tmpVFX, 10f);
        }
    }

    public void ShowGameOverEffect()
    {
        //在transform.position周围一圈生成特效
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            GameObject tmpVFX = Instantiate(goodPickupEffect, transform.position + randomPos, Quaternion.identity);
            Destroy(tmpVFX, 10f);
        }
        
    }
    
    private void CheckComboNumAndGiveoutEffects()
    {
        //comboNum比较大的时候有一些别的效果，比如加分翻倍
        if(comboNum >= 5)
        {
            float intensity =  0.2f * (comboNum / 5);
            if (intensity >= 0.4) intensity = 0.4f;
            HPostProcessingFilters.Instance.SetPostProcessingWithNameAndValue("Vignette", intensity);
        }

        if (comboNum>=30 && comboNum % 30 == 0)
        {
            gameLogicScript.GiveOutTwoCharactersForCheer();
        }
    }

    int totalScore = 0;
    private void UpdateScoreAndShowInUI(int score)
    {
        totalScore += score;
        if(totalScore <= 0) //分数不能为负数
        {
            totalScore = 0;
        }

        CheckComboNumAndGiveoutEffects();
        
        if (score > 0)
        {
            addOrMinusText.text = "+" + score;
            addOrMinusText.transform.DOShakePosition(0.3f, 2f);
            DOVirtual.DelayedCall(0.4f, () =>
            {
                addOrMinusText.text = "";
                scoreText.text = totalScore.ToString();
            });
            if (comboNum >= 5)  //如果连击数大于等于2，就显示连击数
            {
                comboText.text = "Combo x" + comboNum;
                comboText.transform.DOShakePosition(0.3f, 2f);
                DOVirtual.DelayedCall(0.4f, () =>
                {
                    comboText.text = "";
                });
            }
        }
        else  //现在是减分
        {
            addOrMinusText.text = score.ToString();
            addOrMinusText.transform.DOShakeScale(0.3f, 2f);
            DOVirtual.DelayedCall(0.4f, () =>
            {
                addOrMinusText.text = "";
                scoreText.text = totalScore.ToString();
            });
        }
    }
}
