using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class YLaserController : MonoBehaviour
{
    //发射激光 当发射激光之后，会去检测是否碰撞到了物体，如果碰撞到了物体，就会调用物体的方法
    // Start is called before the first frame update
    private RaycastHit hit;

    private float distance;
    public float maxDistance = 13f;
    //VFX
    public VisualEffect LaserVfx;
    float dis2laserLength=6;

    private void Start()
    {
        // LaserVfx = gameObject.GetComponent<VisualEffect>();
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))//如果鼠标左键按下
        //{
            // Debug.Log("鼠标左键按下");
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            //向z方向发射射线
            Ray ray = new Ray(transform.position, transform.forward);
            
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                GameObject hitObject = hit.transform.gameObject;

                // if (hitObject.CompareTag("Shield"))
                if (hitObject.CompareTag("Shield"))//test
                {
                    // 如果射线打中了防护罩
                    //hitObject.transform.localScale = new Vector3(0, 0, 0); // 缩短到相应位置
                    hitObject.GetComponent<HJustTestRipple>().SetRipple(hit.point); // 通知护盾显示撞击
                    // 计算两点之间的距离
                    distance = Vector3.Distance(transform.position, hit.point);
                    LaserVfx.SetFloat("laserLength", distance/dis2laserLength);
                    // Debug.Log("distance: " + distance);
                }
                else if (hitObject.CompareTag("Player"))
                {
                    // 如果射线打中了角色
                    //hitObject.GetComponent<PlayerScript>().Die(); // 触发角色死亡
                }
                
            }
            else
            {
                // 如果射线没有打中任何物体
                LaserVfx.SetFloat("laserLength", maxDistance/dis2laserLength);
            }
        //}
    }
}