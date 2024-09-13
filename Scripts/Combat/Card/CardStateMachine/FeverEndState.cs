using System.Collections.Generic;
using UnityEngine;

public class FeverEndState : IHungerState
{
    private HungerGauge hungerGauge { get; set; }
    private float startTime;
    private Dictionary<int, float> OGMoveSpeed = new Dictionary<int, float>();

    public FeverEndState(HungerGauge hungerGauge)
    {
        this.hungerGauge = hungerGauge;
    }

    public void EnterState()
    {
        hungerGauge.feverEndImage.SetActive(true);
        startTime = Time.time;
        hungerGauge.isFeverEnd = true;
        for (int i = 0; i < CardDeck.Instance.combatCharacterAIList.Count; i++)
        {
            var character = CardDeck.Instance.combatCharacterAIList[i];
            OGMoveSpeed[i] = character.MovementSpeed;

            character.DecreaseAttackSpeed(0.2f);
            character.MovementSpeed *= 0.8f;
        }
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
        hungerGauge.isFeverEnd = false;
        hungerGauge.feverEndImage.SetActive(false);
        this.hungerGauge.hungerGauge.fillAmount = 0.2f;
        for (int i = 0; i < CardDeck.Instance.combatCharacterAIList.Count; i++)
        {
            var character = CardDeck.Instance.combatCharacterAIList[i];          
            character.IncreaseAttackSpeed(0.2f);
            character.MovementSpeed = OGMoveSpeed[i];
        }
        hungerGauge.SetState(hungerGauge.defaultState);
    }


}
