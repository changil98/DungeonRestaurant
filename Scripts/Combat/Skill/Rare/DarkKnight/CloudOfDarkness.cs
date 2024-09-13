using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CloudOfDarkness", menuName = "Skills/DarkKnight/CloudOfDarkness")]
public class CloudOfDarkness : BaseSkill
{
    [SerializeField] private float healthCostPercent = 0.2f;
    [SerializeField] private float shieldGainPercent = 0.1f;
    [SerializeField] private float damageMultiplier = 0.1f;
    [SerializeField] private float healingRatio = 0.5f;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float skillRadius = 5f;
    [SerializeField] private GameObject auraVFXPrefab;
    [SerializeField] private GameObject tickDamageVFX;

    private readonly Color damageColor = new Color(25f / 255f, 0f, 80f / 255f, 1f);
    private readonly Color normalColor = Color.white;
    private const float colorChangeDuration = 0.5f;

    private const string AuraStartState = "AuraStart";
    private const string AuraLoopState = "AuraLoop";
    private const string AuraEndState = "AuraEnd";

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is DarkKnight darkKnight)
        {
            Vector3 offset = new Vector3(0,-1,0);
            EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(skillEffectObject,darkKnight.transform,offset);
            darkKnight.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_CloudOfDarkness");
            darkKnight.StartCoroutine(PerformSkill(darkKnight));
        }
    }

    private IEnumerator PerformSkill(DarkKnight darkKnight)
    {
        float healthCost = darkKnight.CurrentHP * healthCostPercent;
        float initialShieldGain = darkKnight.MaxHP * shieldGainPercent;
        darkKnight.TakeDamage(healthCost,DamageType.Physical,AttackType.Blunt,false);
        float skillShieldAmount = initialShieldGain;
        darkKnight.AddShield(skillShieldAmount);
        darkKnight.isShieldOn = true;
        // Aura VFX 생성 및 시작
        GameObject auraInstance = CreateAuraVFX(darkKnight);
        Animator auraAnimator = auraInstance?.GetComponent<Animator>();
        if (auraAnimator != null)
        {
            auraAnimator.Play(AuraStartState);
            yield return new WaitForSeconds(GetAnimationLength(auraAnimator, AuraStartState));
            auraAnimator.Play(AuraLoopState);
        }

        while (darkKnight.ShieldAmount > 0)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(darkKnight.transform.position, skillRadius, LayerMask.GetMask("Character"));
            float totalDamageDealt = 0f;

            foreach (Collider2D hitCollider in hitColliders)
            {
                BaseCombatAI enemy = hitCollider.GetComponent<BaseCombatAI>();
                if (enemy != null && enemy.team != darkKnight.team)
                {
                    float damage = darkKnight.AttackDamage * damageMultiplier;
                    enemy.TakeDamage(damage, DamageType.Magical, AttackType.Blunt, false);
                    totalDamageDealt += damage;

                    ApplyColorChangeEffect(enemy);
                    PlayTickDamageVFX(enemy);
                }
            }

            float healing = totalDamageDealt * healingRatio;
            darkKnight.Heal(healing);

            yield return new WaitForSeconds(damageInterval);
        }
        darkKnight.isShieldOn = false;
        // Aura VFX 종료
        if (auraAnimator != null)
        {
            auraAnimator.Play(AuraEndState);
            yield return new WaitForSeconds(GetAnimationLength(auraAnimator, AuraEndState));
            Object.Destroy(auraInstance);
        }

        darkKnight.EnableManaGain();
    }

    private GameObject CreateAuraVFX(DarkKnight darkKnight)
    {
        if (auraVFXPrefab != null)
        {
            GameObject auraInstance = Object.Instantiate(auraVFXPrefab, darkKnight.transform.position, Quaternion.identity);
            auraInstance.transform.SetParent(darkKnight.transform);
            auraInstance.transform.localPosition = Vector3.zero;

            auraInstance.transform.localScale = new Vector3(3, 3, 1);

            return auraInstance;
        }
        return null;
    }

    private float GetAnimationLength(Animator animator, string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(stateName))
        {
            return stateInfo.length;
        }
        return 0f;
    }

    private void ApplyColorChangeEffect(BaseCombatAI enemy)
    {
        SpriteRenderer[] spriteRenderers = enemy.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (!renderer.CompareTag("IgnoreColorChange"))
            {
                enemy.StartCoroutine(ChangeColor(renderer));
            }
            
        }
    }
    private IEnumerator ChangeColor(SpriteRenderer renderer)
    {
        renderer.color = damageColor;

        float elapsedTime = 0f;
        while (elapsedTime < colorChangeDuration)
        {

                elapsedTime += Time.deltaTime;
                float t = elapsedTime / colorChangeDuration;
                renderer.color = Color.Lerp(damageColor, normalColor, t);
            
            yield return null;
        }

        renderer.color = normalColor;
    }

    private void PlayTickDamageVFX(BaseCombatAI enemy)
    {
        if (tickDamageVFX != null)
        {
            Vector3 effectPos = enemy.transform.position;
            Vector3 offset = new Vector3(0, 2f, 0);
            EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(
                tickDamageVFX, enemy.transform, offset, enemy.IsFacingRight);
        }
    }
}