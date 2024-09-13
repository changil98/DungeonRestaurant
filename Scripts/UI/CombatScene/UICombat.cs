using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UICombat : UIBase
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text dungeonNameText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private GameObject pause;

    private void Awake()
    {
        GameManager.Instance.combatController.timeText = timeText;
        GameManager.Instance.combatController.waveText = waveText;
        GameManager.Instance.combatController.progressBar = progressBar;
        GameManager.Instance.combatController.dungeonNameText = dungeonNameText;
        GameManager.Instance.combatController.stageText = stageText;
    }

    private void Start()
    {
        SetActive(false);
    }

    public void PauseButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        pause.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClosePauseButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        pause.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
