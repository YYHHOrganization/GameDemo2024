using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;

public class HNiuzhuanwanxiang : MonoBehaviour
{
    public RenderTexture renderTexture;

    private Camera playerCamera;

    private Camera copyCamera;

    public Transform sphere;

    private Animator animator;
    public AnimationClip newClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartSkill()
    {
        animator = GetComponentInChildren<HCharacterInteracionInShow>().gameObject.GetComponent<Animator>();
        if (animator)
        {
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animatorOverrideController["SingleAnimation"] = newClip;
            animator.runtimeAnimatorController = animatorOverrideController;
        }
        animator.SetTrigger("isShowAnimation");
        
        //sphere.GetComponent<Animator>().enabled = false;
        playerCamera = YPlayModeController.Instance.playerCamera().GetComponent<Camera>();
        copyCamera = Instantiate(playerCamera);
        //copyCamera.gameObject.SetActive(false);
        copyCamera.GetComponent<CinemachineBrain>().enabled = false;
        copyCamera.GetComponent<AudioListener>().enabled = false;
        //关闭相机的后处理
        copyCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;
        UniversalAdditionalCameraData cameraData = copyCamera.GetComponent<UniversalAdditionalCameraData>();
        copyCamera.targetTexture = renderTexture;
        //copyCamera.gameObject.SetActive(true);
        //sphere.GetComponent<Animator>().enabled = true;
        DOVirtual.DelayedCall(2f, () => {
            if (cameraData)
            {
                Destroy(cameraData);
            }
            Destroy(copyCamera.gameObject);
            Destroy(this.gameObject);
        }).SetUpdate(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
