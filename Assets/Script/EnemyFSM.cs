
using System.Collections;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, Knockback };
    private State currentState;

    public Rigidbody2D rb;
    public Transform player, detectionPoint;
    public Animator anim;
    public float playerDetectionRange = 5f;
    public LayerMask playerLayer;
    public float speed = 2f;
    public float attackRange = 2;
    private float attackTimer = 0;
    public float attackCooldown = 2f;
    public bool isKnockedBack = false;

    [Header("Attack Animations")]
    public string attack_up = "attack_up";
    
    // Add reference to EnemeyCombat component
    private EnemeyCombat enemyCombat;

    void Start()
    {
        currentState = State.Idle;
        if (anim != null)
        {
            anim.SetBool("isIdle", true);
        }

        // Get EnemeyCombat component
        enemyCombat = GetComponent<EnemeyCombat>();
        if (enemyCombat == null)
        {
            Debug.LogWarning("EnemeyCombat component not found on " + gameObject.name);
        }

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Player found and assigned automatically");
            }
           
        }
    }

    void Update()
    {
        if (currentState != State.Knockback)
        {
            CheckForPlayer();
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            // Execute behavior based on current state
            switch (currentState)
            {
                case State.Idle:
                    // Do nothing, just idle
                    rb.linearVelocity = Vector2.zero;
                    break;
                case State.Chase:
                    ChasePlayer();
                    break;
                case State.Attack:
                    rb.linearVelocity = Vector2.zero;
                    if (Vector2.Distance(transform.position, player.position) > attackRange)
                    {
                        ChangeState(State.Chase);
                    }
                    break;
            }
        }
    }

    void ChasePlayer()
    {
        if (player == null || rb == null) return;

        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move towards player
        rb.linearVelocity = direction * speed;

        // Face player direction (for top-down)
        FacePlayerDirection();
    }

    private void CheckForPlayer()
    {
        if (detectionPoint == null)
        {
            Debug.LogError("DetectionPoint is not assigned!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            Debug.Log("Player detected! Distance: " + distanceToPlayer + ", Current State: " + currentState);

            // Jangan ubah state jika sedang dalam Attack dan masih cooldown
            if (currentState == State.Attack && attackTimer > 0)
            {
                return; // Tetap dalam state Attack
            }

            // Cek attack range
            if (distanceToPlayer <= attackRange && attackTimer <= 0)
            {
                attackTimer = attackCooldown;
                ChangeState(State.Attack);
            }
            // Jika di luar attack range, chase
            else if (distanceToPlayer > attackRange && currentState != State.Attack)
            {
                ChangeState(State.Chase);
            }
            // Jika dalam attack range tapi masih cooldown dan BUKAN sedang attack
            else if (distanceToPlayer <= attackRange && attackTimer > 0 && currentState != State.Attack)
            {
                rb.linearVelocity = Vector2.zero;
                ChangeState(State.Idle);
            }
        }
        else
        {
            ChangeState(State.Idle);
        }
    }

    public void ChangeState(State newState)
    {
        Debug.Log("Changing state from " + currentState + " to " + newState);

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
            if (anim != null) anim.SetBool(attack_up, false);
            
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


            Debug.Log("Triggering directional attack");
            TriggerDirectionalAttack();

        }
    }

    void FacePlayerDirection()
    {
        if (player == null) return;

        // Calculate direction to player for top-down movement
        Vector2 direction = (player.position - transform.position).normalized;

        // Optional: Rotate enemy to face player (uncomment if you want rotation)
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        // Or keep the flip system for horizontal movement only
        Vector3 scale = transform.localScale;
        scale.x = player.position.x > transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

   void TriggerDirectionalAttack()
{
    if (player == null || anim == null) return;

    Vector2 directionToPlayer = (player.position - transform.position).normalized;

    // Reset semua terlebih dulu (hindari tumpang tindih)
    
    anim.SetBool("isAttacking", false);

    if (Mathf.Abs(directionToPlayer.y) > Mathf.Abs(directionToPlayer.x))
    {
        if (directionToPlayer.y > 0)
        {
            anim.SetBool(attack_up, true);
            Debug.Log("Enemy attacking UP");
        }
        else
        {
            
        }
    }
    else
    {
        if (directionToPlayer.x > 0)
        {
            anim.SetBool("isAttacking", true); // kanan
            Debug.Log("Enemy attacking RIGHT");
        }
        else 
        {
            anim.SetBool("isAttacking", true); // kiri
            Debug.Log("Enemy attacking LEFT");
        }
    }

    // Panggil reset agar tidak stuck
    StartCoroutine(ResetAttackBools(0.5f)); // Sesuaikan durasi dengan panjang animasi
}




    // Untuk debugging - bisa dihapus nanti
    void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    IEnumerator ResetAttackBools(float delay)
    {
        yield return new WaitForSeconds(delay); // tunggu animasi selesai

        anim.SetBool("isAttacking", false);
    }
public void Enemyknockback(Transform player, float force, float duration)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - player.position).normalized;
        rb.linearVelocity = (direction * force);
       
    }
}
