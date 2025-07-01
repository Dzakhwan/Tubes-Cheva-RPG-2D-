using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public int Speed;
    private int Direction = 1;
    public Transform Player;
    public Rigidbody2D rb;
    public bool IsChasing;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsChasing == true)
        {
            Vector2 direction = (Player.position - transform.position).normalized;
            rb.linearVelocity = direction.normalized * Speed * Direction;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            IsChasing = true;
        }

    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.linearVelocity = Vector2.zero;
            IsChasing = false;
        }
    }
}
