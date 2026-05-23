using System;
using UnityEngine;

public class EnemyHeadCollision : MonoBehaviour
{

    EnemyMovement enemyMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("destroy enemy");
            enemyMovement.DestroyEnemy();
            
        }
    }

    

}
