using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class YCountDownUI : MonoBehaviour
{
    protected GameObject countDownUI;
    protected TMP_Text countDownText;
    protected Image countDownImage;
    public string addCountDownUIlink = "RecallCountDownPanel";
    
    public float skillLastTime;  //技能的持续时间
    Coroutine countDownCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        GameObject countDownUIGO = Addressables.InstantiateAsync(addCountDownUIlink).WaitForCompletion();
        countDownUI = Instantiate(countDownUIGO, GameObject.Find("Canvas").transform);
        countDownUI.gameObject.SetActive(false);
        countDownText = countDownUI.GetComponentInChildren<TMP_Text>();
        countDownImage = countDownUI.transform.Find("skillIcon").GetComponent<Image>();
    }

    
    public void BeginCountDown()
    {
        countDownCoroutine = StartCoroutine(BeginCountDownCoroutine());
    }

    IEnumerator BeginCountDownCoroutine()
    {
        countDownUI.gameObject.SetActive(true);
        
        int tickCount = (int)(skillLastTime / 0.1f);
        for(int i = 0;i < tickCount;i++)
        {
            yield return new WaitForSeconds(0.1f);
            float remainTime = skillLastTime - i * 0.1f;
            countDownText.text = remainTime.ToString("F1");
            countDownImage.fillAmount = remainTime / skillLastTime;
        }
    }
    
    public void EndCountDown()
    {
        countDownUI.gameObject.SetActive(false);
        if(countDownCoroutine!=null)StopCoroutine(countDownCoroutine);
    }
}
