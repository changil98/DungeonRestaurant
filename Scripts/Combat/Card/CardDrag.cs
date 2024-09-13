using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour , IBeginDragHandler , IDragHandler , IEndDragHandler 
{
    public CardInfo cardInfo;
    public Vector3 defaultPos;

    private bool isDrag = false;
    private bool isCombatStart;
    private bool isEndFever = false;
    private bool isSetPosition = false;

    private Canvas canvas;
    private CanvasGroup canvasGroup;


    private void Awake()
    {
        cardInfo = GetComponent<CardInfo>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        StartCoroutine(Position());
    }

    private void FixedUpdate()
    {
        isEndFever = CardDeck.Instance.hungerGauge.isFeverEnd;
        isCombatStart = GameManager.Instance.isCombatStart;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isCombatStart && !isEndFever)
        {
            canvasGroup.alpha = 0.4f;
            isDrag = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            transform.position += (Vector3)eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log(this.transform.position.y + "현재값");
        //Debug.Log(defaultPos.y + 200f + "목표값");
        //Debug.Log(CombatController.instance.playerCharacters[0].CombatAI.name);
        if (transform.position.y >= defaultPos.y + 150f / canvas.scaleFactor && !isEndFever)
        {
            CardEffect_();
        }
        transform.position = defaultPos;
        canvasGroup.alpha = 1f;
        isDrag = false ;
    }

    IEnumerator Position()
    {
        yield return new WaitForSeconds(0.05f);
        defaultPos = transform.position;
        isSetPosition = true;
    }

    public void CardEffect_()
    {
        var characterList = CombatController.instance.playerCharacters;
        var enemyList = CombatController.instance.enemyCharacters;

        int randomCharacterIdx = Random.Range(0, characterList.Count);
        int randomEnemyIdx = Random.Range(0,enemyList.Count);

        switch (cardInfo.recipeInfo.recipeInfo.recipeType)
        {
            case Recipes.HpPotion:
                CardEffect.Buff_TeamHeal(characterList , 5f);
                break;
            case Recipes.MpPotion:
                CardEffect.Buff_TeamMana(characterList , 5f);
                break;
            case Recipes.Egg:
                CardEffect.Buff_SoloHeal(characterList[randomCharacterIdx] , 10f);
                break;
            case Recipes.Sausage:
                CardEffect.Buff_AttackUp(characterList, cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.Ham:
                CardEffect.Buff_TeamShield(characterList , cardInfo.recipeInfo.recipeCurLevel * 100f);
                break;
            case Recipes.SardineCan:
                CardEffect.DeBuff_EnemyAttackSpeedDown(enemyList , cardInfo.recipeInfo.recipeCurLevel * 0.2f);
                break;
            case Recipes.Lasagne:
                CardEffect.Buff_CriticalChance(characterList , cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.PoisonPotion:
                CardEffect.DeBuff_EnemyDamage(enemyList , 20f , 0f);
                CardEffect.DeBuff_EnemyMoveSpeedDown(enemyList, 30f , cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.MushroomPizza:
                CardEffect.DeBuff_EnemyStun(enemyList , 2f , cardInfo.recipeInfo.recipeCurLevel * 0.5f);
                break;
            case Recipes.ChocolateCake:
                CardEffect.Buff_HpUp(characterList, cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.Ratatouille:
                CardEffect.Buff_DefUp(characterList, cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.SpringRoll:
                CardEffect.Buff_ResistanceUp(characterList, cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.Tiramisu:
                CardEffect.DeBuff_EnemyDamage(enemyList, 20f , cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.EnergyDrink:
                CardEffect.Buff_AttackSpeed(characterList, cardInfo.recipeInfo.recipeCurLevel * 0.1f);
                break;
            case Recipes.CookedChicken:
                CardEffect.Buff_CriticalDamage(characterList, cardInfo.recipeInfo.recipeCurLevel * 5f);
                break;
            case Recipes.Sandwich:
                StartCoroutine(SandWichEffect(3 + (cardInfo.recipeInfo.recipeCurLevel * 1) , characterList));
                break;
            case Recipes.Cocktail:
                StartCoroutine(CocktailEffect(cardInfo.recipeInfo.recipeCurLevel * 1, characterList));
                break;
            case Recipes.Paprika:
                CardEffect.Buff_Resurrect(characterList, ((int)cardInfo.recipeInfo.recipeCurLevel * 25));
                break;
            case Recipes.Candy:
                
                break;
            case Recipes.MusslePot:
                for (int i = 0; i<characterList.Count; i++) 
                {
                    CardEffect.Buff_LegendShield(characterList[i], characterList[i].MaxHP * cardInfo.recipeInfo.recipeCurLevel);
                }
                break;
            default:
                break;
        }
        CardEffect.ChargeHunger(CardDeck.Instance.hungerGauge , cardInfo.recipeInfo.recipeInfo.StomachCharging / 100);
        Destroy(gameObject);
        CardDeck.Instance.cards.Remove(gameObject);
        CardDeck.Instance.UseCard();
    }

    private IEnumerator SandWichEffect(float value, List<PlayerCombatAI> character)
    {
        for(int i = 0; i <value; i++)
        {
            CardEffect.Buff_TeamHeal(character, 5f);
            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator CocktailEffect(float value, List<PlayerCombatAI> character)
    {
        int randomCharacterIdx = Random.Range(0, character.Count);
        for (int i = 0; i < value; i++)
        {
            CardEffect.Buff_SoloMana(character[randomCharacterIdx]);
        }
        yield return null;
    }
}
