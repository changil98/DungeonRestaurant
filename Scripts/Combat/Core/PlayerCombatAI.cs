using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCombatAI : BaseCombatAI
{
    [Title("Character Stat")]
    [SerializeField] protected CharacterData characterData;
    [Title("Camera Target")]
    public Transform cameraTarget;

    private float baseAttackSpeed;
    private float currentAttackSpeedModifier = 0f;

    protected override void Awake()
    {
        base.Awake();
        CreateCameraTarget();
        CharacterSortingManager.Instance.RegisterCharacter(this);
    }

    private void Start()
    {
        team = Team.Ally;
        StartCoroutine(WaitData());
    }

    public void SetPlayerData(CharacterData data)
    {
        characterData = data;
    }

    IEnumerator WaitData()
    {
        yield return new WaitUntil(() => characterData != null);
        SetCharacterInfo();
        InitializedTarget();
    }

    private void CreateCameraTarget()
    {
        GameObject targetObject = new GameObject("CameraTarget");
        cameraTarget = targetObject.transform;
        cameraTarget.SetParent(transform);
        cameraTarget.localPosition = Vector3.zero;
        cameraTarget.localPosition += Vector3.up * 1.5f;
    }


    protected override void InitializeCharacterStat()
    {
        base.InitializeCharacterStat();
        CameraController.Instance.RegisterPlayerCharacter(this);
    }

    protected override void ApplyCharacterStat()
    {
        base.ApplyCharacterStat();
        MaxHP = characterData.Stat.HP;
        CurrentHP = MaxHP;
        MaxMana = characterData.skill.manaCost;
        Mana = 0;
        AttackDamage = characterData.Stat.ATK;
        Def = characterData.Stat.DEF;
        originalDef = Def;
        Resistance = characterData.Stat.Resistance;
        MovementSpeed = characterData.Stat.MoveSpeed;
        AttackSpeed = characterData.Stat.AttackSpeed;
        BaseAttackInterval = 1f / AttackSpeed;
        AttackRange = characterData.Stat.Range;
        CriticalPersent = characterData.Stat.CriticalPercent;
        CriticalDamage = characterData.Stat.CriticalDamage;
        currentSkill = characterData.skill;
    }

    
    protected override void UpdateFacingDirection()
    {
        base.UpdateFacingDirection();
        if (cachedNearestTarget != null && GameManager.Instance.isCombatStart)
        {
            // 현재 대상을 향해 방향 설정
            Vector3 directionToTarget = cachedNearestTarget.transform.position - transform.position;
            bool shouldFaceRight = directionToTarget.x > 0;

            if (shouldFaceRight == IsFacingRight)
            {
                Flip();
            }
        }
        else
        {
            // 기존의 이동 기반 방향 설정 로직
            Vector3 currentPosition = transform.position;
            if (currentPosition.x != lastPosition.x)
            {
                bool isMovingRight = currentPosition.x > lastPosition.x;
                bool shouldFaceRight = isMovingRight;

                if (shouldFaceRight == IsFacingRight)
                {
                    Flip();
                }
            }
            lastPosition = currentPosition;
        }
    }

    public override void Die()
    {
        base.Die();
        SoundManager.Instance.PlaySound("SFX_Hero_Die");
        CameraController.Instance.UnregisterPlayerCharacter(this);
        CharacterSortingManager.Instance.UnregisterCharacter(this);
    }

    public void IncreaseAttackSpeed(float amount)
    {
        AttackSpeedIncrease += amount;
    }

    public void DecreaseAttackSpeed(float amount)
    {
        AttackSpeedDecrease -= amount;
    }

    public void IncreaseMovementSpeed(float amount)
    {
        MovementSpeed += amount;
    }

    public void DecreaseMovementSpeed(float amount)
    {
        MovementSpeed -= amount;
    }

    public float GetMovementSpeed()
    {
        return MovementSpeed;
    }
}
