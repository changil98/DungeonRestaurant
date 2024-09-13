using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyDescriptionPanel : MonoBehaviour, IDataChangeHandler
{
    private EnemyInfo enemyInfo;
    public EnemyInfo EnemyInfo
    {
        get => enemyInfo;
        set
        {
            enemyInfo = value;
            OnDataChange?.Invoke();
        }
    }

    public Image enemyImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public event DataChangeEventHandler OnDataChange;

    private void Start()
    {
        OnDataChange += SetUI;
        SetActive(false);
    }

    private void SetUI()
    {
        enemyImage.sprite = enemyInfo.Prefab.thumnailSprite;
        nameText.text = enemyInfo.EnemyName;
        descriptionText.text = enemyInfo.Description;
    }

    public void SetActive(bool isActive)
    {
        if(isActive)
        {
            SoundManager.Instance.PlaySound("SFX_UI_Slide");
        }       
        gameObject.SetActive(isActive);
    }
}
