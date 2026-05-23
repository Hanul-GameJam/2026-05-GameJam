using System;
using UnityEngine;

public class EnemyHeadCollision : MonoBehaviour
{

<<<<<<< Updated upstream
    EnemyMovement enemyMovement;
=======
    [SerializeField] EnemyMovement enemyMovement;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            Debug.Log("destroy enemy");
=======
            // Debug.Log("head contact with player");
>>>>>>> Stashed changes
            enemyMovement.DestroyEnemy();
            
        }
    }

    

}
