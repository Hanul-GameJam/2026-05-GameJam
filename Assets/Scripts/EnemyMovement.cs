using System.Collections;
using System.Diagnostics;
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
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip deathSound;


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
        ContactPoint2D contact = collision.GetContact(0);

        if (!(contact.normal.y < -0.5f) && collision.gameObject.CompareTag("Player"))
        {            // 1. 충돌한 플레이어 오브젝트에서 getDamage 함수가 있는 스크립트를 가져옵니다.
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();

            // 2. 스크립트가 정상적으로 존재한다면 함수를 호출합니다.
            if (pc != null)
            {
                pc.getDamage(1, transform);
            }
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
        UnityEngine.Debug.Log("waiting");
    }

    public void DestroyEnemy()
    {
        // 1. 전역 SFX_Player를 찾아서 소리만 재생시킵니다.
        GameObject sfxObj = GameObject.Find("SFX_Player");
        if (sfxObj != null)
        {
            AudioSource sfxSource = sfxObj.GetComponent<AudioSource>();
            if (sfxSource != null && deathSound != null)
            {
                sfxSource.PlayOneShot(deathSound);
            }
        }

        // 2. 적은 즉시 파괴해도 소리는 SFX_Player에서 재생되므로 끊기지 않습니다!
        Destroy(gameObject);
        UnityEngine.Debug.Log("enemy destroyed");
    }
}
