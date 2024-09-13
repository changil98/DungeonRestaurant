using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HerbalistSkill : MonoBehaviour
{
    private CircleCollider2D skillArea;
    [SerializeField] private float healInterval = 0.5f;
    [SerializeField] private float healMultiplier = 0.5f;
    [SerializeField] private float attackSpeedIncreasePercent = 0.1f;
    [SerializeField] private GameObject skillEffect;
    private List<PlayerCombatAI> affectedPlayers = new List<PlayerCombatAI>();
    private Coroutine healRoutine;
    private Herbalist herbalist;

    private void Awake()
    {
        skillArea = GetComponent<CircleCollider2D>();
        if (skillArea == null)
        {
            skillArea = gameObject.AddComponent<CircleCollider2D>();
        }
        skillArea.isTrigger = true;
    }

    public void Initialize(Herbalist herbalist)
    {
        this.herbalist = herbalist;
        StartSkillEffect();
        herbalist.DisableManaGain();
    }

    private void StartSkillEffect()
    {
        healRoutine = StartCoroutine(HealRoutine());
    }

    private void OnDisable()
    {
        StopSkillEffect();
    }

    public void StopSkillEffect()
    {
        if (healRoutine != null)
        {
            StopCoroutine(healRoutine);
        }
        ResetBuffs();
        herbalist.EnableManaGain();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCombatAI player = other.GetComponent<PlayerCombatAI>();
        if (player != null && !affectedPlayers.Contains(player))
        {
            affectedPlayers.Add(player);
            ApplyAttackSpeedBuff(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerCombatAI player = other.GetComponent<PlayerCombatAI>();
        if (player != null && affectedPlayers.Contains(player))
        {
            RemoveAttackSpeedBuff(player);
            affectedPlayers.Remove(player);
        }
    }

    private IEnumerator HealRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(healInterval);
            float healAmount = herbalist.AttackDamage * healMultiplier;
            foreach (var player in affectedPlayers)
            {
                player.Heal(healAmount);
            }
        }
    }

    private void ApplyAttackSpeedBuff(PlayerCombatAI player)
    {
        float originalAttackSpeed = player.AttackSpeed;
        float increasedAttackSpeed = originalAttackSpeed * (1 + attackSpeedIncreasePercent);
        player.AttackSpeed = increasedAttackSpeed;
        player.BaseAttackInterval = 1f / increasedAttackSpeed;

        Vector3 effectPos = player.transform.position;
        Vector3 offset = effectPos - player.transform.position;
        offset.y += 2f;
        EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(skillEffect, player.transform, offset, false);
    }

    private void RemoveAttackSpeedBuff(PlayerCombatAI player)
    {
        float originalAttackSpeed = player.AttackSpeed / (1 + attackSpeedIncreasePercent);
        player.AttackSpeed = originalAttackSpeed;
        player.BaseAttackInterval = 1f / originalAttackSpeed;
    }

    private void ResetBuffs()
    {
        foreach (var player in affectedPlayers)
        {
            RemoveAttackSpeedBuff(player);
        }
        affectedPlayers.Clear();
    }
}