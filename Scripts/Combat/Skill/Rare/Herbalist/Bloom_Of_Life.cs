using UnityEngine;
using System.Collections;
using System.Linq;

[CreateAssetMenu(fileName = "BloomOfLife", menuName = "Skills/Herbalist/BloomOfLife")]
public class BloomOfLife : BaseSkill
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject herbalistSkillPrefab;
    [SerializeField] private GameObject vfx1Prefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float skillDuration = 3f;
    [SerializeField] private float manaGainDisableDuration = 3f;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Herbalist herbalist)
        {
            BaseCombatAI target = FindLowestHealthAlly(herbalist);
            if (target != null)
            {
                herbalist.StartCoroutine(LaunchProjectile(herbalist, target));
                herbalist.StartCoroutine(DisableManaGainTemporarily(herbalist));
            }
        }
    }

    private BaseCombatAI FindLowestHealthAlly(Herbalist herbalist)
    {
        return Physics2D.OverlapCircleAll(herbalist.transform.position, 50f, LayerMask.GetMask("Character"))
            .Select(collider => collider.GetComponent<BaseCombatAI>())
            .Where(ally => ally != null && ally.team == herbalist.team && ally != herbalist)
            .OrderBy(ally => ally.CurrentHP / ally.MaxHP)
            .FirstOrDefault();
    }

    private IEnumerator LaunchProjectile(Herbalist herbalist, BaseCombatAI target)
    {
        Vector2 startPosition = herbalist.transform.position;
        Vector2 targetPosition = target.transform.position;
        GameObject projectile = Object.Instantiate(projectilePrefab, startPosition, Quaternion.identity);
        float journeyLength = Vector2.Distance(startPosition, targetPosition);
        float journeyTime = journeyLength / projectileSpeed;
        float elapsedTime = 0;

        while (elapsedTime < journeyTime)
        {
            float journeyFraction = elapsedTime / journeyTime;
            projectile.transform.position = Vector2.Lerp(startPosition, targetPosition, journeyFraction);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Object.Destroy(projectile);
        yield return PlayVFXAndCreateSkillArea(target, herbalist);
    }

    private IEnumerator PlayVFXAndCreateSkillArea(BaseCombatAI target, Herbalist herbalist)
    {
        
        EffectManager.Instance.PlayAnimatedSpriteEffect(vfx1Prefab, target.transform.position, target.IsFacingRight);

        
        yield return new WaitForSeconds(GetEffectDuration(vfx1Prefab));

        
        GameObject skillObject = Object.Instantiate(herbalistSkillPrefab, target.transform.position, Quaternion.identity);
        HerbalistSkill skillComponent = skillObject.GetComponent<HerbalistSkill>();
        if (skillComponent != null)
        {
            skillComponent.Initialize(herbalist);
        }

        Object.Destroy(skillObject, skillDuration);
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
        return 1f; // 기본값으로 1초 반환
    }

    private IEnumerator DisableManaGainTemporarily(Herbalist herbalist)
    {
        herbalist.DisableManaGain();
        yield return new WaitForSeconds(manaGainDisableDuration);
        herbalist.EnableManaGain();
    }
}
