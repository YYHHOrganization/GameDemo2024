using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class YRecallSkill : MonoBehaviour
{
    protected L2PlayerInput playerInput;
    private YRecallObjectPool recallObjectPool;
    private bool beginDetect;

    private Camera playerCamera = null;
    
    YCountDownUI countDownUI;
    private float duration = 20f;
    public void setPool(YRecallObjectPool yRecallObjectPool)
    {
        recallObjectPool = yRecallObjectPool;
    }
   
    // Start is called before the first frame update
    void Start()
    {
        playerInput = new L2PlayerInput();
        countDownUI = gameObject.AddComponent<YCountDownUI>();
        countDownUI.addCountDownUIlink = "RecallCountDownPanel";
        countDownUI.skillLastTime = duration ;
        //playerInput.CharacterControls.Skill2.started +=context =>  BeginRecallSkill();
    }

    //33310000,炸弹可时间回溯,BombCouldRecallFromPool,10,0 没用到
    
    private bool duringRecall;
    //王国之泪出recall 的skill的条件：1.到时间了。2.手动停止。
    //简易点，可以先实现到时间停止，
    public void BeginRecallSkill()
    {
        Time.timeScale = 0;
        // YTriggerEvents.RaiseOnMouseLeftShoot(false);
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        
        playerCamera = HCameraLayoutManager.Instance.playerCamera;
        //将池子里的所有可以倒转的（先用个list也行）东西，高亮显示
        foreach (GameObject obj in recallObjectPool.recallableObjectPool)
        {
            YRecallable recallable = obj.GetComponent<YRecallable>();
            if (recallable != null)
            {
                recallable.SetCouldRecall();
            }
        }
        //不能移动，且停止射击
        //并且此时进入瞄准寻找目标的状态，如果找到了目标，中心点悬停在那里，就调用他的recallable.ChooseRecallTail();
        //todo：从屏幕中心射出射线寻找目标
        beginDetect = true;

        //然后如果点击的话，那个玩意就回溯
    }
    // 如果射线与物体相交
    YRecallable preRecallable=null;
    YRecallable recallable = null;
    private GameObject target = null;
    private void BeginDetectObject()
    {
        // 从摄像头位置创建射线
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // 改为鼠标悬停的地方检测是否有合适的物体
        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject newTarget = hitInfo.collider.gameObject;
            YRecallable newRecallable = newTarget.GetComponent<YRecallable>();

            // 如果是同一个物体，则不清除上一个轨迹
            if (newRecallable != null)
            {
                if (newRecallable != recallable)
                {
                    preRecallable?.ClearRecallTail(); // 清除上一个物体的轨迹
                    newRecallable.ChooseRecallTail(); // 设置新物体的轨迹
                    preRecallable = newRecallable;
                    recallable = newRecallable;
                }
                // 如果点击的是同一个物体，保持不变
            }
            else
            {
                preRecallable?.ClearRecallTail(); // 清除上一个物体的轨迹
                recallable = null;
            }

            target = newTarget;
        }
        else
        {
            preRecallable?.ClearRecallTail(); // 没有击中任何物体，清除上一个物体的轨迹
            recallable = null;
            target = null;
        }
    }

    private bool isRecalling=false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isRecalling)
            {
                isRecalling = true;
                BeginRecallSkill();
            }
            else
            {
                isRecalling = false;
                EndRecall();
            }
        }

        if (!duringRecall&&beginDetect)
        {
            BeginDetectObject();

            if (recallable != null&&Input.GetKeyDown(KeyCode.Mouse0))
            {
                Recall();
            }
        }
    }
    
    void Recall()
    {
        Time.timeScale = 1;
        
        Debug.Log(">>_<<Begin Recall!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //搞个计时器，
        countDownUI.BeginCountDown();
        CountDownCoroutine = StartCoroutine(CountDown(duration));
        
        
        duringRecall = true;
        recallable.Recalling(duration);
    }
    private Coroutine CountDownCoroutine;
    public IEnumerator CountDown(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndRecall();
    }
    void EndRecall()
    {
        if(CountDownCoroutine!=null)StopCoroutine(CountDownCoroutine);
        
        //如果进入了选择物体界面再退出 已经选中了
        Time.timeScale = 1;
        Debug.Log("()()()()END Recall!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        countDownUI.EndCountDown();
        
        //将所有复原  包括shader，然后进行到一半的recall先停下
        // recallable.EndRecall();
        
        foreach (GameObject obj in recallObjectPool.recallableObjectPool)
        {
            YRecallable recallable = obj.GetComponent<YRecallable>();
            if (recallable != null)
            {
                recallable.EndRecall();
            }
        }
        
        duringRecall = false;
        beginDetect = false;
    }

    
}
