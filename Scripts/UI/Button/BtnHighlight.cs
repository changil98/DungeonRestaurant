using UnityEngine;

public class BtnHighlight : MonoBehaviour
{
    public RectTransform targetObject;
    public GameObject text;
    public GameObject icon;

    private Vector2 height = new Vector2(100, 200);
    private Vector2 originalHeight;

    private void Awake()
    {
        originalHeight = targetObject.sizeDelta;
    }

    public void OnPointerEnter()
    {
        if (targetObject != null)
        {
            SoundManager.Instance.PlaySound("SFX_UI_Slide");
            targetObject.sizeDelta = height;
            text.SetActive(true);
            icon.SetActive(false);
        }
    }

    public void OnPointerExit()
    {
        if (targetObject != null)
        {
            targetObject.sizeDelta = originalHeight;
            text.SetActive(false);
            icon.SetActive(true);
        }
    }
}
