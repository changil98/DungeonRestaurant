using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{

    private Transform currentEffectsObject;
    private Transform currentEffectsParent;

    private List<ParticleSystem> effects;
    private Dictionary<string, AnimatedEffectPool> effectPools;
    private Dictionary<string, ParticleEffectPool> particleEffectPools;

    protected override void Awake()
    {
        base.Awake();

        effects = new List<ParticleSystem>();
        effectPools = new Dictionary<string, AnimatedEffectPool>();
        particleEffectPools = new Dictionary<string, ParticleEffectPool>();
    }

    //스프라이트 애니메이션 이펙트
    public void PlayAnimatedSpriteEffect(GameObject effectPrefab, Vector3 position, bool flipX = false)
    {
        if (effectPrefab == null) return;

        string prefabName = effectPrefab.name;
        if (!effectPools.ContainsKey(prefabName))
        {
            effectPools[prefabName] = new AnimatedEffectPool(effectPrefab, transform);
        }

        effectPools[prefabName].Play(position, flipX);
    }


    public GameObject PlayFollowingAnimatedSpriteEffect(GameObject effectPrefab, Transform targetToFollow, Vector3 offset, bool flipX = false)
    {
        if (effectPrefab == null) return null;

        string prefabName = effectPrefab.name;
        if (!effectPools.ContainsKey(prefabName))
        {
            effectPools[prefabName] = new AnimatedEffectPool(effectPrefab, transform);
        }

        GameObject effectObject = effectPools[prefabName].PlayAndFollow(targetToFollow, offset, flipX);

        // 이펙트의 초기 위치를 설정합니다.
        effectObject.transform.position = targetToFollow.position + offset;

        return effectObject;
    }

    public void PlayOneShot(ParticleSystem particleSystem, Vector3 position, float duration)
    {
        if (particleSystem == null) return;
        string prefabName = particleSystem.name;
        if (!particleEffectPools.ContainsKey(prefabName))
        {
            particleEffectPools[prefabName] = new ParticleEffectPool(particleSystem, transform);
        }
        particleEffectPools[prefabName].Play(position, duration);
    }

    public GameObject PlayFollowingParticleEffect(ParticleSystem particleSystemPrefab, Transform targetToFollow, Vector3 offset, float duration)
    {
        if (particleSystemPrefab == null) return null;
        string prefabName = particleSystemPrefab.name;
        if (!particleEffectPools.ContainsKey(prefabName))
        {
            particleEffectPools[prefabName] = new ParticleEffectPool(particleSystemPrefab, transform);
        }
        return particleEffectPools[prefabName].PlayAndFollow(targetToFollow, offset, duration);
    }

    public void PlaySpriteOneShot(SpriteRenderer spriteEffect, Vector3 position, bool flipX)
    {
        var obj = Instantiate(spriteEffect, position, Quaternion.identity);
        obj.flipX = flipX;
        obj.gameObject.AddComponent<Disposable>().lifetime = 2f;
    }

    private class EffectPool
    {
        private const int PoolSize = 5;

        private List<ParticleSystem> effectPool;
        private int currentEffectIndex;

        public EffectPool(ParticleSystem particleSystem)
        {
            var pMain = particleSystem.main;
            pMain.playOnAwake = false;

            effectPool = new List<ParticleSystem>();
            for (int i = 0; i < PoolSize; i++)
            {
                effectPool.Add(Instantiate(particleSystem, Instance.transform));
            }
        }

        public void Play(Vector3 position)
        {
            var effect = effectPool[currentEffectIndex];
            effect.transform.position = position;
            effect.Play();

            currentEffectIndex = (currentEffectIndex + 1) % effectPool.Count;
        }

        public void PlayWithColor(Vector3 position, Color color)
        {
            var effect = effectPool[currentEffectIndex];

            // Temporarily override start color
            var main = effect.main;
            var prevColor = main.startColor;
            main.startColor = color;

            Play(position);

            Instance.StartCoroutine(ResetEffectColor(main, prevColor, main.duration));
        }

        private IEnumerator ResetEffectColor(ParticleSystem.MainModule system, ParticleSystem.MinMaxGradient color,
            float delay)
        {
            yield return new WaitForSeconds(delay);
            system.startColor = color;
        }
    }

}

