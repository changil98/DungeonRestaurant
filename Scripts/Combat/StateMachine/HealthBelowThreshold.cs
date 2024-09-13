using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class HealthBelowThreshold : IPredicate
{
    private BaseCombatAI character;
    private float threshold;

    public HealthBelowThreshold(BaseCombatAI character, float threshold)
    {
        this.character = character;
        this.threshold = threshold;
    }

    public bool Evaluate()
    {
        return character.CurrentHP <= threshold;
    }

}
