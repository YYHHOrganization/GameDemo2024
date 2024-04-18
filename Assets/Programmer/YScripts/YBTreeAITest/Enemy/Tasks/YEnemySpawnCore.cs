using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AI
{
    public class YEnemySpawnCore : YBTEnemyAction
    {
        public GameObject corePrefab;
        // public Transform coreSpawnPoint;
        YEnemyBT coreenemyBT;
        // public GameObject coreCollider;
        public string corePrefabLink= "YEnemyCore";

        public override void OnStart()
        {
            base.OnStart();
            corePrefab = Addressables.InstantiateAsync(corePrefabLink).WaitForCompletion();
            corePrefab.transform.position = transform.position;
            // corePrefab.transform.parent = transform;
            // corePrefab.transform.localPosition = new Vector3(0, 0, 0);
            // corePrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
            coreenemyBT = corePrefab.GetComponent<YEnemyBT>();
            //角色血条冻结或者消失
            
            //核心血条出现
            
        }

        public override TaskStatus OnUpdate()
        {
            if(coreenemyBT.curHealth >0)
            {
                // Debug.Log(coreenemyBT.gameObject.name+" is alive"+coreenemyBT.curHealth);
                return TaskStatus.Running;
            }
            else
            {
                //corePrefab.SetActive(false);
                Object.Destroy(corePrefab);
                return TaskStatus.Success;
            }
        }
    }
}