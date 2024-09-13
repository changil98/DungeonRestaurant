using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnArea : MonoBehaviour
{
    public Vector2 areaSize;
    public float minDistanceBetweenEnemies = 1f;

    private List<Vector2> occupiedPositions = new List<Vector2>();

    public Vector2 GetRandomPosition()
    {
        Vector2 randomPos;
        int maxAttempts = 100;
        int attempts = 0;

        do
        {
            randomPos = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            attempts++;
        } while (IsPositionOccupied(randomPos));

        occupiedPositions.Add(randomPos);
        return (Vector2)transform.position + randomPos;
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        foreach (Vector2 occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(position, occupiedPos) < minDistanceBetweenEnemies)
            {
                return true;
            }
        }
        return false;
    }

    public void ClearOccupiedPositions()
    {
        occupiedPositions.Clear();
    }

    //TODO :나중에 지우기
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}

