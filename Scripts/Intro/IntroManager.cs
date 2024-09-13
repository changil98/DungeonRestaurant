using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public static IntroManager Instance;

    public UIIntroScene UIIntroScene { get; set; } = null;

    private float introTime = 3f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(IntroStart(introTime));
    }

    IEnumerator IntroStart(float time)
    {
        SoundManager.Instance.LoadSoundBank("IntroSoundBank");
        yield return new WaitUntil(() => UIIntroScene != null);
        Image bg = UIIntroScene.BGImage;
        Image title = UIIntroScene.TitleImage;
        Camera camera = UIManager.Instance.MainCamera;

        Color bgColor = bg.color;
        Color titleColor = title.color;

        Vector3 startPosition = camera.transform.position;
        Vector3 targetPosition = new Vector3(0.2f, -0.9f, camera.transform.position.z);

        while (bgColor.a > 0.5f)
        {
            bgColor.a -= Time.deltaTime / time;
            bg.color = bgColor;
            yield return null;
        }
        if (!SoundManager.Instance.IsBGMPlaying("BGM_Intro"))
        {
        SoundManager.Instance.PlaySound("BGM_Intro",0.1f);

        }
        while (titleColor.a < 1)
        {
            titleColor.a += Time.deltaTime / time;
            title.color = titleColor;
            yield return null;
        }

        float clickable = 0f;
        bool skipIntro = false;

        while (clickable < time)
        {
            if (Input.GetMouseButtonDown(0))
            {
                titleColor.a = 0f;
                bgColor.a = 0f;
                title.color = titleColor;
                bg.color = bgColor;
                skipIntro = true;
                break;
            }

            clickable += Time.deltaTime;
            yield return null;
        }

        if (!skipIntro)
        {
            yield return new WaitForSeconds(time - clickable);
        }

        while (titleColor.a > 0 || bgColor.a > 0)
        {
            if (titleColor.a > 0)
            {
                titleColor.a -= Time.deltaTime / time;
                title.color = titleColor;
            }

            if (bgColor.a > 0)
            {
                bgColor.a -= Time.deltaTime / time;
                bg.color = bgColor;
            }

            yield return null;
        }

        while (camera.orthographicSize > 0)
        {
            camera.orthographicSize -= Time.deltaTime * time;
            camera.transform.position = Vector3.Lerp(startPosition, targetPosition, 1 - camera.orthographicSize / time);
            if (camera.orthographicSize <= 0)
            {
                camera.orthographicSize = 0;
                GameManager.Instance.RestaurantState();
                yield break;
            }
            yield return null;
        }
    }
}
