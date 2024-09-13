using UnityEngine;

public class RestaurantSceneButton : MonoBehaviour
{
    public void OnRestaurantSceneButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 21)
        {
            ToturialsManager.Instance.isClear[6] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        GameManager.Instance.RestaurantState();
        Time.timeScale = 1.0f;
    }
}
