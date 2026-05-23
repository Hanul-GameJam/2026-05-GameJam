using System.Diagnostics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("추적 설정")]
    [SerializeField] private Transform target; // 이제 인스펙터에서 비워두어도 자동으로 찾습니다.
    [SerializeField] private float smoothTime = 0.2f; // 따라가는 속도 (낮을수록 빠르게 쫓아감)

    [Header("위치 보정")]
    [Tooltip("카메라의 기본 위치 (Z값은 반드시 -10 등 음수여야 카메라가 화면을 비춥니다)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("카메라 이동 제한 (선택사항)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minY = -5f; // 카메라가 이보다 더 아래로 내려가지 않음

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // 만약 인스펙터에서 Target을 직접 지정하지 않았다면 자동으로 찾음
        if (target == null)
        {
            FindPlayerTarget();
        }
    }

    void LateUpdate()
    {
        // 만약 플레이어를 아직도 못 찾았다면 다시 한 번 탐색 시도 (방어 코드)
        if (target == null)
        {
            FindPlayerTarget();
            return;
        }

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

    private void FindPlayerTarget()
    {
        // 1단계: 이름이 딱 "Player"인 오브젝트 찾기
        GameObject playerObj = GameObject.Find("Player");

        // 2단계: 만약 이름으로 못 찾았다면 "Player" 태그(Tag)로 찾기 (보험)
        if (playerObj == null)
        {
            playerObj = GameObject.FindWithTag("Player");
        }

        // 찾았다면 Transform 컴포넌트 연결
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            UnityEngine.Debug.LogWarning("카메라가 'Player' 이름이나 태그를 가진 오브젝트를 씬에서 찾을 수 없습니다! 플레이어 오브젝트 이름을 확인해 주세요.");
        }
    }
}