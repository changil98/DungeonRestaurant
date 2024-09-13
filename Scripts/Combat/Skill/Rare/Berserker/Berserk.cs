using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Berserk", menuName = "Skills/Berserker/Berserk")]
public class Berserk : BaseSkill
{
    public float baseDuration = 3f;
    public float maxSizeIncrease = 1.5f;
    public float attackSpeedIncrease = 0.5f;
    public float attackDamageIncrease = 50f;
    public float healingRatio = 0.5f;
    public float durationIncreaseOnKill = 3f;
    public GameObject killVFX;
    public GameObject healVFX;
    public GameObject auraVFX;

    private float remainingDuration;
    private Coroutine berserkCoroutine;


    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Berserker berserker)
        {
            Vector3 effectPos = character.transform.position;
            Vector3 offset = effectPos - character.transform.position;
            offset.y += 2.5f;
            EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(skillEffectObject,character.transform,offset,character.IsFacingRight);
            berserker.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_Berserk");
            ActivateBerserkState(berserker);
        }
    }

    public void ActivateBerserkState(Berserker berserker)
    {
        if (berserkCoroutine != null)
        {
            berserker.StopCoroutine(berserkCoroutine);
        }
        remainingDuration = baseDuration;
        berserkCoroutine = berserker.StartCoroutine(ApplyBerserkEffect(berserker));
    }

    private IEnumerator ApplyBerserkEffect(Berserker berserker)
    {
        Vector3 originalScale = berserker.transform.localScale;
        float originalAttackSpeed = berserker.AttackSpeed;
        float originalAttackDamage = berserker.AttackDamage;

        berserker.SetBerserkState(true);
        berserker.DisableManaGain();
        GameObject auraInstance = CreateAuraVFX(berserker);
        Animator auraAnimator = auraInstance?.GetComponent<Animator>();

        if (auraAnimator != null)
        {
            auraAnimator.Play("fairiefirered");
        }

        float elapsedTime = 0f;
        while (remainingDuration > 0)
        {
            // 경과 시간 계산
            float deltaTime = Mathf.Min(Time.deltaTime, remainingDuration);
            elapsedTime += deltaTime;
            remainingDuration -= deltaTime;

            // 크기 계산 (최대 크기에 도달하면 유지)
            float sizeIncreaseFactor = Mathf.Min(elapsedTime / baseDuration, 1f);
            float currentSizeIncrease = Mathf.Lerp(1f, maxSizeIncrease, sizeIncreaseFactor);
            berserker.transform.localScale = originalScale * currentSizeIncrease;


            berserker.AttackDamage = originalAttackDamage + attackDamageIncrease;

            yield return null;
        }

        // Aura VFX 종료
        if (auraAnimator != null)
        {
            Object.Destroy(auraInstance);
        }

        // 스킬 종료 시 원래 크기로 복귀
        berserker.transform.localScale = originalScale;
        berserker.AttackSpeed = originalAttackSpeed;
        berserker.AttackDamage = originalAttackDamage;

        berserker.EnableManaGain();
        berserker.SetBerserkState(false);
    }

    private GameObject CreateAuraVFX(Berserker berserker)
    {
        if (auraVFX != null)
        {
            GameObject auraInstance = Object.Instantiate(auraVFX, berserker.transform.position, Quaternion.identity);
            auraInstance.transform.SetParent(berserker.transform);
            auraInstance.transform.localPosition = Vector3.zero;

            auraInstance.transform.localScale = new Vector3(3, 3, 1);

            return auraInstance;
        }
        return null;
    }

    public void OnDamageDealt(Berserker berserker, float damage)
    {
        float healAmount = damage * healingRatio;
        Vector3 effectPos = berserker.transform.position;
        Vector3 offset = effectPos - berserker.transform.position;
        offset.y += 1f;
        EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(healVFX, berserker.transform, offset, false);
        berserker.Heal(healAmount);
    }

    public void OnEnemyKilled(Berserker berserker)
    {
        Vector3 effectPos = berserker.cachedNearestTarget.transform.position;
        effectPos.y += 1.5f;
        EffectManager.Instance.PlayAnimatedSpriteEffect(killVFX, effectPos, berserker.cachedNearestTarget.IsFacingRight);
        remainingDuration += durationIncreaseOnKill;      
    }
}
