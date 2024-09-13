using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEffectPool
{
    private const int InitialPoolSize = 20;
    private GameObject prefab;
    private List<GameObject> pool;
    private Transform parentTransform;

    public AnimatedEffectPool(GameObject effectPrefab, Transform parent)
    {
        prefab = effectPrefab;
        parentTransform = parent;
        pool = new List<GameObject>();

        for (int i = 0; i < InitialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    private GameObject CreateNewInstance()
    {
        GameObject instance = Object.Instantiate(prefab, parentTransform);
        instance.SetActive(false);
        pool.Add(instance);
        return instance;
    }

    public void Play(Vector3 position, bool flipX)
    {
        GameObject effectObject = GetFreeInstance();
        effectObject.transform.position = position;

        SpriteRenderer spriteRenderer = effectObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipX;
        }

        Animator animator = effectObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(0, -1, 0f);  // 애니메이션을 처음부터 재생
        }

        effectObject.SetActive(true);

        float animationLength = GetAnimationLength(animator);
        EffectManager.Instance.StartCoroutine(ReturnToPool(effectObject, animationLength));
    }

    public GameObject PlayAndFollow(Transform targetToFollow, Vector3 offset, bool flipX)
    {
        GameObject effectObject = GetFreeInstance();
        effectObject.transform.position = targetToFollow.position + offset;

        SpriteRenderer spriteRenderer = effectObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipX;
        }

        Animator animator = effectObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(0, -1, 0f);
        }

        effectObject.SetActive(true);

        FollowTarget followComponent = effectObject.GetComponent<FollowTarget>();
        if (followComponent == null)
        {
            followComponent = effectObject.AddComponent<FollowTarget>();
        }
        followComponent.SetTarget(targetToFollow, offset);

        float animationLength = GetAnimationLength(animator);
        EffectManager.Instance.StartCoroutine(ReturnToPool(effectObject, animationLength));

        return effectObject;
    }



    private GameObject GetFreeInstance()
    {
        foreach (GameObject instance in pool)
        {
            if (!instance.activeInHierarchy)
            {
                return instance;
            }
        }

        return CreateNewInstance();
    }

    private IEnumerator ReturnToPool(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }

    private float GetAnimationLength(Animator animator)
    {
        if (animator == null) return 1f;  // 기본값 설정

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip.length;
        }

        return 1f;  // 애니메이션 정보를 찾지 못한 경우 기본값 반환
    }
}
