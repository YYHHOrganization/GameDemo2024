using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AI
{
    public class YEnemyShootCircle : YBTEnemyAction
    {
        public GameObject bulletPrefab;
        public string bulletPrefabLink= "BulletBig02Enemy";
        int ShootInterval = 1;
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
            int j = 0;
            var sequence = DOTween.Sequence();//创建一个序列 360/15 = 24
            for (int i = 0; i < 24; i++)
            {
                sequence.AppendCallback(() =>
                {
                    j++;
                    Vector3 shootDirection = new Vector3(Mathf.Sin(j * 15 * Mathf.Deg2Rad), 0.4f, Mathf.Cos(j* 15 * Mathf.Deg2Rad));
                    GameObject bullet = Object.Instantiate(bulletPrefab, transform.position + shootDirection * 2 , Quaternion.Euler(0, j*15, 0));
                    // SetBulletBaseAttribute(bullet.GetComponent<HEnemyBulletMoveBase>());
                });
                sequence.AppendInterval(0.1f);
            }
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