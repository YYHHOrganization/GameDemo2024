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
    public class YEnemyShootCircle : YBTEnemyAction
    {
        public GameObject bulletPrefab;
        public string bulletPrefabLink= "BulletBig02Enemy";
        // int ShootInterval = 1;
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
            bulletPrefab = Addressables.LoadAssetAsync<GameObject>(bulletPrefabLink).WaitForCompletion();
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
                    GameObject bullet = Object.Instantiate
                    (bulletPrefab,
                        transform.position + shootDirection * 2,
                        Quaternion.Euler(0, j*bulletAngle, 0));//); //(0, j*15, 0));
                    // SetBulletBaseAttribute(bullet.GetComponent<HEnemyBulletMoveBase>());

                    //test
                    // for (int i = 0; i < 10; i++)
                    // {
                    //     GameObject bulletn = Object.Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, j*15, 0));
                    // }

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