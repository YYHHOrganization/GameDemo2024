using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YLaserController : MonoBehaviour
{
    //发射激光 当发射激光之后，会去检测是否碰撞到了物体，如果碰撞到了物体，就会调用物体的方法
    // Start is called before the first frame update
    private RaycastHit hit;

    private float distance;
    
    //VFX
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//如果鼠标左键按下
        {
            Debug.Log("鼠标左键按下");
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            //向z方向发射射线
            Ray ray = new Ray(transform.position, transform.forward);
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                GameObject hitObject = hit.transform.gameObject;

                // if (hitObject.CompareTag("Shield"))
                if (hitObject.CompareTag("Puppet"))//test
                {
                    // 如果射线打中了防护罩
                    //hitObject.transform.localScale = new Vector3(0, 0, 0); // 缩短到相应位置
                    //hitObject.GetComponent<ShieldScript>().ShowImpact(); // 通知护盾显示撞击
                    
                    // 计算两点之间的距离
                    distance = Vector3.Distance(transform.position, hit.point);
                    Debug.Log("distance: " + distance);
                }
                else if (hitObject.CompareTag("Player"))
                {
                    // 如果射线打中了角色
                    //hitObject.GetComponent<PlayerScript>().Die(); // 触发角色死亡
                }
            }
        }
    }
}