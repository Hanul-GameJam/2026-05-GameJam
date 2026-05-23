using System.Collections;
using System.Numerics;
using System.Runtime.Serialization;
using System.Threading;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
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
            
            isEnemyFacingLeft = !isEnemyFacingLeft;
        }

        EnemyRoaming(isEnemyFacingLeft);

    
        //transform.Translate(move * EnemySpeed);
        //transform.position = ;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("enemy contact with player tag");
        }
    }

    private void EnemyRoaming(bool isEnemyFacingLeft)
    {   
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
