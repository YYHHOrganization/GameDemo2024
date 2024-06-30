using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRecallObjectPool : MonoBehaviour
{
    public List<GameObject> recallableObjectPool;
    private YRecallSkill recallSkill;

    private void Start()
    {
        recallableObjectPool = new List<GameObject>();
        recallSkill= gameObject.AddComponent<YRecallSkill>();
        recallSkill.setPool(this);
        // Subscribe to ObjectSpawned event from YObjectPool
        YObjectPool._Instance.ObjectSpawned += OnObjectSpawnedAndPutIntoList;
    }
    //todo：这里有一个将go加入到recallableObjectPool中的函数
    // Event handler for ObjectSpawned event
    private void OnObjectSpawnedAndPutIntoList(GameObject spawnedObject)
    {
        if (spawnedObject != null && !recallableObjectPool.Contains(spawnedObject))
        {
            recallableObjectPool.Add(spawnedObject);
        }
    }

    private void OnDestroy()
    {
        YObjectPool._Instance.ObjectSpawned -= OnObjectSpawnedAndPutIntoList;
    }
}
