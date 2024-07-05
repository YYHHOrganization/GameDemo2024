using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HRogueEnemyMoveMirrorWithPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform player;
    private Vector3 startPos;
    private bool isMoving = false;
    private HPlayerStateMachine playerStateMachine;
    private Animator animator;
    private Animator playerAnimator;
    private YRouge_RoomBase currentRoomScript;
    
    
    private bool startDetectLocation = false;
    void Start()
    {
        player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        playerStateMachine = player.GetComponent<HPlayerStateMachine>();
        playerAnimator = player.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        DOVirtual.DelayedCall(2f, ()=>
        {
            startDetectLocation = true;
        });
        currentRoomScript = YRogue_RoomAndItemManager.Instance.currentRoom.gameObject.GetComponent<YRouge_RoomBase>();
        roomSpaceKeep RoomSpaceKeep = currentRoomScript.RoomSpaceKeep;
        //startPos 生成在房间中点
        startPos = new Vector3
        (RoomSpaceKeep.bottomLeft.x + (RoomSpaceKeep.width) / 2,
            0.02f,
            RoomSpaceKeep.bottomLeft.y + (RoomSpaceKeep.length) / 2) + YRogueDungeonManager.Instance.RogueDungeonOriginPos;
    }

    // Update is called once per frame
    void Update()
    {
        MoveMirrorWithPlayerAndFace();
    }

    private void MoveMirrorWithPlayerAndFace()
    {
        if (player == null) return;
        Vector3 playerPos = player.position;
        //与玩家以startPos和playerPos为中心的对称点
        Vector3 mirrorPos = new Vector3(2 * startPos.x - playerPos.x, playerPos.y, 2 * startPos.z - playerPos.z);
        UpdateAnimatorInfo();
        UpdateEnemyLocation(mirrorPos);
        transform.LookAt(player);
    }

    private void UpdateEnemyLocation(Vector3 mirrorPos)
    { 
        transform.position = mirrorPos;
    }

    private void UpdateAnimatorInfo()
    {
        //保持animator的状态和playerAnimator的状态一致
        if (playerAnimator.GetBool("isWalking"))
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", false);
        }
        else if (playerAnimator.GetBool("isRunning"))
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
            animator.SetBool("isJumping", false);
        }
        else if (playerAnimator.GetBool("isJumping"))
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", true);
        }
        else 
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }

}
