using UnityEngine;

public class DeleteSaveDataButton : MonoBehaviour
{
    public void OnDeleteDataButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        DataManager.Instance.DeleteSaveData();
    }
}
