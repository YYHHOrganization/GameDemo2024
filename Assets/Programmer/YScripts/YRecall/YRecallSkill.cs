using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class YRecallSkill : MonoBehaviour
{
    protected L2PlayerInput playerInput;
    private YRecallObjectPool recallObjectPool;
    private bool beginDetect;

    private Camera playerCamera = null;
    
    YCountDownUI countDownUI;
    private float duration = 20f;
    
    private bool duringRecall;
    HPlayerStateMachine playerStateMachine;

    private bool CanDoRecallSkill = false;

    private string YRecallLightHandVFXAddLink = "YRecallLightHandVFX";
    GameObject YRecallLightHandVFX;
    
    LayerMask layerMask = 0;
    
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

        GameObject YRecallLightHandVFXGO = Addressables.InstantiateAsync(YRecallLightHandVFXAddLink).WaitForCompletion();
        YRecallLightHandVFX = YRecallLightHandVFXGO;
        YRecallLightHandVFX.transform.position += new Vector3(0, -100, 0);
        
        YTriggerEvents.OnLoadEndAndBeginPlay += LoadEndAndBeginPlay;
        //playerInput.CharacterControls.Skill2.started +=context =>  BeginRecallSkill();
        
        //layer上   去掉ignoreBullet 
        layerMask = (1<<LayerMask.NameToLayer("IgnoreBullet"));
        layerMask = ~layerMask;
    }

    private void LoadEndAndBeginPlay(object sender, YTriggerEventArgs e) //sender是触发事件的对象，e是事件的参数
    {
        if (e.activated)
        {
            CanDoRecallSkill = true;
            countDownUI.Init();
            
        }
        else
        {
            CanDoRecallSkill = false;
        }
        
    }

    //33310000,炸弹可时间回溯,BombCouldRecallFromPool,10,0 没用到

    //王国之泪出recall 的skill的条件：1.到时间了。2.手动停止。
    //简易点，可以先实现到时间停止，
    public void BeginRecallSkill()
    {
        HPostProcessingFilters.Instance.SetScanEffectPostProcessing(true);
        
        //显示高光亮
        //绑定到YPlayModeController.Instance.curCharacter的手上，比如“zHandTwist_R”节点下,
        //Transform.Find方法只能查找直接的子对象 
        // YRecallLightHandVFX.transform.SetParent
        //     (YPlayModeController.Instance.curCharacter.transform
        //         .Find("Armature/Hips/Spine/Chest/Right shoulder/Right arm/Right elbow/zHandTwist_R"));
        Transform[] allChildren = YPlayModeController.Instance.curCharacter.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == "zHandTwist_R")
            {
                YRecallLightHandVFX.transform.SetParent(child);
                YRecallLightHandVFX.transform.localPosition = Vector3.zero;
                break;
            }
        }
        YRecallLightHandVFX.SetActive(true);
        
        //改变动画
        if(playerStateMachine==null)playerStateMachine = YPlayModeController.Instance.curCharacter.GetComponent<HPlayerStateMachine>();
        playerStateMachine.OnStandingIdle();
        
        Time.timeScale = 0;
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
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
        
        //UI上显示“等待鼠标点击选择”
        countDownUI.showChooseRecalllUI(true);
    }
    // 如果射线与物体相交
    YRecallable preRecallable=null;
    YRecallable recallable = null;
    private GameObject target = null;
    
    Vector2 mousePos = Vector2.zero;
    private void BeginDetectObject()
    {
        // 从摄像头位置创建射线
        //在这里存储鼠标在屏幕上的位置
        mousePos = Mouse.current.position.ReadValue();
        Ray ray = playerCamera.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;

        // 改为鼠标悬停的地方检测是否有合适的物体
        // if (Physics.Raycast(ray, out hitInfo))
        if (Physics.Raycast(ray, out hitInfo, 200f, layerMask))
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
        if (YPlayModeController.Instance.LockEveryInputKey) return;
        if (!CanDoRecallSkill) return;
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
    
    //Skill的Recall()这个代码会去在Recall开始的时候订阅recallable.是否GoBeRecycled()
    void Recall()
    {
        Time.timeScale = 1;
        HPostProcessingFilters.Instance.SetScanEffectPostProcessing(false);
        
        SetPostProcessing(true,mousePos);
        
        //动画
        playerStateMachine.OnSpellRecall(true);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            playerStateMachine.OnStandingIdleBack();
            YTriggerEvents.RaiseOnMouseLeftShoot(true);
        });
        
        YRecallLightHandVFX.SetActive(false);
        
        Debug.Log(">>_<<Begin Recall!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //搞个计时器，
        countDownUI.BeginCountDown();
        CountDownCoroutine = StartCoroutine(CountDown(duration));
        
        duringRecall = true;
        recallable.Recalling(duration);
        recallable.beRecycledAction += OnRecallableGoBeRecycled;

        
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
    }
    private void OnRecallableGoBeRecycled()
    {
        // Handle the event (e.g., stop the recall process)
        EndRecall();
    }
    private ScriptableRendererFeature feature = null;
    private void SetPostProcessing(bool isOn,Vector2 screenPos )
    {
        if (isOn)
        {
            if (this.feature == null)
            {
                string name = "FullScreenDaozhuanqiankun";
                feature =
                    HPostProcessingFilters.Instance.GetRenderFeature(name);
            }
            
            //将mousePos转换为屏幕坐标 且归一化为0-1
            Vector2 pos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
            
            //设置这个feature里面的参数
            HPostProcessingFilters.Instance.SetPassMaterialParameters
                (feature, "_FlowValue",3f, 0, 1.5f,
                    "_StartPos",pos);
        }
        else
        {
            if (this.feature != null)
            {
                feature.SetActive(false);
            }
        }
    }

    private Coroutine CountDownCoroutine;
    public IEnumerator CountDown(float duration)
    {
        yield return new WaitForSeconds(duration);
        EndRecall();
    }
    void EndRecall()
    {
        YRecallLightHandVFX.SetActive(false);
        HPostProcessingFilters.Instance.SetScanEffectPostProcessing(false);
        
        if(CountDownCoroutine!=null)StopCoroutine(CountDownCoroutine);
        
        //如果进入了选择物体界面再退出 已经选中了
        Time.timeScale = 1;
        Debug.Log("()()()()END Recall!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        countDownUI.EndCountDown();
        
        //改变动画
        playerStateMachine.OnStandingIdleBack();
        playerStateMachine.OnSpellRecall(false);
        
        //将所有复原  包括shader，然后进行到一半的recall先停下
        // recallable.EndRecall();
        
        if (recallable != null)
        {
            recallable.beRecycledAction -= OnRecallableGoBeRecycled;
        }
        
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
        
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        
        if(feature!=null)HPostProcessingFilters.Instance.SetPassMaterialParameters
            (feature, false);
        
        isRecalling = false;
        
        //UI上显示“等待鼠标点击选择”的UI消失
        countDownUI.showChooseRecalllUI(false);
        
    }

    private void OnDestroy()
    {
        YTriggerEvents.OnLoadEndAndBeginPlay -= LoadEndAndBeginPlay;
        SetPostProcessing(false,Vector2.zero);
    }
}
