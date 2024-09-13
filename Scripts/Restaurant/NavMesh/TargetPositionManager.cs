using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetPositionManager : Singleton<TargetPositionManager>
{
    public List<Vector3> targetPositions = new List<Vector3>();
    public List<bool> checkArrive = new List<bool>();
    public List<Image> foodImage = new List<Image>();

    public GameObject[] character;

    public int poolSize;

    public Queue<GameObject> pool;

    public Coroutine instantiateCharacterCoroutine;

    private new void Awake()
    {
        InitializeTargetPositions();
        InitializePool();
        CheckAndStartInstantiateCharacter();
    }

    void InitializeTargetPositions()
    {
        targetPositions.Add(new Vector3(-5.15f, 0.2f, 0f));
        targetPositions.Add(new Vector3(-4.25f, 0.2f, 0f));
        targetPositions.Add(new Vector3(-3.35f, 0.2f, 0f));
        targetPositions.Add(new Vector3(-2.45f, 0.2f, 0f));
        targetPositions.Add(new Vector3(-4.4f, -1.6f, 0f));
        targetPositions.Add(new Vector3(-2.85f, -1.6f, 0f));
        targetPositions.Add(new Vector3(-0.3f, -1.15f, 0f));
        targetPositions.Add(new Vector3(1.2f, -1.15f, 0f));
        targetPositions.Add(new Vector3(1.9f, 0.2f, 0f));
        targetPositions.Add(new Vector3(3.5f, 0.2f, 0f));
        targetPositions.Add(new Vector3(7f, 0.15f, 0f));
        targetPositions.Add(new Vector3(7f, -1.2f, 0f));
        targetPositions.Add(new Vector3(6f, -0.7f, 0f));

        checkArrive = new List<bool>(new bool[targetPositions.Count]);
    }

    void InitializePool()
    {
        pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            int randomCharacter = Random.Range(0, character.Length);
            GameObject obj = Instantiate(character[randomCharacter]);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    private void CheckAndStartInstantiateCharacter()
    {
        if (instantiateCharacterCoroutine == null && checkArrive.Contains(false))
        {
            instantiateCharacterCoroutine = StartCoroutine(InstantiateCharacter());
        }
    }

    IEnumerator InstantiateCharacter()
    {
        while (true)
        {
            if (checkArrive.Contains(false))
            {
                GameObject newCharacter = GetPooledObject();
                AgentMovement agentMovement = newCharacter.GetComponent<AgentMovement>();
                if (agentMovement != null)
                {
                    agentMovement.Initialize();
                }
                yield return new WaitForSeconds(5);
            }
            else
            {
                yield return null;
            }
        }
    }

    private GameObject GetPooledObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            int randomCharacter = Random.Range(0, character.Length);
            GameObject obj = Instantiate(character[randomCharacter]);
            obj.transform.SetParent(transform);
            return obj;
        }
    }

    public (Vector3, int) GetTargetPositionWithIndex()
    {
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < checkArrive.Count; i++)
        {
            if (!checkArrive[i])
            {
                availableIndexes.Add(i);
            }
        }

        if (availableIndexes.Count == 0)
        {
            return (Vector3.zero, -1);
        }

        int randomIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];
        checkArrive[randomIndex] = true;
        return (targetPositions[randomIndex], randomIndex);
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
        checkArrive[GetTargetIndexFromObject(obj)] = false;
        CheckAndStartInstantiateCharacter();
    }

    private int GetTargetIndexFromObject(GameObject obj)
    {
        AgentMovement agentMovement = obj.GetComponent<AgentMovement>();
        return agentMovement != null ? agentMovement.TargetIndex : -1;
    }
}
