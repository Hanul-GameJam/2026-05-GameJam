using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class TutorialBoss : MonoBehaviour
{
    public enum BossState { Chase, PrepareDash, Dashing, Resting }

    [Header("Current State")]
    public BossState currentState = BossState.Chase;
    public int health = 3;

    [Header("Movement & Dash Settings")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float prepareTime = 1.5f;
    [SerializeField] private float restTime = 2.5f;
    [SerializeField] private float patternCooldown = 4f;

    [Header("Hurt Settings")]
    [SerializeField] private float hurtWaitTime = 0.5f;
    private bool isHurtThisRest = false;
    public string nextSceneName;

    [Header("Edge Detection")]
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private float edgeCheckDistance = 1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Platform Spawn Settings")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private float platformSpawnHeight = 3f;
    private GameObject currentPlatform;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float patternTimer;
    private int facingDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        //시작할 때 오브젝트의 실제 좌우 스케일에 맞춰 facingDirection 동기화
        facingDirection = transform.localScale.x > 0 ? 1 : -1;

        patternTimer = patternCooldown;
        SetBossState(BossState.Chase);
    }

    void Update()
    {
        if (player == null || health <= 0) return;

        switch (currentState)
        {
            case BossState.Chase:
                animator.SetBool("isWalking", true);
                animator.SetBool("isDashing", false);
                HandleChase();
                break;
            case BossState.PrepareDash:
                animator.SetBool("isWalking", false);
                animator.SetBool("isDashing", true);
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // 준비할 때는 멈춤
                break;
            case BossState.Resting:
                animator.SetBool("isWalking", false);
                animator.SetBool("isDashing", false);
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
            case BossState.Dashing:
                if (!IsGroundAhead())
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isDashing", false);
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // 즉시 브레이크
                    
                    StartCoroutine(RestRoutine());
                }
                else
                {
                    // 바닥이 있을 때만 전진
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isDashing", true);
                    rb.linearVelocity = new Vector2(facingDirection * dashSpeed, rb.linearVelocity.y);
                }
                break;
        }
    }

    private void SpawnPlatform()
    {
        // 프리팹이 등록되어 있고, 현재 만들어진 발판이 없을 때만 생성
        if (platformPrefab != null && currentPlatform == null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, platformSpawnHeight, 0);
            currentPlatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void DestroyPlatform()
    {
        if (currentPlatform != null)
        {
            Destroy(currentPlatform);
            currentPlatform = null; // 지운 후에는 변수를 다시 비워줍니다.
        }
    }

    private void HandleChase()
    {
        float distanceToPlayerX = player.position.x - transform.position.x;

        if (distanceToPlayerX > 0.5f) Flip(1);
        else if (distanceToPlayerX < -0.5f) Flip(-1);

        if (IsGroundAhead())
        {
            rb.linearVelocity = new Vector2(facingDirection * chaseSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        patternTimer -= Time.deltaTime;
        if (patternTimer <= 0f)
        {
            StartCoroutine(DashPatternRoutine());
        }
    }

    private bool IsGroundAhead()
    {
        if (edgeCheck == null) return false;
        RaycastHit2D hit = Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void Flip(int direction)
    {
        if (facingDirection != direction)
        {
            facingDirection = direction;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
    }

    private void SetBossState(BossState newState)
    {
        currentState = newState;


        switch (currentState)
        {
            case BossState.Chase:
                spriteRenderer.color = Color.white;
                DestroyPlatform(); // 💡 [추가됨] 확실한 처리를 위해 추적 상태에서도 삭제
                break;
            case BossState.PrepareDash:
                spriteRenderer.color = Color.red;
                SpawnPlatform();   // 💡 [추가됨] 돌진 준비를 할 때 발판 생성!
                break;
            case BossState.Dashing:
                spriteRenderer.color = Color.red;
                break;
            case BossState.Resting:
                spriteRenderer.color = Color.green;
                DestroyPlatform(); // 💡 [추가됨] 돌진이 끝나고 휴식 상태일 때 발판 삭제!
                break;
        }
    }

    private IEnumerator DashPatternRoutine()
    {
        SetBossState(BossState.PrepareDash);
        yield return new WaitForSeconds(prepareTime);

        SetBossState(BossState.Dashing);
        yield return new WaitForSeconds(0.5f);

        if (currentState == BossState.Dashing)
        {
            StartCoroutine(RestRoutine());
        }
    }

    private IEnumerator RestRoutine()
    {
        isHurtThisRest = false;
        SetBossState(BossState.Resting);

        yield return new WaitForSeconds(restTime);

        patternTimer = patternCooldown;
        SetBossState(BossState.Chase);
    }

    private IEnumerator HurtRecoveryRoutine()
    {
        yield return new WaitForSeconds(hurtWaitTime);

        patternTimer = patternCooldown;
        SetBossState(BossState.Chase);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.GetContact(0);

            if (contact.normal.y < -0.5f && currentState == BossState.Resting && !isHurtThisRest)
            {
                isHurtThisRest = true;
                TakeDamage();

                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);
                }

                if (health > 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(HurtRecoveryRoutine());
                }
            }
            else if (currentState != BossState.Resting)
            {
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

                if (pc != null)
                {
                    pc.getDamage(1, transform);
                }
            }
        }
    }

    private void TakeDamage()
    {
        if (health <= 0) return;

        health--;
        UnityEngine.Debug.Log("보스 데미지 입음! 남은 체력: " + health);

        if (health <= 0)
        {
            UnityEngine.Debug.Log("튜토리얼 보스 처치 완료! (은퇴 축하 컷씬 재생)");
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);
            DestroyPlatform();
            Destroy(gameObject);
        }
    }
}