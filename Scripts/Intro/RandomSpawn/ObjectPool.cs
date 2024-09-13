using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject[] prefabs;
    public int poolSize;
    private Queue<GameObject> pool;

    void Start()
    {
        pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = InstantiateRandomPrefab();
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = InstantiateRandomPrefab();
        newObj.SetActive(false);
        newObj.transform.SetParent(transform);
        pool.Enqueue(newObj);
        return newObj;
    }

    private GameObject InstantiateRandomPrefab()
    {
        int randomIndex = Random.Range(0, prefabs.Length);
        return Instantiate(prefabs[randomIndex]);
    }
}
