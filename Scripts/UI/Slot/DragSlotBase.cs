using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlotBase : MonoBehaviour
{
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected Image characterImage;
    [SerializeField] protected Image skillImageFrame;
    [SerializeField] protected Image skillImage;

    protected CharacterData characterData;

    protected delegate void DataUpdateHandler(CharacterData characterData);
    protected DataUpdateHandler OnDataUpdate;

    protected virtual void Awake()
    {
        OnDataUpdate += SetImage;
        IntializeSlotImage();
        SetActive(false);
    }

    protected void IntializeSlotImage()
    {
        SetImageAlpha(characterImage, 0.6f);
        SetImageAlpha(backgroundImage, 0.6f);
        SetImageAlpha(skillImage, 0.6f);
        SetImageAlpha(skillImageFrame, 0.6f);
    }

    protected virtual void SetImage(CharacterData characterData)
    {
        if (characterData != null)
        {
            characterImage.sprite = characterData.skin.characterThumnail;
            skillImage.sprite = characterData.skill.skillImg;
            SetImageAlpha(characterImage, 0.6f);
            skillImageFrame.gameObject.SetActive(true);
        }
        else if (characterData == null)
        {
            characterImage.sprite = null;
            SetImageAlpha(characterImage, 0f);
            skillImageFrame.gameObject.SetActive(false);
        }
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

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
