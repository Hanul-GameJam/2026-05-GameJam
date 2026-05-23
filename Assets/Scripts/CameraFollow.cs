using System.Diagnostics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Tracking Settings")]
    [SerializeField] private Transform target; // ���� �ν����Ϳ��� ����ξ �ڵ����� ã���ϴ�.
    [SerializeField] private float smoothTime = 0.2f; // ���󰡴� �ӵ� (�������� ������ �Ѿư�)

    [Header("Position Offset")]
    [Tooltip("ī�޶��� �⺻ ��ġ (Z���� �ݵ�� -10 �� �������� ī�޶� ȭ���� ����ϴ�)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Camera Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minY, maxY; // ī�޶� �̺��� �� �Ʒ��� �������� ����
    [SerializeField] private float minX, maxX;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // ���� �ν����Ϳ��� Target�� ���� �������� �ʾҴٸ� �ڵ����� ã��
        if (target == null)
        {
            FindPlayerTarget();
        }
    }

    void LateUpdate()
    {
        // ���� �÷��̾ ������ �� ã�Ҵٸ� �ٽ� �� �� Ž�� �õ� (��� �ڵ�)
        if (target == null)
        {
            FindPlayerTarget();
            return;
        }

        // 1. ī�޶� �̵��ؾ� �� ���� ��ǥ ��ġ ���
        Vector3 targetPosition = target.position + offset;

        // 2. Y�� �ϴ� ���� (�� ���̳� �� ���� �������� �ʰ� ��)
        if (useBounds && targetPosition.y < minY)
        {
            targetPosition.y = minY;
        }

        if (useBounds && targetPosition.y > maxY)
        {
            targetPosition.y = maxY;
        }

        if (useBounds && targetPosition.x < minX)
        {
            targetPosition.x = minX;
        }

        if (useBounds && targetPosition.x > maxX)
        {
            targetPosition.x = maxX;
        }

        // 3. SmoothDamp�� ����� ���� ��ġ���� ��ǥ ��ġ�� �ε巴�� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void FindPlayerTarget()
    {
        // 1�ܰ�: �̸��� �� "Player"�� ������Ʈ ã��
        GameObject playerObj = GameObject.Find("Player");

        // 2�ܰ�: ���� �̸����� �� ã�Ҵٸ� "Player" �±�(Tag)�� ã�� (����)
        if (playerObj == null)
        {
            playerObj = GameObject.FindWithTag("Player");
        }

        // ã�Ҵٸ� Transform ������Ʈ ����
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ī�޶� 'Player' �̸��̳� �±׸� ���� ������Ʈ�� ������ ã�� �� �����ϴ�! �÷��̾� ������Ʈ �̸��� Ȯ���� �ּ���.");
        }
    }
}