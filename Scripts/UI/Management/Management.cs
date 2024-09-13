using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Management : MonoBehaviour
{
    public CharacterData characterData;

    public CharacterData CharacterData
    {
        get { return characterData; }
        set
        {
            characterData = value;
            SetImage();
        }
    }
    private Toggle toggle;
    public Image backgroundImage;
    public Image characterImage;
    public Image skillImg;
    public TextMeshProUGUI name;
    public TextMeshProUGUI levelTxt;
    public GameObject infoManagement;
    public ManageSlotInstance manageSlotInstance;

    public int index;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OnToggleValueChanged(bool isOn)
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 31)
        {
            ToturialsManager.Instance.isClear[14] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        if (isOn)
        {
            StartCoroutine(SetCharacterInfo());
        }
    }

    private IEnumerator SetCharacterInfo()
    {
        GameObject infoInstance = Instantiate(infoManagement);
        InfoManagement infoManagementScript = infoInstance.GetComponent<InfoManagement>();

        infoManagementScript.SetInfo(CharacterData, index);
        toggle.isOn = false;
        yield return null;
    }

    public void SetImage()
    {
        if (characterData != null)
        {
            SetImageAlpha(characterImage, 1f);
            characterImage.sprite = characterData.skin.characterThumnail;
            skillImg.sprite = characterData.skill.skillImg;
            name.text = characterData.characterName;
            levelTxt.text = characterData.Level.ToString();
        }
        else if (characterData == null)
        {
            SetImageAlpha(characterImage, 0f);
            characterImage.sprite = null;
        }
    }

    public void SetImageAlpha(Image image, float alpha)
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
}
