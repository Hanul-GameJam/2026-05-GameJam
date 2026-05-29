using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class BeaverBoss : MonoBehaviour
{
    public enum BossState { Chase, PrepareDash, Dashing, Resting }

    [Header("Current State")]
    public BossState currentState = BossState.Chase;
    public int health = 3;

    // 💡 비버 전용 추가 세팅
    [Header("Beaver Unique Attacks")]
    [SerializeField] private float aggroRange = 6f;         // 이 거리 안에 들어오면 빠르게 추격
    [SerializeField] private float fastChaseSpeed = 6f;     // 빠른 추격 속도
    [SerializeField] private float meleeAttackRange = 1.5f; // 근접 공격 사거리
    [SerializeField] private float attackCooldown = 1.2f;   // 공격 쿨다운 시간
    private float nextAttackTime = 0f;

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

    [Header("Animation & Audio")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip hurtSound;

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
                HandleChaseAndAttack(); // 💡 일반 추적 대신 거리 계산이 포함된 새로운 함수 사용
                break;
            case BossState.PrepareDash:
                animator.SetBool("isWalking", false);
                animator.SetBool("isDashing", true);
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

                    StartCoroutine(RestRoutine());
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isDashing", true);
                    rb.linearVelocity = new Vector2(facingDirection * dashSpeed, rb.linearVelocity.y);
                }
                break;
        }
    }

    // 💡 에디터에서 오브젝트를 선택했을 때 범위를 시각적으로 보여주는 기즈모 함수
    private void OnDrawGizmosSelected()
    {
        // 1. 근접 공격 사거리 (빨간색 원)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        // 2. 빠른 추격(어그로) 사거리 (노란색 원)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        // 3. (보너스) 낭떠러지 체크 레이저 (파란색 선)
        if (edgeCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * edgeCheckDistance);
        }
    }

    private void HandleChaseAndAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceToPlayerX = player.position.x - transform.position.x;

        // 플레이어 쪽으로 방향 뒤집기
        if (distanceToPlayerX > 0.5f) Flip(1);
        else if (distanceToPlayerX < -0.5f) Flip(-1);

        // 1. 근접 공격 사거리 내 (가장 우선)
        if (distanceToPlayer <= meleeAttackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);

            if (Time.time >= nextAttackTime)
            {
                animator.SetTrigger("Attack");
                nextAttackTime = Time.time + attackCooldown;
            }
            return; // 공격 중에는 멈춰야 하므로 여기서 함수 종료
        }

        // --- 여기서부터는 이동 로직 ---
        animator.SetBool("isWalking", true);
        float currentSpeed = chaseSpeed;

        // 2. 어그로 범위 내 (빠른 추격 속도 적용)
        if (distanceToPlayer <= aggroRange)
        {
            currentSpeed = fastChaseSpeed;
        }

        // 절벽 체크 후 이동
        if (IsGroundAhead())
        {
            rb.linearVelocity = new Vector2(facingDirection * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // 돌진 패턴 쿨다운 감소
        patternTimer -= Time.deltaTime;
        if (patternTimer <= 0f)
        {
            StartCoroutine(DashPatternRoutine());
        }
    }

    private void SpawnPlatform()
    {
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
            currentPlatform = null;
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
                DestroyPlatform();
                break;
            case BossState.PrepareDash:
                spriteRenderer.color = Color.red;
                SpawnPlatform();
                break;
            case BossState.Dashing:
                spriteRenderer.color = Color.red;
                break;
            case BossState.Resting:
                spriteRenderer.color = Color.green;
                DestroyPlatform();
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
                    //playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);

                    float bounceDir = (collision.transform.position.x < transform.position.x) ? -1f : 1f;

                    playerRb.linearVelocity = new Vector2(bounceDir * 30, 30);
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

        if (hurtSound != null)
        {
            GameObject sfxObj = GameObject.Find("SFX_Player");
            if (sfxObj != null)
            {
                AudioSource sfxSource = sfxObj.GetComponent<AudioSource>();
                if (sfxSource != null)
                {
                    sfxSource.PlayOneShot(hurtSound);
                }
            }
        }

        if (health <= 0)
        {
            UnityEngine.Debug.Log("비버 보스 처치 완료!");
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);
            DestroyPlatform();
            Destroy(gameObject);
        }
    }
}