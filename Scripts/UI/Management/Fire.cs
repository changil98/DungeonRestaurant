using UnityEngine;

public class Fire : MonoBehaviour
{
    public InfoManagement infoManagement;

    private void Awake()
    {
        infoManagement = FindObjectOfType<InfoManagement>();
    }

    public void YesFire()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        DataManager.Instance.characterList.RemoveAt(infoManagement.index);
        infoManagement.Fire();
        FindObjectOfType<ManageSlotInstance>().Refresh();
        Destroy(gameObject);
    }

    public void NoFire()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        Destroy(gameObject);
    }
}
