using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour
{
    public RecipeData info;

    public GameObject targetObject; // 크기가 변경될 객체
    private Vector3 scale = new Vector3(1.2f, 1.2f, 1); // 마우스를 올렸을 때의 크기
    private Vector3 originalScale; // 원래 크기

    public Toggle toggle;
    public Image foodImage;
    public Slider collectionSlide;
    public TextMeshProUGUI foodLVTxt;
    public TextMeshProUGUI foodNameTxt;
    public TextMeshProUGUI collectPercent;

    public GameObject recipeDescription;
    public TextMeshProUGUI description;

    public Button button;

    private void Start()
    {
        if (targetObject != null)
        {
            originalScale = targetObject.transform.localScale; // 원래 크기 저장
        }

        toggle.onValueChanged.AddListener(ShowDescription);
        button.onClick.AddListener(HideDescription);

        InitializeUI();
        collectionSlide.interactable = false;
    }

    public void OnPointerEnter()
    {
        if (targetObject != null)
        {
            targetObject.transform.localScale = scale;
        }
    }

    public void OnPointerExit()
    {
        if (targetObject != null)
        {
            targetObject.transform.localScale = originalScale;
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(ShowDescription);
        button.onClick.RemoveListener(HideDescription);
    }

    private void InitializeUI()
    {
        foodImage.sprite = info.recipeInfo.sprite;
        if (info.recipeCurLevel == 0)
        {
            foodImage.color = new Color(0.65f, 0.65f, 0.65f, 1f);
        }
        foodNameTxt.text = info.recipeInfo.recipeName;
        UpdateUI();
    }

    private void UpdateUI()
    {
        foodLVTxt.text = "LV." + info.recipeCurLevel;
        if (info.recipeCurLevel < info.recipeMaxLevel)
        {
            collectPercent.text = info.recipeCurExp + " / " + info.recipeMaxExp;
            collectionSlide.value = (float)info.recipeCurExp / info.recipeMaxExp;

        }
        else
        {
            collectPercent.text = "Max";
            collectionSlide.value = 1f;
        }
    }

    public void ShowDescription(bool isOn)
    {
        if (isOn)
        {
            SoundManager.Instance.PlaySound("SFX_UI_Click");
            if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 24)
            {
                ToturialsManager.Instance.isClear[8] = true;
                ToturialsManager.Instance.OnNextPhase();
            }
            StartCoroutine(ShowDescription(0.5f));
        }
    }

    IEnumerator ShowDescription(float time)
    {
        recipeDescription.SetActive(true);
        description.text = info.recipeInfo.recipeDescription;
        yield return new WaitForSeconds(time);
        toggle.isOn = false;
    }

    public void HideDescription()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        recipeDescription.SetActive(false);
    }
}
