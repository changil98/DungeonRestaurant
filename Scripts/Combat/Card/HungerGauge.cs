using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HungerGauge : MonoBehaviour
{
    public Image hungerGauge;
    public Image cookingGauge;
    public GameObject feverImgae;
    public GameObject hungerImgae;
    public GameObject feverEndImage;

    public IHungerState currentState;
    public DefaultState defaultState;
    public HungerState hungerState;
    public FeverState feverState;
    public FeverEndState feverEndState;

    public bool isFeverEnd = false;

    private void Awake()
    {
        CardDeck.Instance.hungerGauge = this;
        defaultState = new DefaultState(this);
        hungerState = new HungerState(this);
        feverState = new FeverState(this);
        feverEndState = new FeverEndState(this);

    }

    private void Start()
    {

        SetState(defaultState);
        StartCoroutine(Cooking());

    }

    private void FixedUpdate()
    {
        if (currentState == hungerState || currentState == feverState || currentState == feverEndState) 
        {
            currentState.UpdateState();
        }
    }

    public void SetState(IHungerState newState)
    {
        currentState = newState;
        newState.EnterState();
    }

    public IEnumerator Hunger()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (hungerGauge != null)
            {
                if (hungerGauge.fillAmount <= 0f)
                {
                    SetState(hungerState);
                    break;
                }
                else if (hungerGauge.fillAmount >= 1f)
                {
                    SetState(feverState);
                    break;
                }
                hungerGauge.fillAmount -= 0.02f;
            }
            if (!GameManager.Instance.isCombatStart)
            {
                break;
            }
        }
    }

    IEnumerator Cooking()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (cookingGauge != null)
            {
                cookingGauge.fillAmount += 0.1f;
                if (cookingGauge.fillAmount >= 1f)
                {
                    if (CardDeck.Instance.cards.Count <= 2f) 
                    {
                        CardDeck.Instance.Draw();
                        cookingGauge.fillAmount = 0f;
                    }
                    else
                    {
                        cookingGauge.fillAmount = 1f;
                    }
                }
            }
            if (!GameManager.Instance.isCombatStart)
            {
                break;
            }
        }
    }
}
