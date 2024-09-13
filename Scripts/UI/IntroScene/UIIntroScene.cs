using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIIntroScene : UIBase
{
    public Image BGImage;
    public Image TitleImage;

    private void Awake()
    {
        UIManager.Instance.AddUIDictionary(UIType.IntroUI, this);
        StartCoroutine(SetIntroManager());
    }

    IEnumerator SetIntroManager()
    {
        yield return new WaitUntil(() => IntroManager.Instance != null);
        IntroManager.Instance.UIIntroScene = this;
    }
}
