using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class YLaserBase : MonoBehaviour
{
    //发射激光 当发射激光之后，会去检测是否碰撞到了物体，如果碰撞到了物体，就会调用物体的方法
    // Start is called before the first frame update
    protected RaycastHit hit;

    protected float distance;
    public float maxDistance = 13f;
    //VFX
    public VisualEffect LaserVfx;
    protected float dis2laserLength=6;
    
    public bool enterLaserDetect = false;

    public VisualEffect LaserHitVfx;
    
    //记录从没打到物体进入有打到物体的状态
    protected bool isBeShotActive = false;
    //记录从打到物体进入没打到物体的状态
    protected bool isBeShotDeactive = false;
    
    protected void Start()
    {
        // LaserVfx = gameObject.GetComponent<VisualEffect>();
        Debug.Log("LaserHitVfx.Stop();");
        LaserHitVfx.Stop();
        dis2laserLength = 6*gameObject.transform.localScale.z;
    }


   
}
