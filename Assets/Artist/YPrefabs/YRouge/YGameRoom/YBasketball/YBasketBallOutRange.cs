using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YBasketBallOutRange : MonoBehaviour
{
    //
    public Transform center;
    public List<GameObject> balls;
    public Transform throwPos;
    bool isInMisson = false;
    public float range = 1.5f;
    public Transform shootTarget;
    // Start is called before the first frame update
    void Start()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            EnterMisson();
        });
    }
    Tween throwBallTween;
    Tween setfalseBallTween;
    public void EnterMisson()
    {
        isInMisson = true;
        //开始出球
        //每三秒出一个球
        throwBallTween = DOVirtual.DelayedCall(5f, () =>
        {
            ThrowBall();
        }).SetLoops(50);
        DOVirtual.DelayedCall(100f, () =>
        {
            setfalseBall();
        });
    }
    
    void setfalseBall()
    {
        //然后过一段时间开始 对于每个球，都会3s后自动set false 最先放进balls里面的球最先set false
        setfalseBallTween=DOVirtual.DelayedCall(5f, () =>
        {
            if (balls.Count > 0)
            {
                balls[0].SetActive(false);
                balls.RemoveAt(0);
            }
        }).SetLoops(50);
    }
    public void ExitMisson()
    {
        isInMisson = false;
        throwBallTween.Kill();
        setfalseBallTween.Kill();
        //结束出球
        //停止出球，并将球都set false
    }
    
    public string fallingballPrefabID = "33310003";
    //抛物线出球
    public void ThrowBall()
    {
        //抛物线出球
        GameObject curBall1 = YObjectPool._Instance.Spawn(fallingballPrefabID);
        balls.Add(curBall1);
        curBall1.transform.position = throwPos.position;
        curBall1.SetActive(true);
        StartCoroutine(BulletThrowOut(curBall1));
        
        // StartCoroutine(IsBallinRange(curBall1));
    }
    IEnumerator IsBallinRange(GameObject curBall)
    {
        yield return new WaitForSeconds(0.1f);
        if (Mathf.Sqrt(curBall.transform.position.x * curBall.transform.position.x + curBall.transform.position.z * curBall.transform.position.z) > range)
        {
            curBall.SetActive(false);
            ThrowBall();
            
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (isInMisson)
    //     {
    //         //判断球是不是在范围内，不在范围内就set false
    //         if (curBall != null)
    //         {
    //             if (Mathf.Sqrt(curBall.transform.position.x * curBall.transform.position.x + curBall.transform.position.z * curBall.transform.position.z) > range)
    //             {
    //                 curBall.SetActive(false);
    //                 ThrowBall();
    //             }
    //         }
    //     }
    // }
    
    IEnumerator BulletThrowOut(GameObject curBall)
    {
        if (shootTarget != null)
        {
            //朝着target沿着抛物线扔出子弹
            Vector3 targetPos = shootTarget.position+new Vector3(Random.Range(-5f,5f),0,Random.Range(-5f,5f));
            Vector3 startPos = curBall.transform.position;
            float height = 3f;
            float duration = 1f;
            float time = 0;
            while (time <= 1)
            {
                time += Time.deltaTime / duration;
                float yOffset = height * 4 * (time - time * time);
                curBall.transform.position = Vector3.Lerp(startPos, targetPos, time) + yOffset * Vector3.up;
                yield return null;
            }
           
        }
        //Destroy(this.gameObject, 5f);
    }
}
