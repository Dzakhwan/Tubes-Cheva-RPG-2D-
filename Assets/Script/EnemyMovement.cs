using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.2f;
    public int maxHP = 3;
    private int currentHP;

    public Transform player;
    public Transform pointA;
    public Transform pointB;

    private Vector2 currentTarget;

    private Rigidbody2D rb;

    private enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    private State currentState = State.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        currentTarget = pointB.position;
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                break;

            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                Attack();
                break;
        }

        // Deteksi jarak player
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < attackRange && currentState != State.Dead)
        {
            currentState = State.Attack;
        }
        else if (distance < 5f && currentState != State.Attack && currentState != State.Dead)
        {
            currentState = State.Chase;
        }
        else if (distance >= 5f && currentState != State.Dead)
        {
            currentState = State.Patrol;
        }
    }

    void Patrol()
    {
        Vector2 dir = ((Vector2)currentTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            currentTarget = currentTarget == (Vector2)pointA.position ? pointB.position : pointA.position;
        }
    }

    void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Musuh menyerang!");

        // Di sinilah kamu bisa pasang animasi atau damage ke player

        // Contoh delay balik ke chase
        Invoke(nameof(BackToChase), 1.5f);
    }

    void BackToChase()
    {
        currentState = State.Chase;
    }

    public void TakeDamage(int damage)
    {
        if (currentState == State.Dead) return;

        currentHP -= damage;
        Debug.Log("Enemy took damage. HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy mati");
        currentState = State.Dead;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        // Tambahkan animasi kematian di sini kalau ada
        // Destroy(gameObject, 2f);
    }
}
