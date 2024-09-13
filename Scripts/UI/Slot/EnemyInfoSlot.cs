using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnemyInfoSlot : MonoBehaviour, IDataChangeHandler, IPointerEnterHandler, IPointerExitHandler
{
    [ShowInInspector, ReadOnly]
    private EnemyInfo data;
    public EnemyInfo Data
    {
        get => data;
        set
        {
            data = value;
            OnDataChange?.Invoke();
        }
    }

    public event DataChangeEventHandler OnDataChange;

    [Title("UI")]
    [SerializeField] private Image enemyImage;
    [SerializeField] public UIEnemyDescriptionPanel descriptionPanel;

    private void Awake()
    {
        OnDataChange += SetUI;
    }
    private void SetUI()
    {
        if (data != null)
        {
            enemyImage.sprite = data.Prefab.thumnailSprite;
            gameObject.SetActive(true);
        }
        else
        {
            enemyImage.sprite = null;
            gameObject.SetActive(false);
        }
    }
    public bool IsEmpty()
    {
        if (data == null)
            return true;
        else
            return false;
    }

    public void Clear()
    {
        Data = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.EnemyInfo = data;
        descriptionPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.SetActive(false);
    }
}
