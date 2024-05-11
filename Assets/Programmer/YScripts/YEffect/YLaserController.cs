using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

// public abstract class YLaserController : MonoBehaviour
public class YLaserController : YLaserBase
{
    
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterFunc(other);
    }

    //protected abstract void TriggerEnterFunc(Collider other);
    protected void TriggerEnterFunc(Collider other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Puppet"))
        {
            enterLaserDetect = true;
        }
    }

    private bool isInHitCD = false;
    void Update()
    {
        if (enterLaserDetect) 
        {
            //向z方向发射射线
            // Ray ray = new Ray(transform.position, transform.forward);
            //
            // if (Physics.Raycast(ray, out hit, maxDistance))
            //如果撞到东西了
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
            {
                GameObject hitObject = hit.transform.gameObject;
                
                //HandleHitProj
                if (hitObject.CompareTag("Shield")) //test
                {
                    // 如果射线打中了防护罩
                    //hitObject.transform.localScale = new Vector3(0, 0, 0); // 缩短到相应位置
                    hitObject.GetComponent<HJustTestRipple>().SetRipple(hit.point); // 通知护盾显示撞击
                    // 计算两点之间的距离

                    // Debug.Log("distance: " + distance);
                }
                else if (hitObject.CompareTag("Player"))
                {
                    // 如果射线打中了角色
                    //hitObject.GetComponent<PlayerScript>().Die(); // 触发角色死亡
                    Debug.Log("hit player!!");
                    if (YPlayModeController.Instance.isRogue)
                    {
                        if (isInHitCD) return;
                        HRoguePlayerAttributeAndItemManager.Instance.ChangeHealth(-1);
                        isInHitCD = true;
                        DOVirtual.DelayedCall(1f, () => { isInHitCD = false; });
                    }
                    else
                    {
                        YPlayModeController.Instance.PlayerDie();
                    }
                }
                else if (hitObject.CompareTag("Puppet"))
                {
                    // 如果射线打中了角色
                    Debug.Log("hit puppet!!");
                    YScreenPlayController.Instance.PuppetDie();
                }

                distance = Vector3.Distance(transform.position, hit.point);
                LaserVfx.SetFloat("laserLength", distance / dis2laserLength);

                //从“没打中”物体进入到“打中”物体
                if (isBeShotActive == false)
                {
                    LaserHitVfx.Play();
                    //Debug.Log("LaserHitVfx.Play();");
                    isBeShotActive = true;
                    isBeShotDeactive = false;
                }

                LaserHitVfx.transform.position = hit.point;

                //Debug.Log("hitsthhhh" + hit.transform.name);

            }
            //如果没撞到东西
            else
            {
                //从“打中”物体进入到“没打中”物体
                if (isBeShotDeactive == false)
                {
                    LaserHitVfx.Stop();
                    //Debug.Log("LaserHitVfx.Stop();");
                    isBeShotDeactive = true;
                    isBeShotActive = false;
                    // 如果射线没有打中任何物体
                    LaserVfx.SetFloat("laserLength", maxDistance / dis2laserLength);
                }
            }
        }
    }
    
    
    //protected abstract void HandleHitProj(GameObject hitObject);
    //判断是否打中物体（是否打中特定或者打中所有）    {打中逻辑；从“没打中”物体进入到“打中”物体逻辑（开启hiteff）}
    //判断是否没打中物体  {从“打中”物体进入到“没打中”物体}
}