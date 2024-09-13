using UnityEngine;

public class InstantiateUIBtn : MonoBehaviour
{
    public GameObject UI;
    public GameObject UIInstance;

    public void OpenInstantiateUI()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 3)
        {
            ToturialsManager.Instance.isClear[0] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        if (UIInstance == null)
        {
            UIInstance = Instantiate(UI);
        }
        else
        {
            if (UIInstance.activeSelf == false)
            {
                UIInstance.SetActive(true);

                CharacterEmploymentList characterEmploymentList = UIInstance.GetComponentInChildren<CharacterEmploymentList>();
                if (characterEmploymentList != null)
                {
                    characterEmploymentList.SetCharacterCount();
                }
            }
        }
    }

    public void BackBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 6)
        {
            ToturialsManager.Instance.isClear[1] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        UI.SetActive(false);
    }
}
