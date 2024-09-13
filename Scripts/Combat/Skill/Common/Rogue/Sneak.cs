using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Rogue Sneak Skill", menuName = "Skills/Rogue/Sneak")]
public class Sneak : BaseSkill
{
    public float teleportDuration = 0.5f;
    public float untargetableDuration = 2f;
    public GameObject teleportStartVFX;
    public GameObject teleportEndVFX;

    public Sneak()
    {
        useOnCombatStart = true;
    }

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Rogue rogue)
        {
            rogue.StartCoroutine(PerformSneak(rogue));
        }
    }

    private IEnumerator PerformSneak(Rogue rogue)
    {
        
        rogue.PlayAnimation(AnimationData.Skill_NormalHash);
        SoundManager.Instance.PlaySound("SFX_RogueBuff");
        BaseCombatAI lowestHealthEnemy = FindLowestHealthEnemy(rogue);
        if (lowestHealthEnemy != null)
        {
            Vector3 startPosition = rogue.transform.position;
            Vector3 targetPosition = lowestHealthEnemy.transform.position;

            // ���� ����� �̹� ���� ���� ü���� ���� ��� ���ڸ��� �ӹ���
            bool shouldTeleport = lowestHealthEnemy != rogue.cachedNearestTarget;

            // ���� ����Ʈ
            Vector2 effectOffset = rogue.transform.position;
            effectOffset.y += 2f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(teleportStartVFX, effectOffset, false);

            if (shouldTeleport)
            {
                float elapsedTime = 0f;
                while (elapsedTime < teleportDuration)
                {
                    rogue.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / teleportDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // ���� ��ġ ����
                rogue.transform.position = targetPosition;

                // ���� ����Ʈ
                EffectManager.Instance.PlayAnimatedSpriteEffect(teleportEndVFX, targetPosition, false);
            }
            else
            {
                // ���ڸ� ����Ʈ
                yield return new WaitForSeconds(teleportDuration);
                EffectManager.Instance.PlayAnimatedSpriteEffect(teleportEndVFX, rogue.transform.position, false);
            }

            // ���ο� Ÿ�� ����
            rogue.cachedNearestTarget = lowestHealthEnemy;
        }

        rogue.BecomeUntargetable(untargetableDuration);
    }

    private BaseCombatAI FindLowestHealthEnemy(Rogue rogue)
    {
        BaseCombatAI lowestHealthEnemy = null;
        float lowestHealth = float.MaxValue;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(rogue.transform.position, 50f, LayerMask.GetMask("Character"));

        foreach (Collider2D enemyCollider in enemies)
        {
            BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy.team != rogue.team && enemy.CurrentHP < lowestHealth)
            {
                lowestHealth = enemy.CurrentHP;
                lowestHealthEnemy = enemy;
            }
        }

        return lowestHealthEnemy;
    }
}