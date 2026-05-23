using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("추적 설정")]
    [SerializeField] private Transform target; // 따라갈 대상 (칼립소)
    [SerializeField] private float smoothTime = 0.2f; // 따라가는 속도 (낮을수록 빠르게 쫓아감)

    [Header("위치 보정")]
    [Tooltip("카메라의 기본 위치 (Z값은 반드시 -10 등 음수여야 카메라가 화면을 비춥니다)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("카메라 이동 제한 (선택사항)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minY = -5f; // 카메라가 이보다 더 아래로 내려가지 않음 (낙사 구간 등에서 유용)

    private Vector3 velocity = Vector3.zero;

    // 플레이어의 Update나 FixedUpdate 이동이 모두 끝난 후 카메라가 이동해야 덜덜거림(Jittering)이 없습니다.
    void LateUpdate()
    {
        if (target == null) return;

        // 1. 카메라가 이동해야 할 최종 목표 위치 계산
        Vector3 targetPosition = target.position + offset;

        // 2. Y축 하단 제한 (맵 밖이나 땅 밑을 보여주지 않게 함)
        if (useBounds && targetPosition.y < minY)
        {
            targetPosition.y = minY;
        }

        // 3. SmoothDamp를 사용해 현재 위치에서 목표 위치로 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}