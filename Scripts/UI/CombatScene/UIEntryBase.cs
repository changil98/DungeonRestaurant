using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIEntryBase : UIBase
{
    [SerializeField] protected Transform background;
    [SerializeField] protected GameObject slotPrefab;

    [SerializeField] protected ToggleGroup toggleGroup;
    [SerializeField] protected Toggle currentActiveToggle;

    public bool IsInit { get; protected set; } = false;

    protected virtual void Awake()
    {
        toggleGroup = background.GetComponent<ToggleGroup>();
    }

    public virtual void OnToggleValueChanged(bool isOn)
    {
        currentActiveToggle = toggleGroup.ActiveToggles().FirstOrDefault();
    }
}
