using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    [SerializeField] GameObject footStepDust;
    [SerializeField] Transform effectPos;
    private Vector2 targetPosition;
    public float speed = 1;
    private bool movingToCenter = true;
    private bool isPaused = false;
    private float pauseTime = 4f;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private float footStepTimer = 0f;
    private float footStepInterval = 0.3f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isPaused) return;

        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 newPos = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            footStepTimer += Time.deltaTime;
            if(footStepTimer >= footStepInterval)
            {
                MakeFootStepDust();
                footStepTimer = 0f;
            }
           
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

            if (targetPosition.x > transform.position.x)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
        }
        else
        {
            if (movingToCenter)
            {
                StartCoroutine(PauseBeforeMovingAgain());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }


    public void Initialize(Vector2 startPos, Vector2 targetPos)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        startPos = new Vector2(startPos.x, Random.Range(-3.5f, -2f));
        transform.position = startPos;

        targetPosition = targetPos;

        movingToCenter = true;
        isPaused = false;

        spriteRenderer.flipX = startPos.x > 0 ? false : true;
        spriteRenderer.enabled = true;
    }

    private IEnumerator PauseBeforeMovingAgain()
    {
        isPaused = true;
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(pauseTime);
        spriteRenderer.enabled = true;
        float randomY = Random.Range(-3.5f, -2f);
        targetPosition = new Vector2(Random.Range(0, 2) == 0 ? 9.5f : -9.5f, randomY);
        movingToCenter = false;
        isPaused = false;
    }

    private void MakeFootStepDust()
    {
        if(footStepDust != null)
        {
            EffectManager.Instance.PlayAnimatedSpriteEffect(footStepDust, effectPos.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        SpawnCharacter other = collider.GetComponent<SpawnCharacter>();
        if (other != null)
        {
            if (transform.position.y < other.transform.position.y)
            {
                spriteRenderer.sortingOrder = Mathf.Clamp(other.spriteRenderer.sortingOrder + 1, 0, 50);
            }
            else
            {
                other.spriteRenderer.sortingOrder = Mathf.Clamp(spriteRenderer.sortingOrder + 1, 0, 50);
            }
        }
    }
}
