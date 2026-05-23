using System.Collections;
using UnityEngine;

public class PillarCollision : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float knockbackPowerX = 15f; // 뒤로 밀려나는 힘
    [SerializeField] private float knockbackPowerY = 15f; // 위로 튀어오르는 힘

    PlayerController pc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (rb != null)
            {
            // 내 위치와 공격자의 위치를 비교해 튕겨나갈 방향(왼쪽 -1, 오른쪽 1)을 계산합니다.
            int knockbackDir = transform.position.x < collision.transform.position.x ? 1 : -1;

            // 💡 1. 넉백이 덮어씌워지는 것을 막기 위해 조작 스크립트를 잠시 끕니다.
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            // 2. 넉백 속도 적용
            collision.rigidbody.linearVelocity = new Vector2(knockbackDir * knockbackPowerX, knockbackPowerY);

            // 💡 3. 0.3초(원하는 넉백 지속 시간) 뒤에 다시 조작 가능하도록 함수를 예약 실행합니다.
            StartCoroutine(restoreCtrl(controller));
            }
        }
    }

    private IEnumerator restoreCtrl(PlayerController controller)
    {
        yield return new WaitForSeconds(0.3f);
        controller.RestoreControl();
    } 
}
