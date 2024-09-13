using TMPro;
using UnityEngine;

public class UserLevelDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text userLevelText;

    void Start()
    {
        UserInfo.userInfo.SetUserLevelEvent(SetUserLevel);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.SetUserLevelEvent(SetUserLevel);
    }

    public void SetUserLevel(int level)
    {
        userLevelText.text = level.ToString();
    }
}
