using UnityEngine;

public class BeaverMovement : EnemyMovement
{
    [Header("Beaver Settings")]
    [SerializeField] private float chaseRange = 10f; // 추적 범위
    [SerializeField] private float chaseSpeed = 7f; // 추적 속도
    [SerializeField] private float attackRange = 5f; // 공격 범위
    [SerializeField] private float attackCooldownTimer = 1.2f;

    private float nextAttackTime = 0f;
    private Transform player;

    protected override void Start()
    {
        // 부모 클래스의 Start를 실행하여 rb와 animator를 초기화합니다[cite: 1].
        base.Start();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected override void Update()
    {
        if (player == null)
        {
            base.Update();
            return;
        }

        // 1. 공격 애니메이션이 재생 중인지 확인
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsName("Bieber_Attack");

        // 공격 중이라면 이동 로직을 정지
        if (isAttacking)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 2. 거리 판정에 따른 행동
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer(); // 공격 범위 내
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer(); // 추적 범위 내
        }
        else
        {
            // 둘 다 아니면 부모(EnemyMovement)의 기본 순찰 로직(타이머 및 방향 전환)을 실행[cite: 1]
            base.Update();
        }
    }

    private void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack"); // 애니메이터의 Attack 파라미터 호출
            nextAttackTime = Time.time + attackCooldownTimer;
        }
    }

    private void ChasePlayer()
    {
        animator.SetBool("isWalking", true);

        Vector3 direction = (player.position - transform.position).normalized;

        // 부모 클래스에 public으로 열려있는 isEnemyFacingLeft 변수를 활용[cite: 1]
        isEnemyFacingLeft = direction.x < 0;

        // 부모 클래스와 동일한 방식으로 스케일 방향을 계산[cite: 1]
        float transformScaleX = Mathf.Sign(isEnemyFacingLeft ? -1 : 1) * Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(transformScaleX, transform.localScale.y, transform.localScale.z);

        transform.position += new Vector3(Mathf.Sign(direction.x), 0, 0) * chaseSpeed * Time.deltaTime;
    }

    // 애니메이션 이벤트에서 호출할 데미지 판정 함수
    public void PerformAttackDamage()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.getDamage(1, transform);
            }
        }
    }

    // 씬 뷰에서 범위 확인용 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}