using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AgentMovement : MonoBehaviour
{
    private Vector3 target;
    private Vector3 startPosition;

    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private TargetPositionManager targetManager;
    private RecipeDataManager recipeManager;

    public bool isReachedTarget = false;
    public bool isReturnStarted = false;
    public bool isOrder = false;
    public bool isAutoPay = false;

    public GameObject emojiPrefab;
    public Image btnImage;
    public Sprite orderImage;
    public Sprite reactionImage;

    private int recipePrice;

    public int TargetIndex { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        startPosition = new Vector3(0.7f, -7f, 0);
        agent.transform.position = startPosition;
        targetManager = TargetPositionManager.Instance;
        recipeManager = RecipeDataManager.Instance;
    }

    private void Update()
    {
        if (!isReachedTarget)
        {
            CheckIfReachedTarget();
            UpdateSpriteFlip();
        }
    }

    private void CheckIfReachedTarget()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                isReachedTarget = true;
                isOrder = false;
                OrderingFood();
            }
        }
    }

    private void OrderingFood()
    {
        emojiPrefab.SetActive(true);
        animator.SetBool("Move", false);
        StartCoroutine(ReturnToStartPositionAfterDelay());
    }

    private IEnumerator ReturnToStartPositionAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        if (!isOrder)
        {
            emojiPrefab.SetActive(false);
            target = startPosition;
            isReturnStarted = true;
            SetAgentPosition();
        }
    }

    private void UpdateSpriteFlip()
    {
        if (agent.velocity.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (agent.velocity.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void Initialize()
    {
        SoundManager.Instance.PlaySound("SFX_Door_Open");
        SoundManager.Instance.PlaySound("SFX_Ring_Bell");
        SetTargetPosition();
        SetAgentPosition();
    }

    private void SetTargetPosition()
    {
        (target, TargetIndex) = targetManager.GetTargetPositionWithIndex();
        if (TargetIndex == -1)
        {
            targetManager.ReturnObjectToPool(gameObject);
            return;
        }
    }

    private void SetAgentPosition()
    {
        agent.SetDestination(target);
        animator.SetBool("Move", true);
        isReachedTarget = false;
    }

    public void EmojiBtn()
    {
        if (!isOrder)
        {
            SoundManager.Instance.PlaySound("SFX_UI_Click");
            isOrder = true;
            emojiPrefab.SetActive(false);
            StartCoroutine(OfferFood());
        }
        else
        {
            isAutoPay = true;
            FloatingText();
            SoundManager.Instance.PlaySound("SFX_Coin");
            PayPrice();
            emojiPrefab.SetActive(false);
            target = startPosition;
            isReturnStarted = true;
            SetAgentPosition();
        }
    }

    private void FloatingText()
    {
        Color textColor = Color.yellow;
        string moneyText = $"+{recipePrice}";
        FloatingTextManager.Instance.ShowFloatingText(moneyText, transform.position, textColor);
    }

    private void PayPrice()
    {
        DataManager.Instance.userInfo.Gold += recipePrice;
    }

    IEnumerator OfferFood()
    {
        yield return new WaitForSeconds(2);
        RecipeData randomRecipe = RecipePercent();
        recipePrice = (int)randomRecipe.recipeInfo.sellPrice;
        UpdateFoodImage(randomRecipe);
        yield return new WaitForSeconds(2);
        targetManager.foodImage[TargetIndex].gameObject.SetActive(false);
        Reaction();
    }

    private RecipeData RecipePercent()
    {
        float randomIdx = Random.Range(0, 1f);
        if (randomIdx < 0.75)
        {
            int idx = Random.Range(0, recipeManager.recipesCommon.Count);
            return recipeManager.recipesCommon[idx];
        }
        else if (randomIdx < 0.99)
        {
            int idx = Random.Range(0, recipeManager.recipesMagic.Count);
            return recipeManager.recipesMagic[idx];
        }
        else
        {
            int idx = Random.Range(0, recipeManager.recipesLegend.Count);
            return recipeManager.recipesLegend[idx];
        }
    }

    private void UpdateFoodImage(RecipeData recipe)
    {
        targetManager.foodImage[TargetIndex].gameObject.SetActive(true);
        targetManager.foodImage[TargetIndex].sprite = recipe.recipeInfo.sprite;
    }

    private void Reaction()
    {
        emojiPrefab.SetActive(true);
        btnImage.sprite = reactionImage;
        btnImage.color = Color.black;
        StartCoroutine(AutoPay());
    }

    IEnumerator AutoPay()
    {
        yield return new WaitForSeconds(2f);
        if (!isAutoPay)
        {
            FloatingText();
            SoundManager.Instance.PlaySound("SFX_Coin");
            PayPrice();
            emojiPrefab.SetActive(false);
            target = startPosition;
            isReturnStarted = true;
            SetAgentPosition();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReturnStarted == true)
        {
            Disappear();
        }
    }

    private void Disappear()
    {
        btnImage.sprite = orderImage;
        isReachedTarget = false;
        isReturnStarted = false;
        isOrder = false;
        isAutoPay = false;

        TargetPositionManager.Instance.checkArrive[TargetIndex] = false;
        TargetPositionManager.Instance.ReturnObjectToPool(gameObject);
    }
}
