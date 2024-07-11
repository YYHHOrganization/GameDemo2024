using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class HWaterBaseLogic : MonoBehaviour
{
    private float locateY = 0;
    public Transform waterFloor;

    private Transform player;
    // Start is called before the first frame update
    public GameObject airWall;
    void Start()
    {
        locateY = waterFloor.position.y;
        ripple = Addressables.LoadAssetAsync<GameObject>("EffectWaterRipple").WaitForCompletion();
    }

    private bool isInWater = false;
    private bool isFloatOnWater = false;
    private bool isInDivingCD = false;
    private GameObject rippleObj;
    private GameObject ripple;
    private enum WaterEffectType
    {
        OnWaterFlow, //在水面上
        InWater, //在水中
        LeaveWater, //离开水面
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isFloatOnWater = true;
            // other.GetComponent<CharacterController>().enabled = false;
            // other.transform.position = new Vector3(other.transform.position.x, locateY, other.transform.position.z);
            // other.GetComponent<CharacterController>().enabled = true;
            other.gameObject.GetComponent<HPlayerStateMachine>().SetOnWaterFloat(true);
            // if (isInWater) return;
            // Debug.Log("Enter Water!");
            // //other.gameObject.GetComponent<HPlayerStateMachine>().SetInWater(true);
            //
            // isInWater = true;
            SetEffect(WaterEffectType.OnWaterFlow, other);
            airWall.SetActive(true);
        }
    }

    private void SetEffect(WaterEffectType type, Collider player)
    {
        switch (type)
        {
            case WaterEffectType.OnWaterFlow:
                if (rippleObj == null)
                {
                    rippleObj = Instantiate(ripple, player.transform);
                    rippleObj.transform.position = new Vector3(rippleObj.transform.position.x, locateY, rippleObj.transform.position.z);
                }
                break;
            case WaterEffectType.InWater:
                if (rippleObj)
                {
                    Destroy(rippleObj);
                }
                break;
            case WaterEffectType.LeaveWater:
                if (rippleObj)
                {
                    Destroy(rippleObj);
                }
                break;
        }
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("other.transform.position.y ++ locateY" + other.transform.position.y + " ++ " + locateY);
            // if (!isInWater)
            // {
            //     Debug.Log("Enter Water!");
            //     other.gameObject.GetComponent<HPlayerStateMachine>().SetInWater(true);
            //     isInWater = true;
            // }
            //刚下水 -1.5 -1
            
            if (other.transform.position.y > locateY - 0.7f)
            {
                other.gameObject.GetComponent<HPlayerStateMachine>().SetOnWaterFloat(true);
                SetEffect(WaterEffectType.OnWaterFlow, other);
            }
            else if(other.transform.position.y < locateY - 2.0f) //潜入水里了
            {
                SetEffect(WaterEffectType.InWater, other);
                //添加进入水里的特效
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Exit Water!");
            // if (other.transform.position.y >= locateY + 5)
            // {
            //     other.gameObject.GetComponent<HPlayerStateMachine>().SetInWater(false);
            //     isInWater = false;
            // }
            other.gameObject.GetComponent<HPlayerStateMachine>().SetOnWaterFloat(false);
            SetEffect(WaterEffectType.LeaveWater, other);
            airWall.SetActive(false);
        }
    }

    private void Update()
    {
        // if (!player)
        // {
        //     GameObject go = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer();
        //     if (go != null)
        //     {
        //         player = go.transform;
        //     }
        // }
    }
}
