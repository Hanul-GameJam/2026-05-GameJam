using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("Player Stats")]
    [SerializeField] public int Health = 3;

    [Header("Movement Settings")]
=======
    [Header("�÷��̾� ���°�")]
    [SerializeField] public int Health = 3;

    [Header("�̵� ����")]
>>>>>>> Stashed changes
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;
    public Transform topLeftBoundary, bottomRightBoundary;

<<<<<<< Updated upstream
    [Header("Control Assists (Coyote Time & Buffer)")]
=======
    [Header("���۰� ���� ���� (�ڿ��� Ÿ�� & ���Է� ����)")]
>>>>>>> Stashed changes
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

<<<<<<< Updated upstream
    [Header("Ground Detection")]
=======
    [Header("�ٴ� ���� ����")]
>>>>>>> Stashed changes
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

<<<<<<< Updated upstream
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackPowerX = 5f; // 뒤로 밀려나는 힘
    [SerializeField] private float knockbackPowerY = 5f; // 위로 튀어오르는 힘
=======
    [Header("Animator")]
    public bool isWalking;
    [SerializeField] private Animator animator;
>>>>>>> Stashed changes

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 clampedPosition = rb.position;

        clampedPosition.x = Mathf.Clamp(
            clampedPosition.x,
            topLeftBoundary.position.x,
            bottomRightBoundary.position.x
        );

        clampedPosition.y = Mathf.Clamp(
            clampedPosition.y,
            bottomRightBoundary.position.y,
            topLeftBoundary.position.y
        );

        rb.position = clampedPosition;

        if (rb.position.x <= topLeftBoundary.position.x && horizontalInput < 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (rb.position.y <= bottomRightBoundary.position.y && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }

        HandleInput();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        UpdateCoyoteTime();
        ApplyMovement();
        CheckAndApplyJump();
    }

    private void HandleInput()
    {
        horizontalInput = 0f;

<<<<<<< Updated upstream
        // 키보드가 연결되어 있지 않으면 예외 처리
        if (Keyboard.current == null) return;

        // 좌우 이동 (A, D 및 좌우 방향키)
=======
        // Ű���尡 ����Ǿ� ���� ������ ���� ó��
        if (Keyboard.current == null || animator == null)
        {
            Debug.LogWarning("Keyboard or Animator component is missing.");
            
            return;
        }

        if (horizontalInput == 0f) {
            animator.SetBool("isWalking", false);
        }

        // �¿� �̵� (A, D �� �¿� ����Ű)
>>>>>>> Stashed changes
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalInput = -1f;

            animator.SetBool("isWalking", true);
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalInput = 1f;

<<<<<<< Updated upstream
        // 점프 선입력 버퍼 (Jump Buffer) 갱신
=======
            animator.SetBool("isWalking", true);
        }

        // ���� ���Է� ���� (Jump Buffer) ����
>>>>>>> Stashed changes
        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void UpdateCoyoteTime()
    {
<<<<<<< Updated upstream
        // 코요테 타임 (Coyote Time) 갱신
        // 바닥에 닿아있으면 타이머 초기화, 공중이면 시간에 따라 감소
=======
        // �ڿ��� Ÿ�� (Coyote Time) ����
        // �ٴڿ� ��������� Ÿ�̸� �ʱ�ȭ, �����̸� �ð��� ���� ����
>>>>>>> Stashed changes
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void ApplyMovement()
    {
<<<<<<< Updated upstream
        // Unity 6.x 기준 최신 API인 linearVelocity 사용
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // 캐릭터 방향 전환 (좌우 반전)
=======
        // Unity 6.x ���� �ֽ� API�� linearVelocity ���
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // ĳ���� ���� ��ȯ (�¿� ����)
>>>>>>> Stashed changes
        if (horizontalInput != 0)
        {
            float transformScaleX = Mathf.Sign(horizontalInput) * Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(transformScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    private void CheckAndApplyJump()
    {
<<<<<<< Updated upstream
        // 코요테 타임(발판을 막 벗어남)과 선입력 버퍼(미리 누름)가 모두 유효할 때 점프 실행
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // y 속도 초기화로 일관된 점프력 보장
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // 점프 후 카운터 즉시 0으로 만들어 이중 점프 방지
=======
        // �ڿ��� Ÿ��(������ �� ���)�� ���Է� ����(�̸� ����)�� ��� ��ȿ�� �� ���� ����
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // y �ӵ� �ʱ�ȭ�� �ϰ��� ������ ����
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // ���� �� ī���� ��� 0���� ����� ���� ���� ����
>>>>>>> Stashed changes
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // value(데미지량)와 attacker(공격자의 위치)를 함께 받습니다.
    public void getDamage(int value, Transform attacker)
    {
        this.Health -= value;
        UnityEngine.Debug.Log("current_health = " + this.Health);

        if (rb != null && attacker != null)
        {
            // 내 위치와 공격자의 위치를 비교해 튕겨나갈 방향(왼쪽 -1, 오른쪽 1)을 계산합니다.
            int knockbackDir = transform.position.x < attacker.position.x ? -1 : 1;

            // 💡 1. 넉백이 덮어씌워지는 것을 막기 위해 조작 스크립트를 잠시 끕니다.
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            // 2. 넉백 속도 적용
            rb.linearVelocity = new Vector2(knockbackDir * knockbackPowerX, knockbackPowerY);

            // 💡 3. 0.3초(원하는 넉백 지속 시간) 뒤에 다시 조작 가능하도록 함수를 예약 실행합니다.
            Invoke(nameof(RestoreControl), 0.3f);
        }
    }

    // 잠시 꺼뒀던 조작 권한을 다시 켜주는 함수
    private void RestoreControl()
    {
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = true;
        }
    }
}
