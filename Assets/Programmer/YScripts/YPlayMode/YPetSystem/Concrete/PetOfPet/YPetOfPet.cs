using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YPetOfPet : MonoBehaviour
{
    //宠物的宠物，比如卡芙卡的蜘蛛
    [SerializeField]YPetWeapon petWeapon;
    private void Start()
    {
        
    }

    public void BeginAttackDetect()
    {
        //开始检测攻击
        petWeapon.SetDetectShootOn();
        
    }
    public void StopAttackDetect()
    {
        //停止检测攻击
        petWeapon.SetDetectShootOff();
    }
}
