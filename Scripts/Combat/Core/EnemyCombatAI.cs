using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCombatAI : BaseCombatAI
{
    [Title("Character Stat")]
    [SerializeField] protected EnemyInfo info;
    public SpriteRenderer pixlmobSprite;

    private void Start()
    {
        team = Team.Enemy;
        StartCoroutine(WaitData());
    }

    public void SetEnemyData(EnemyInfo info)
    {
        this.info = info;
    }

    IEnumerator WaitData()
    {
        yield return new WaitUntil(() => info != null);
        SetCharacterInfo();
        InitializedTarget();
    }

    protected override void InitializeCharacterStat()
    {
        base.InitializeCharacterStat();
        pixlmobSprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void ApplyCharacterStat()
    {
        base.ApplyCharacterStat();
        MaxHP = info.HP;
        CurrentHP = MaxHP;
        MaxMana = info.MP;
        Mana = 0;
        AttackDamage = info.ATK;
        Def = info.DEF;
        Resistance = info.Resistance;
        MovementSpeed = info.MoveSpeed;
        AttackSpeed = info.AttackSpeed;
        BaseAttackInterval = 1f / AttackSpeed;
        AttackRange = info.Range;
    }

    protected override void UpdateFacingDirection()
    {
        base.UpdateFacingDirection();
        if (cachedNearestTarget != null && GameManager.Instance.isCombatStart)
        {
            Vector3 directionToTarget = cachedNearestTarget.transform.position - transform.position;
            bool shouldFaceRight = directionToTarget.x > 0;
            if (shouldFaceRight)
            {
                pixlmobSprite.flipX = true;
            }
            else
            {
                pixlmobSprite.flipX = false;
            }
        }
        else
        {
            Vector3 currentPosition = transform.position;
            if (currentPosition.x != lastPosition.x)
            {
                bool isMovingRight = currentPosition.x > lastPosition.x;
                
                if (isMovingRight)
                {
                    pixlmobSprite.flipX = true;
                }
                else
                {
                    pixlmobSprite.flipX = false;
                }
            }
            lastPosition = currentPosition;
        }
    }

    public override void Die()
    {
        base.Die();
        SoundManager.Instance.PlaySound("SFX_Enemy_Die");
        StartCoroutine(ShrinkSprite());
    }

    private IEnumerator ShrinkSprite()
    {
        Transform spriteTransform = transform.Find("Sprite");
        SpriteRenderer spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
        float shrinkDuration = 0.5f;
        float elapsedTime = 0f;
        Vector3 originalScale = spriteTransform.localScale;

        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / shrinkDuration;
            float newScaleY = Mathf.Lerp(originalScale.y, 0f, t);

            spriteTransform.localScale = new Vector3(originalScale.x, newScaleY, originalScale.z);

            yield return null;
        }
    }
    public void IncreaseAttackSpeed(float amount)
    {
        AttackSpeedIncrease += amount;
    }

    public void DecreaseAttackSpeed(float amount)
    {
        AttackSpeedDecrease -= amount;
    }
}