using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectPool
{
    private const int InitialPoolSize = 20;
    private ParticleSystem prefab;
    private List<ParticleSystem> pool;
    private Transform parentTransform;

    public ParticleEffectPool(ParticleSystem particleSystemPrefab, Transform parent)
    {
        prefab = particleSystemPrefab;
        parentTransform = parent;
        pool = new List<ParticleSystem>();
        for (int i = 0; i < InitialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    private ParticleSystem CreateNewInstance()
    {
        ParticleSystem instance = Object.Instantiate(prefab, parentTransform);
        instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        pool.Add(instance);
        return instance;
    }

    public void Play(Vector3 position, float duration)
    {
        ParticleSystem effect = GetFreeInstance();
        effect.transform.position = position;
        ResetAndPlayEffect(effect);
        EffectManager.Instance.StartCoroutine(ReturnToPoolAfterDuration(effect, duration));
    }

    public GameObject PlayAndFollow(Transform targetToFollow, Vector3 offset, float duration)
    {
        ParticleSystem effect = GetFreeInstance();
        effect.transform.position = targetToFollow.position + offset;
        ResetAndPlayEffect(effect);
        GameObject effectObject = effect.gameObject;
        FollowTarget followComponent = effectObject.GetComponent<FollowTarget>();
        if (followComponent == null)
        {
            followComponent = effectObject.AddComponent<FollowTarget>();
        }
        followComponent.SetTarget(targetToFollow, offset);
        EffectManager.Instance.StartCoroutine(ReturnToPoolAfterDuration(effect, duration));
        return effectObject;
    }

    private ParticleSystem GetFreeInstance()
    {
        foreach (ParticleSystem instance in pool)
        {
            if (!instance.isPlaying)
            {
                return instance;
            }
        }
        return CreateNewInstance();
    }

    private void ResetAndPlayEffect(ParticleSystem effect)
    {
        effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        effect.Clear();
        effect.gameObject.SetActive(true);
        effect.Play();
    }

    private IEnumerator ReturnToPoolAfterDuration(ParticleSystem effect, float duration)
    {
        yield return new WaitForSeconds(duration);
        effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        effect.gameObject.SetActive(false);
    }
}

