using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class EntrySlotBase : MonoBehaviour, IDataChangeHandler
{
    [Title("Character Data")]
    [SerializeField] protected CharacterData characterData;
    public CharacterData CharacterData
    {
        get { return characterData; }
        set
        {
            characterData = value;
            OnDataChange?.Invoke();
        }
    }

    public event DataChangeEventHandler OnDataChange;
    [ShowInInspector] public int Index { get; set; }



    [Title("UI")]
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected Image characterImage;
    [SerializeField] protected GameObject skillImageFrame;
    [SerializeField] protected Image skillImage;
    public Toggle toggle;

    

    protected virtual void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnDataChange += SetImage;
    }


    public void OnToggleValueChanged(bool isOn)
    {
        SetSelectionEffect();
    }

    protected void SetImage()
    {
        if (characterData != null)
        {
            SetImageAlpha(characterImage, 1f);
            characterImage.sprite = characterData.skin.characterThumnail;
            skillImage.sprite = characterData.skill.skillImg;
            skillImageFrame.SetActive(true);
        }
        else if (characterData == null)
        {
            SetImageAlpha(characterImage, 0f);
            characterImage.sprite = null;
            skillImage.sprite = null;
            skillImageFrame.SetActive(false);
        }
    }

    public bool IsSlotEmpty()
    {
        if (CharacterData == null)
            return true;
        else
            return false;

    }

    protected void SetImageAlpha(Image image, float alpha)
    {
        if (alpha < 0f || alpha > 1f)
        {
            Debug.Log("alpha °ªÀº 0f~1f");
            return;
        }
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public void SetSelectionEffect()
    {
        if (toggle.isOn)
        {
            backgroundImage.color = Color.yellow;
        }
        else
        {
            backgroundImage.color = Color.white;
        }
    }
}
