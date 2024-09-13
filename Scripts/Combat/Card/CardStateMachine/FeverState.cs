using System.Collections.Generic;
using UnityEngine;

public class FeverState : IHungerState
{
    private HungerGauge hungerGauge { get; set; }
    private float startTime;
    private Dictionary<int, float> OGMoveSpeed = new Dictionary<int, float>();

    public FeverState(HungerGauge hungerGauge)
    {
        this.hungerGauge = hungerGauge;
    }

    public void EnterState()
    {
        hungerGauge.feverImgae.SetActive(true);
        for (int i = 0; i < CardDeck.Instance.combatCharacterAIList.Count; i++)
        {
            var character = CardDeck.Instance.combatCharacterAIList[i];
            OGMoveSpeed[i] = character.MovementSpeed;

            character.IncreaseAttackSpeed(0.2f);
            character.MovementSpeed *= 1.2f;
        }
        startTime = Time.time;
    }

    public void UpdateState()
    {
        if (Time.time - startTime >= 20f)
        {
            ExitState();
        }
    }

    public void ExitState()
    {
        hungerGauge.feverImgae.SetActive(false);
        for (int i = 0; i < CardDeck.Instance.combatCharacterAIList.Count; i++)
        {
            var character = CardDeck.Instance.combatCharacterAIList[i];
            character.DecreaseAttackSpeed(0.2f);
            character.MovementSpeed = OGMoveSpeed[i];
        }
        this.hungerGauge.hungerGauge.fillAmount = 0.2f;
        hungerGauge.SetState(hungerGauge.feverEndState);
    }
}