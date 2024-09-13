using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStartButton : MonoBehaviour
{
    public void OnCombatStartButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        GameManager.Instance.CombatStart();
    }
}
