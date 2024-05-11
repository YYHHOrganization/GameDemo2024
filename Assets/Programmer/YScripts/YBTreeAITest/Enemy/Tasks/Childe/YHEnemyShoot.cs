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
    public class YHEnemyShoot  : YBTEnemyAction
    {
        public GameObject bulletPrefab;
        public string bulletPrefabLink= "BulletBump01Enemy";

        public string bulletFireType = "ShootBulletMuzzleCircleBumping";
        public int bossBulletDamage = 1;

        public float bossBulletRange = 10f;

        public float bossBulletSpeed = 8f;
        //start
        public override void OnStart()
        {
            base.OnStart();
            
            // animator.SetTrigger("isSkiil1");
            //测试
            bulletPrefab = Addressables.LoadAssetAsync<GameObject>(bulletPrefabLink).WaitForCompletion();
            FireSpecialBulletType();
        }
        // protected override void OnUpdateEnemy()
        // {
        //     ShootBulletForward(true, true);
        //     
        //     //test
        //     transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
        // }
        //

        private void FireSpecialBulletType()
        {
            GameObject muzzleObj = new GameObject("muzzle");
            muzzleObj.transform.position = transform.position;
            muzzleObj.transform.rotation = transform.rotation;
            muzzleObj.transform.parent = transform;
            HBulletMuzzleUtility muzzleUtility = muzzleObj.AddComponent<HBulletMuzzleUtility>();
            muzzleUtility.SetSelfDefInitializeAttribute(bulletPrefab, bulletFireType, true, bossBulletDamage, bossBulletRange, bossBulletSpeed, player.transform);
            muzzleUtility.Shoot();
            Object.Destroy(muzzleObj, 20f);
        }
       
        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}