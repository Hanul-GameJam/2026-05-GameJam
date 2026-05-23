using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveToNextLevel : MonoBehaviour
{
    private Rigidbody2D rb;
    public int nextLevelIndex;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<GameManager>().LoadNextLevel(nextLevelIndex);
        }
    }
}
