using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private void OnEnable()
    {
        ObjectManager.PoolManager = this;
    }


    [Serializable]
    public class Pool
    {
        public string tagName;
        public GameObject prefab;
        public int firstSize;
        public int currentSize;
        public Transform usedContainer;
        public Transform readyToUseContainer;
    }

    public List<Pool> pools;
    public Dictionary<string, Stack<GameObject>> poolDictionary;
    public Dictionary<string, Pool> poolDic;

    private Vector3 firstSpawnPos = Vector3.zero;

    private void Awake()
    {
        poolDictionary = new Dictionary<string, Stack<GameObject>>();
        poolDic = new Dictionary<string, Pool>();

        foreach (Pool pool in pools)
        {
            GameObject usedContainer = new GameObject(pool.tagName + " Used Container");
            usedContainer.transform.SetParent(transform);
            GameObject readyToUseContainer = new GameObject(pool.tagName + " Ready To Used Container");
            readyToUseContainer.transform.SetParent(transform);

            pool.usedContainer = usedContainer.transform;
            pool.readyToUseContainer = readyToUseContainer.transform;

            Stack<GameObject> objectPool = new Stack<GameObject>();
            
            poolDictionary.Add(pool.tagName, objectPool);
            poolDic.Add(pool.tagName, pool);

            SpawnForPool(pool, pool.firstSize);
            
        }
    }

    private void SpawnForPool(Pool targetPool, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(targetPool.prefab, firstSpawnPos, Quaternion.identity, targetPool.readyToUseContainer.transform);
            obj.SetActive(false);
            obj.name = targetPool.tagName + " " + (targetPool.currentSize+i).ToString();
            poolDictionary[targetPool.tagName].Push(obj);
        }

        targetPool.currentSize += count;
        
    }

    public void SpawnFromPool(string tag, Vector3 spawnPos)
    {
        SpawnFromPool(tag, spawnPos, Quaternion.identity);
    }

    public void SpawnFromPool(string tag, Vector3 spawnPos, Quaternion rotation)
    {
        if (poolDictionary[tag].Count <= 0)
        {
            SpawnForPool(poolDic[tag], 10);
        }
        
        GameObject obj = poolDictionary[tag].Pop();
        obj.transform.position = spawnPos;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(poolDic[tag].usedContainer);
        obj.SetActive(true);
    }

    public GameObject SpawnFromPoolAsObject(string tag, Vector3 spawnPos, Quaternion rotation)
    {
        GameObject obj = poolDictionary[tag].Pop();
        obj.transform.position = spawnPos;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(poolDic[tag].usedContainer);
        obj.SetActive(true);
        return obj;
    }

    public void BackToThePool(string tag, GameObject targetObject)
    {
        targetObject.transform.SetParent(poolDic[tag].readyToUseContainer);
        targetObject.SetActive(false);
        poolDictionary[tag].Push(targetObject);
    }
}