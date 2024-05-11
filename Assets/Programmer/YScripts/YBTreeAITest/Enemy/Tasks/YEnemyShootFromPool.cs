using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;

namespace Core.AI
{
    public class YEnemyShootFromPool : YBTEnemyAction
    {
        public GameObject bulletPrefab;
        public string bulletPrefabLink= "BulletBig02Enemy";

        private string nameid = "33300004";//"33300000";
        // int ShootInterval = 1;
        //bulletNum是子弹的数量，totalAngle是子弹的总角度，shootIntervalTime是子弹发射的间隔时间
        [Header("以下总的时间=子弹数量*间隔时间，要斟酌，不然上一次的子弹还没发射完，下一次的子弹就开始发射了，会有bug")]
        public int bulletNum = 24;
        //总共的角度
        public int totalAngle = 360; 
        public float shootIntervalTime = 0.1f;

        private int bulletAngle;
        //start
        public override void OnStart()
        {
            base.OnStart();
            
            // animator.SetTrigger("isSkiil1");
            //测试
            // bulletPrefab = Addressables.LoadAssetAsync<GameObject>(bulletPrefabLink).WaitForCompletion();
        }
        protected override void OnUpdateEnemy()
        {
            
            ShootBulletForward(true, true);
            
            //test
            transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
        }
        //初始化武器并攻击玩家
        protected void ShootBulletForward(bool trackPlayer = false, bool isChasing=false)
        {
            Profiler.BeginSample("ShootBulletCircle!!");
            string textStr = $"textStr{Time.time}";
            string textStr1 = $"textStr1{Time.time}";
            string textStr2 = $"textStr2{Time.time}";
            string textStr3 = $"textStr3{Time.time}";
            
            int j = 0;
            bulletAngle = totalAngle / bulletNum;////创建一个序列 例如360/15 = 24
            var sequence = DOTween.Sequence();//创建一个序列 例如360/15 = 24
            for (int i = 0; i < bulletNum; i++)
            {
                sequence.AppendCallback(() =>
                {
                    j++;
                    //Vector3 shootDirection = new Vector3(Mathf.Sin(j * 15 * Mathf.Deg2Rad), 0.4f, Mathf.Cos(j* 15 * Mathf.Deg2Rad));
                    Vector3 shootDirection = new Vector3(
                        Mathf.Sin(j * bulletAngle * Mathf.Deg2Rad), 
                        0.4f, 
                        Mathf.Cos(j* bulletAngle * Mathf.Deg2Rad));
                    //GameObject bullet = Object.Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, j*15, 0));
                    //GameObject bullet = YBulletsPool.bulletsPoolInstance.GetPooledObject();
                    GameObject bullet = YObjectPool._Instance.Spawn(nameid);
                    
                    bullet.transform.position = transform.position + shootDirection * 2;
                    //第二次修改其旋转角度的时候，这里的j*15是有问题的，因为这里的j是从1开始的，所以这里应该是j*bulletAngle
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.transform.rotation = Quaternion.Euler(0, j * bulletAngle, 0);//0, j * 15, 0);
                    bullet.SetActive(true);
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.transform.rotation = Quaternion.Euler(0, j * bulletAngle, 0);//0, j * 15, 0);
                    
                    //test
                    
                    // for (int i = 0; i < 10; i++)
                    // {
                    //     //GameObject bulletn = Object.Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, j*15, 0));
                    //     // GameObject bulletn = YBulletsPool.bulletsPoolInstance.GetPooledObject();
                    //     GameObject bulletn = YObjectPool._Instance.Spawn(nameid);
                    //     bulletn.transform.position = transform.position + shootDirection * 2;
                    //     bulletn.transform.rotation = Quaternion.Euler(0, j * 15, 0);
                    //     bulletn.SetActive(true);
                    // }
                    //
                    //testEnd
                });
                sequence.AppendInterval(shootIntervalTime);//(0.1f);
            }
            
            Profiler.EndSample();
        }
        // public IEnumerator ShootCircleInterval()  //也是环状射击子弹，不过一圈的每一颗子弹是间隔发射的
        // {
        //     while (true)
        //     {
        //         for (int i = 0; i < 360; i += 15)
        //         {
        //             Vector3 shootDirection = new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), 0.4f, Mathf.Cos(i * Mathf.Deg2Rad));
        //             GameObject bullet = Instantiate(chaseBulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, i, 0));
        //             SetBulletBaseAttribute(bullet.GetComponent<HEnemyBulletMoveBase>());
        //             yield return new WaitForSeconds(0.1f);
        //         }
        //         yield return new WaitForSeconds(enemy._RogueEnemyChaseShootInterval());
        //     }
        // }
        public override void OnEnd()
        {
            base.OnEnd();
            StopAllCoroutines();
        }
    }
    
}