using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSginalController : MonoBehaviour
{
    private GameObject go;
    List<string> animationChooseList;
    // Start is called before the first frame update
    void Start()
    {
        go = GameObject.Find("YSpotLightInStreetLight");
        if(go==null)
        {
            Debug.Log("go is null");
        }
        else
        {
            Debug.Log("go is not null");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    bool isgetFlag = false;
    public void testSignal(int clipIndex)
    {
        if (!isgetFlag)
        {
            animationChooseList = yPlanningTable.Instance.getAnimationChooseList();
            isgetFlag = true;
        }
        
        Debug.Log("testsignal!!!"+clipIndex);
        //此句中应该先是否这个是否释放了“交互”的动作，简单的方法可以通过查询yplanning表里面是否交互了
        if (animationChooseList[clipIndex] == "ButtonPushing")
        {
            Debug.Log("jiaohu！！"+clipIndex);
        }

        
       
        
        // Debug.Log("testsignal!!!"+clipIndex);
        // if (clipIndex%2==0)
        // {
        //     go.SetActive(false);
        // }
        // else if (clipIndex%2== 1)
        // {
        //     go.SetActive(true);
        // }
        
    }
    //开始交互
    void StartInteract()
    {
        if(go.activeSelf)
        {
            go.SetActive(false);
        }
        else
        {
            go.SetActive(true);
        }
    }
}
