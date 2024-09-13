using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public static CardDeck Instance;

    [Title("Card List")]
    public List<RecipeData> recipeInfos = new List<RecipeData>();
    public List<GameObject> cards = new List<GameObject>();
    public GameObject[] Position;

    [Title("CombatCharacter")]
    public List<PlayerCombatAI> combatCharacterAIList = new List<PlayerCombatAI>();
    public GameObject card;
    public HungerGauge hungerGauge;

    private List<RecipeData> recipeInfosCommon = new List<RecipeData>();
    private List<RecipeData> recipeInfosMagic = new List<RecipeData>();
    private List<RecipeData> recipeInfosLegend = new List<RecipeData>();

    private void Awake()
    {
        Instance = this;
        recipeInfosCommon = RecipeDataManager.Instance.recipesCommon;
        recipeInfosMagic = RecipeDataManager.Instance.recipesMagic;
        recipeInfosLegend = RecipeDataManager.Instance.recipesLegend;
        combatCharacterAIList = CombatController.instance.playerCharacters;
    }

    private void Start()
    {
        StartCoroutine(InitializeCharacters());
    }

    private IEnumerator InitializeCharacters()
    {
        yield return new WaitUntil(() => CombatController.instance != null);

        if (CombatController.instance == null)
        {
            Debug.LogError("CombatController instance is null");
        }

        FirstDraw();
    }

    public void Draw()
    {
        int randomidx;
        do
        {
            RareCard();
            randomidx = Random.Range(0, recipeInfos.Count);
        } while (recipeInfos[randomidx].recipeCurLevel == 0);
        GameObject newCard = Instantiate(card, this.transform);
        CardInfo cardInfo = newCard.GetComponentInChildren<CardInfo>();
        cardInfo.recipeInfo = recipeInfos[randomidx];
        cards.Add(newCard);
        SetCardPositions();
    }

    public void Darw(RecipeData recipe) // for debug
    {
        GameObject newCard = Instantiate(card, this.transform);
        CardInfo cardInfo = newCard.GetComponentInChildren<CardInfo>();
        cardInfo.recipeInfo = recipe;
        cards.Add(newCard);
        SetCardPositions();
    }

    private void RareCard()
    {
        float randomIdx = Random.Range(0, 1f);
        if (randomIdx < 0.75)
        {
            int idx = Random.Range(0, recipeInfosCommon.Count);
            recipeInfos = recipeInfosCommon;
        }
        else if (randomIdx < 0.99)
        {
            int idx = Random.Range(0, recipeInfosMagic.Count);
            recipeInfos = recipeInfosMagic;
        }
        else
        {
            int idx = Random.Range(0, recipeInfosLegend.Count);
            recipeInfos = recipeInfosLegend;
        }
    }

    private void SetCardPositions()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (i < Position.Length)
            {
                cards[i].transform.position = Position[i].transform.position;
            }
        }
    }

    public void UseCard()
    {
        SetCardPositions();
    }

    private void FirstDraw()
    {
        for (int i = 0; i < 3; i++)
        {
            Draw();
        }
    }
}
