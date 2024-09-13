using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    public void SetTarget(Transform newTarget, Vector3 newOffset)
    {
        target = newTarget;
        offset = newOffset;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}