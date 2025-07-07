using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    private enum State { Idle, Chase, Attack }
    private State currentState;

    public Rigidbody2D rb;
    public Transform player;
    public float speed = 2f;
    public float attackRange = 2;
    public Animator anim;

    void Start()
    {
        currentState = State.Idle;
        if (anim != null)
        {
            anim.SetBool("isIdle", true);
        }
    }

    void Update()
    {
        // Execute behavior based on current state
        switch (currentState)
        {
            case State.Idle:
                // Do nothing, just idle
                break;
            case State.Chase:
                ChasePlayer();
                break;
            case State.Attack:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void ChasePlayer()
    {
        if (player == null || rb == null) return;
        if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
        {
            ChangeState(State.Attack);

        }
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move towards player
        rb.linearVelocity = direction * speed;

        // Flip sprite to face player
        FlipToPlayer();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player == null) player = collision.transform;
            ChangeState(State.Chase);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(State.Idle);
        }
    }

    void ChangeState(State newState)
    {
        // Exit current state
        if (currentState == State.Idle)
        {
            if (anim != null) anim.SetBool("isIdle", false);
        }
        else if (currentState == State.Chase)
        {
            if (anim != null) anim.SetBool("isWalking", false);
        }
        else if (currentState == State.Attack)
        {
            if (anim != null) anim.SetBool("isAttacking", false);
        }

        // Change to new state
        currentState = newState;

        // Enter new state
        if (currentState == State.Idle)
        {
            if (anim != null) anim.SetBool("isIdle", true);
        }
        else if (currentState == State.Chase)
        {
            if (anim != null) anim.SetBool("isWalking", true);
        }
        else if (currentState == State.Attack)
        {
            if (anim != null) anim.SetBool("isAttacking", true);
        }
    }

    void FlipToPlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        scale.x = player.position.x > transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
    void Attack()
    {
        Debug.Log("Attack");
    }
}
