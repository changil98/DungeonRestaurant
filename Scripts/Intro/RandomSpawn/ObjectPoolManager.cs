using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public ObjectPool objectPool;
    public Vector2 spawnPos1 = new Vector2(9.5f, 0);
    public Vector2 spawnPos2 = new Vector2(-9.5f, 0);
    public Vector2 targetPos = new Vector2(0, -1.8f);
    private float spawnInterval = 3f;

    void Start()
    {
        StartCoroutine(SpawnCharacters());
    }

    IEnumerator SpawnCharacters()
    {
        while (true)
        {
            Vector2 spawnPos = Random.Range(0, 2) == 0 ? spawnPos1 : spawnPos2;
            GameObject character = objectPool.GetPooledObject();
            if (character != null)
            {
                character.SetActive(true);
                character.GetComponent<SpawnCharacter>().Initialize(spawnPos, targetPos);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
