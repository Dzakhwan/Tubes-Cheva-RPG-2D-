using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{

    public float stunTime = 1f;
    public bool isKnockedBack = false;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void knockback(Transform playerTransform, float knockbackForce)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = (direction * knockbackForce);
        
    }

    
}
