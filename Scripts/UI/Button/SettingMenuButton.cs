using UnityEngine;

public class SettingMenuButton : MonoBehaviour
{
    public void OnSettingMenuButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        UIManager.Instance.ActivePopupUI(UIType.Popup_SettingMenu);
    }
}
