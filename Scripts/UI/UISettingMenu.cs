using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingMenu : UIBase
{
    [SerializeField] private UIBase saveCompletePopup;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button fullScreenButoon;
    [SerializeField] private Button floatingTextButton;
    [SerializeField] private Image floatingTextButtonImage;
    [SerializeField] private TextMeshProUGUI floatingText;
    [SerializeField] private Button gameQuitButton;
    [SerializeField] private UIBase deleteMenu;

    private void Awake()
    {
        UIManager.Instance.AddUIDictionary(UIType.Popup_SettingMenu, this);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        fullScreenButoon.onClick.AddListener(OnScreenModeChanged);
        floatingTextButton.onClick.AddListener(OnFloatingTextChanaged);
        gameQuitButton.onClick.AddListener(GameQuit);
    }

    private void Start()
    {
        bgmVolumeSlider.value = SoundManager.Instance.bgmVolume;
        sfxVolumeSlider.value = SoundManager.Instance.sfxVolume;
    }

    public void OnSaveButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        DataManager.Instance.SaveData();
        saveCompletePopup.SetActive(true);
    }

    public void OnFloatingTextChanaged()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (FloatingTextManager.Instance.isFloatingTextEnabled)
        {
            FloatingTextManager.Instance.isFloatingTextEnabled = false;
            floatingText.text = "Off";

            Color buttonColor = floatingTextButtonImage.color;
            buttonColor.a = 50f / 255f;
            floatingTextButtonImage.color = buttonColor;
        }
        else
        {
            FloatingTextManager.Instance.isFloatingTextEnabled = true;
            floatingText.text = "On";

            Color buttonColor = floatingTextButtonImage.color;
            buttonColor.a = 1.0f;
            floatingTextButtonImage.color = buttonColor;
        }
    }

    public void OnScreenModeChanged()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }

    public void OnDeleteDataMenuButton()
    {
        deleteMenu.SetActive(true);
    }

    private void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
    Application.Quit();
#elif UNITY_WEBGL
    Application.ExternalEval("window.close();");
#elif UNITY_STANDALONE
    Application.Quit();
#endif
    }
}
