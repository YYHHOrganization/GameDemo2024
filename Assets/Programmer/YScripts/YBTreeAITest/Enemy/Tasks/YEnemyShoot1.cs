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
    public class YEnemyShoot1 : YBTEnemyAction
    {
        public GameObject bulletPrefab;
        public string bulletPrefabLink= "BulletBig02Enemy";
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
            /*
             * 1.根据预制体生成子弹(bulletPrefabLink)
             * 2.子弹向前发射：bullet.GetComponent<bulletBase>().ShootForward();
             * 3.开启子弹射击的协程这种写到子弹里面应该就行了
             */
            if (bulletPrefab)
            {
                GameObject bullet = Object.Instantiate(bulletPrefab, shootOrigin.position, shootOrigin.rotation);
                if (trackPlayer)
                {
                    bullet.gameObject.GetComponent<HEnemyBulletMoveBase>().SetTarget(player.transform, true);
                }
            }
        }
       
        public override void OnEnd()
        {
            base.OnEnd();
            StopAllCoroutines();
        }
    }
}