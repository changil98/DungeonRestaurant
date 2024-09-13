using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEnterSceneButton : MonoBehaviour
{
    public void OnDungeonEnterSceneButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 7)
        {
            ToturialsManager.Instance.isClear[2] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        GameManager.Instance.DungeonEnterState();
    }
}
