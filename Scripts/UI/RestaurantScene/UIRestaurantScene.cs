using UnityEngine;

public class UIRestaurantScene : UIBase
{
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        UIManager.Instance.AddUIDictionary(UIType.RestaurantUI, this);
    }
}
