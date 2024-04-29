using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace Core.AI
{
    public class YEnemySpawnFallingBullet  : YBTEnemyAction
    {
        public Collider spawnAreaCollider;
        
        public GameObject bulletPrefab;

        public int spawnCount = 4;
        public float spawnInterval = 0.5f;
        //start
        public override void OnStart()
        {
            base.OnStart();
            
            animator.SetTrigger("isSkiil1");
            //测试
            string bulletPrefabLink = "BulletBig01Enemy";
            bulletPrefab = Addressables.LoadAssetAsync<GameObject>(bulletPrefabLink).WaitForCompletion();
        }
        protected override void OnUpdateEnemy()
        {
            SpawnBulletFalling(false, false);
        }
       
       
        //初始化武器并攻击玩家
        protected void SpawnBulletFalling(bool trackPlayer = false, bool isChasing=false)
        {
            //Instantiate(bulletMuzzle).""
            var sequence = DOTween.Sequence();//创建一个序列
            for (int i = 0; i < spawnCount; i++)
            {
                sequence.AppendCallback(() =>
                {
                    SpawnBullet();
                });
                sequence.AppendInterval(spawnInterval);
            }
            /*
             * 1.根据预制体生成子弹(bulletPrefabLink)
             * 2.子弹向前发射：bullet.GetComponent<bulletBase>().ShootForward();
             * 3.开启子弹射击的协程这种写到子弹里面应该就行了
             */
            
        }
        private void SpawnBullet()
        {
            Vector3 spawnPos = new Vector3(Random.Range(spawnAreaCollider.bounds.min.x, spawnAreaCollider.bounds.max.x),
                spawnAreaCollider.bounds.max.y, Random.Range(spawnAreaCollider.bounds.min.z, spawnAreaCollider.bounds.max.z));
            //向下的方向
            int randomRot = Random.Range(70, 110);
            Quaternion rot=Quaternion.Euler(randomRot,0,0);
            GameObject bullet = Object.Instantiate(bulletPrefab, spawnPos, rot);
            // bullet.gameObject.GetComponent<HEnemyBulletMoveBase>().SetTarget(player.transform, true);
        }
       
        public override void OnEnd()
        {
            base.OnEnd();
            StopAllCoroutines();
        }
    }
}
