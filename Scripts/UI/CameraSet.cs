using UnityEngine;

public class CameraSet : MonoBehaviour
{
    private Canvas canvas;

    private void Awake()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();
        canvas.worldCamera = UIManager.Instance.MainCamera;
    }
}
