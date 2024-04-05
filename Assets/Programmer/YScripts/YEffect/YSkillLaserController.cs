using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSkillLaserController : YLaserBase
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        LaserHitVfx.Stop();
        enterLaserDetect = true;
    }

    private void OnDisable()
    {
        enterLaserDetect = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (enterLaserDetect) 
        {
            //Debug 显示一条射线
            Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.red);
            //向z方向发射射线
            // Ray ray = new Ray(transform.position, transform.forward);
            //
            // if (Physics.Raycast(ray, out hit, maxDistance))
            //如果撞到东西了
            
            //只和"LayerBroken"做碰撞检测  1 << LayerMask.NameToLayer("LayerTri")表示LayerTri的层级
            //比如LayerTri的层级是8，那么1 << 8 = 256  
            //https://blog.csdn.net/boyZhenGui/article/details/121558769
            // if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, 
                    1 << LayerMask.NameToLayer("LayerBroken")))
            {
                Debug.Log("hitsthhhh" + hit.transform.name);
                GameObject hitObject = hit.transform.gameObject;
                
                string Tag = hitObject.tag;
                //HandleHitProj
                if (Tag == "CouldBroken"||Tag == "Shield") //test // "Puppet"只是测试 其实是怪这种
                {
                    // 如果射线打中了**
                    Debug.Log("distance11: " + distance);
                    if(Tag == "CouldBroken")
                    {
                        //YFractureExplosionObject fractureExplosionObject = hitObject.GetComponent<YFractureExplosionObject>();
                        YFractureExplosionObject fractureExplosionObject 
                            = hitObject.GetComponentInParent<YFractureExplosionObject>();
                        fractureExplosionObject.TriggerExplosion(hit.point);
                    }
                    else if(Tag == "Shield")
                    {
                        Debug.Log("Shhhhhhhhhhh");
                        //hitObject.GetComponent<YHandleHitPuppet>().HandleHitPuppet();
                        hitObject.GetComponentInParent<YPatrolAI>().die();
                    }
                    
                    distance = Vector3.Distance(transform.position, hit.point);
                    LaserVfx.SetFloat("laserLength", distance / dis2laserLength);

                    //从“没打中”物体进入到“打中”物体
                    if (isBeShotActive == false)
                    {
                        LaserHitVfx.Play();
                        Debug.Log("LaserHitVfx.Play();");
                        isBeShotActive = true;
                        isBeShotDeactive = false;
                    }

                    LaserHitVfx.transform.position = hit.point;

                    
                }
                
                //撞到了别的东西  当作没撞到 当作没看到 -如果认为没撞到东西 
                else
                {
                    //从“打中”物体进入到“没打中”物体
                    if (isBeShotDeactive == false)
                    {
                        LaserHitVfx.Stop();
                        Debug.Log("LaserHitVfx.Stop();");
                        isBeShotDeactive = true;
                        isBeShotActive = false;
                        // 如果射线没有打中任何物体
                        LaserVfx.SetFloat("laserLength", maxDistance / dis2laserLength);
                    }
                }
                

            }
            //如果认为没撞到东西
            else
            {
                //从“打中”物体进入到“没打中”物体
                if (isBeShotDeactive == false)
                {
                    LaserHitVfx.Stop();
                    Debug.Log("LaserHitVfx.Stop();");
                    isBeShotDeactive = true;
                    isBeShotActive = false;
                    // 如果射线没有打中任何物体
                    LaserVfx.SetFloat("laserLength", maxDistance / dis2laserLength);
                }
            }
        }
    }
    
    
}
