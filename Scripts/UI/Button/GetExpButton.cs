using UnityEngine;

public class GetExpButton : MonoBehaviour
{
    public void OnButtonClick()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        //UserInfo.userInfo.ExpUpUsingMedal();
    }
}
