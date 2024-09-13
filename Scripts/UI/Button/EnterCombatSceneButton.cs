
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCombatSceneButton : MonoBehaviour
{
    public void OnCombatSceneButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 10)
        {
            ToturialsManager.Instance.isClear[3] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        GameManager.Instance.CombatState();
    }
}
