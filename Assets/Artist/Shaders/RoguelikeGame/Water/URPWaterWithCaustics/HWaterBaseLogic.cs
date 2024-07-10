using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    }

    private bool isInWater = false;
    private bool isFloatOnWater = false;
    private bool isInDivingCD = false;
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
            airWall.SetActive(true);
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
