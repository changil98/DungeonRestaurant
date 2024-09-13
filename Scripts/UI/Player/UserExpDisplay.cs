using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserExpDisplay : MonoBehaviour
{
    [SerializeField] private Slider expSlider;

    void Start()
    {
        expSlider.maxValue = UserInfo.userInfo.MaxExp;
        UserInfo.userInfo.SetExpEvent(SetUserExp);
        expSlider.interactable = false;
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveExpEvent(SetUserExp);
    }

    public void SetUserExp(int exp)
    {
        expSlider.value = exp % UserInfo.userInfo.MaxExp;
    }
}
