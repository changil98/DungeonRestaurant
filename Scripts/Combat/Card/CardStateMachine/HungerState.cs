
using System.Collections.Generic;
using UnityEngine;

public class HungerState : IHungerState
{
    private HungerGauge hungerGauge { get; set; }
    private float hungerFillmount;
    private Dictionary<int, float> OGMoveSpeed = new Dictionary<int, float>();

    public HungerState(HungerGauge hungerGauge)
    {
        this.hungerGauge = hungerGauge;
    }

    public void EnterState()
    {
        hungerGauge.hungerImgae.SetActive(true);
        hungerFillmount = hungerGauge.hungerGauge.fillAmount;
        for(int i = 0; i < CardDeck.Instance.combatCharacterAIList.Count; i++)
        {
            var character = CardDeck.Instance.combatCharacterAIList[i];
            OGMoveSpeed[i] = character.MovementSpeed;


            character.DecreaseAttackSpeed(0.2f);
            character.MovementSpeed *= 0.8f;
        }
    }

    public void UpdateState()
    {
        if (hungerFillmount < hungerGauge.hungerGauge.fillAmount)
        {
            ExitState();
        }
        
    }

    public void ExitState()
    {
        hungerGauge.hungerImgae.SetActive(false);
        for (int i = 0; i < CardDeck.Instance.combatCharacterAIList.Count; i++)
        {
            var character = CardDeck.Instance.combatCharacterAIList[i];
            character.IncreaseAttackSpeed(0.2f);
            character.MovementSpeed = OGMoveSpeed[i];
        }
        hungerGauge.SetState(hungerGauge.defaultState);
    }
}
