using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BattoJutsu", menuName = "Skills/SwordMaster/BattoJutsu")]
public class BattoJutsu : BaseSkill
{
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private float stunDuration = 1f;
    [SerializeField] private GameObject VFX1;
    [SerializeField] private GameObject VFX2;
    [SerializeField] private float slowMotionFactor = 0.6f;
    [SerializeField] private float darknessDuration = 1f;
    [SerializeField] private float skillDuration = 2f;
    private float skillAnimationLength;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is SwordMaster swordMaster)
        {
            swordMaster.StartCoroutine(PerformBattoJutsu(swordMaster));
        }
    }

    private IEnumerator PerformBattoJutsu(SwordMaster swordMaster)
    {
        if (swordMaster.gameObject.activeSelf && swordMaster.cachedNearestTarget != null)
        {
            Animator animator = swordMaster.GetComponentInChildren<Animator>();
            skillAnimationLength = AnimationData.GetAnimationLength(animator, AnimationData.Skill_NormalHash);


            swordMaster.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_Battojutsu");
            yield return new WaitForSeconds(skillAnimationLength * 0.58f);

            List<BaseCombatAI> enemies = FindEnemies(swordMaster);

            List<GameObject> vfx1Instances = new List<GameObject>();
            foreach (var enemy in enemies)
            {
                GameObject vfx1Instance = PlayUnscaledEffect(VFX1, enemy.transform.position, enemy.IsFacingRight);
                vfx1Instances.Add(vfx1Instance);
            }
            yield return new WaitForSecondsRealtime(GetEffectDuration(VFX1));
            foreach (var instance in vfx1Instances)
            {
                Destroy(instance);
            }

            yield return new WaitForSeconds(skillAnimationLength * 0.42f);

            List<GameObject> vfx2Instances = new List<GameObject>();
            foreach (var enemy in enemies)
            {
                GameObject vfx2Instance = PlayUnscaledEffect(VFX2, enemy.transform.position, enemy.IsFacingRight);
                vfx2Instances.Add(vfx2Instance);
            }
            yield return new WaitForSecondsRealtime(GetEffectDuration(VFX2));
            foreach (var instance in vfx2Instances)
            {
                Destroy(instance);
            }

            if (enemies.Count == 1)
            {
                BaseCombatAI singleEnemy = enemies[0];
                singleEnemy.ApplyStun(stunDuration);
                for (int i = 0; i < 5; i++)
                {
                    ApplyDamage(swordMaster, singleEnemy);
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
            else
            {
                foreach (var enemy in enemies)
                {
                    ApplyDamage(swordMaster, enemy);
                }
            }
        }
    }


    private GameObject PlayUnscaledEffect(GameObject effectPrefab, Vector3 position, bool flipX)
    {
        GameObject effectInstance = Object.Instantiate(effectPrefab, position, Quaternion.identity);
        Animator animator = effectInstance.GetComponent<Animator>();
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        SpriteRenderer spriteRenderer = effectInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipX;
        }
        return effectInstance;
    }

    private void ApplyDamage(SwordMaster swordMaster, BaseCombatAI enemy)
    {
        float damage = swordMaster.AttackDamage * damageMultiplier;
        if(enemy.gameObject.activeSelf)
        {     
            SoundManager.Instance.PlaySound("SFX_SkillSlash");
            enemy.TakeDamage(damage, DamageType.Physical, AttackType.Slash, false);
        }
    }

    private List<BaseCombatAI> FindEnemies(SwordMaster swordMaster)
    {
        List<BaseCombatAI> enemies = new List<BaseCombatAI>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(swordMaster.transform.position, 50f, LayerMask.GetMask("Character"));
        foreach (var collider in colliders)
        {
            BaseCombatAI enemy = collider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy.team != swordMaster.team)
            {
                enemies.Add(enemy);
            }
        }
        return enemies;
    }

    private float GetEffectDuration(GameObject effectPrefab)
    {
        Animator animator = effectPrefab.GetComponent<Animator>();
        if (animator != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                return clips[0].length;
            }
        }
        return 1f;
    }
}
