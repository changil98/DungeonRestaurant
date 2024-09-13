using UnityEngine;
using UnityEngine.Events;


// TODO : 특강 UIBase 코드임. 필요한 UI에 상속시켜서 사용
public class UIBase : MonoBehaviour
{
    //public eUIPosition uiPosition;
    public UnityAction<object[]> opened;
    public UnityAction<object[]> closed;

    private void Awake()
    {
        opened = OnOpened;
        closed = OnClosed;
    }

    public virtual void SetActive(bool isActive)
    {
        if(!isActive)
        {
            SoundManager.Instance.PlaySound("SFX_UI_Click");
        }
        gameObject.SetActive(isActive);
    }

    public virtual void HideDirect() { }

    public virtual void OnOpened(object[] param) { }

    public virtual void OnClosed(object[] param) { }
}
