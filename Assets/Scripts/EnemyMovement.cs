using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{   

    private Rigidbody2D rb;

    [SerializeField] public float EnemySpeed = 5f;
    // [SerializeField] public GameObject.Transform.Position EnemyPosition;
    [SerializeField] public float turnInterval = 3f;  // 3 sec
    [SerializeField] public float turnWaitTime = 2.0f;
    private float timer = 0f;
    [SerializeField] public bool isEnemyFacingLeft = true;

    [Header("Animator")]
    [SerializeField] private bool isWalking;
    [SerializeField] private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {
        // UnityEngine.Vector3 move = new UnityEngine.Vector3(1, 0, 0);
        timer += Time.deltaTime;
        if (timer >= turnInterval)
        {
            Invoke("Wait", turnWaitTime);
            timer = 0.0f;
            animator.SetBool("isWalking", false);
            
            isEnemyFacingLeft = !isEnemyFacingLeft;
        }

        EnemyRoaming(isEnemyFacingLeft);

    
        //transform.Translate(move * EnemySpeed);
        //transform.position = ;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);

        if (!(contact.normal.y < -0.5f) && collision.gameObject.CompareTag("Player"))
        {            // 1. �浹�� �÷��̾� ������Ʈ���� getDamage �Լ��� �ִ� ��ũ��Ʈ�� �����ɴϴ�.
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

            // 2. ��ũ��Ʈ�� ���������� �����Ѵٸ� �Լ��� ȣ���մϴ�.
            if (pc != null)
            {
                pc.getDamage(1, transform);
            }
        }
    }

    private void EnemyRoaming(bool isEnemyFacingLeft)
    {
        animator.SetBool("isWalking", true);

        float transformScaleX = Mathf.Sign(isEnemyFacingLeft ? -1 : 1) * Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(transformScaleX, transform.localScale.y, transform.localScale.z);
        
        //rb.AddForce(UnityEngine.Vector2.right * EnemySpeed, ForceMode2D.Impulse);
        if (!isEnemyFacingLeft)
        {
            transform.position += UnityEngine.Vector3.right * EnemySpeed * Time.deltaTime;
        }
        else if (isEnemyFacingLeft)
        {
            transform.position += UnityEngine.Vector3.left * EnemySpeed * Time.deltaTime;
        }
    }

    private void Wait()
    {
        Debug.Log("waiting");
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
        Debug.Log("enemy destoryed");
    }
}
