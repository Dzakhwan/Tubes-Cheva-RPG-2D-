using System.Collections;
using Unity.Collections;
using UnityEngine;
using Pathfinding;

public class WizardFSM : MonoBehaviour
{
    public enum State { Idle, Chase, Attack, Death, Knockback };
    private State currentState;

    [Header("References")]
    public Rigidbody2D rb;
    public Transform player, detectionPoint;
    public Animator anim;
    
    [Header("Detection & Movement")]
    public float playerDetectionRange = 8f;
    public LayerMask playerLayer;
    public float speed = 1.5f;
    
    [Header("Attack Settings")]
    public float attackRange = 3f;
    private float attackTimer = 0;
    public float attackCooldown = 3f;
    public bool isKnockedBack = false;
    
    [Header("Attack Duration")]
    public float attackDuration = 1.5f;
    
    [Header("Pathfinding")]
    public AIDestinationSetter aiDestinationSetter;
    public AIPath aiPath;
    private bool usePathfinding = true;
    private bool pathfindingInitialized = false;
    
    // References
    private BossHealth bossHealth;
    private bool isDead = false;

    void Start()
    {
        currentState = State.Idle;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        bossHealth = GetComponent<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogWarning("BossHealth component not found on " + gameObject.name);
        }

        aiPath = GetComponent<AIPath>();
        if (aiPath != null)
        {
            aiPath.enabled = false;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Player found and assigned automatically for Wizard");
            }
        }

        StartCoroutine(InitializePathfinding());
    }

    void Update()
    {
        // Jangan lakukan apapun jika sudah mati
        if (isDead || currentState == State.Death) return;
        
        if (currentState != State.Knockback)
        {
            CheckForPlayer();
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            
            switch (currentState)
            {
                case State.Idle:
                    rb.linearVelocity = Vector2.zero;
                    break;
                case State.Chase:
                    ChasePlayer();
                    break;
                case State.Attack:
                    rb.linearVelocity = Vector2.zero;
                    if (Vector2.Distance(transform.position, player.position) > attackRange * 1.5f)
                    {
                        ChangeState(State.Chase);
                    }
                    break;
            }
        }
        
        if (transform.parent != null)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    void ChasePlayer()
    {
        if (player == null || rb == null) return;

        if (usePathfinding && pathfindingInitialized && aiPath != null && aiPath.enabled)
        {
            return;
        }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }

        FacePlayerDirection();
    }

    private void CheckForPlayer()
    {
        if (detectionPoint == null || isDead)
        {
            if (detectionPoint == null)
                Debug.LogError("DetectionPoint is not assigned!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
            if (usePathfinding && pathfindingInitialized)
            {
                if (aiDestinationSetter != null)
                {
                    aiDestinationSetter.enabled = true;
                    aiDestinationSetter.target = player;
                }
                if (aiPath != null)
                {
                    aiPath.enabled = true;
                }
            }

            Debug.Log("Wizard detected player! Distance: " + distanceToPlayer + ", Current State: " + currentState);

            if (currentState == State.Attack && attackTimer > 0)
            {
                return;
            }

            if (distanceToPlayer <= attackRange && attackTimer <= 0)
            {
                attackTimer = attackCooldown;
                ChangeState(State.Attack);
            }
            else if (distanceToPlayer > attackRange && currentState != State.Attack)
            {
                ChangeState(State.Chase);
            }
            else if (distanceToPlayer <= attackRange && attackTimer > 0 && currentState != State.Attack)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            if (usePathfinding && pathfindingInitialized)
            {
                if (aiDestinationSetter != null)
                {
                    aiDestinationSetter.enabled = false;
                }
                if (aiPath != null)
                {
                    aiPath.enabled = false;
                }
            }
            
            if (currentState == State.Chase)
            {
                ChangeState(State.Idle);
            }
        }
    }

    public void ChangeState(State newState)
    {
        // Jangan bisa keluar dari death state
        if (isDead && currentState == State.Death) return;
        
        Debug.Log("Wizard changing state from " + currentState + " to " + newState);

        ExitCurrentState();
        currentState = newState;
        EnterNewState();
    }

    private void ExitCurrentState()
    {
        if (anim == null) return;

        switch (currentState)
        {
            case State.Idle:
                anim.SetBool("isIdle", false);
                break;
            case State.Chase:
                anim.SetBool("isWalking", false);
                break;
            case State.Attack:
                anim.SetBool("isAttack", false);
                break;
        }
    }

    private void EnterNewState()
    {
        if (anim == null) return;
        
        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isIdle", true);
                break;
            case State.Chase:
                anim.SetBool("isWalking", true);
                break;
            case State.Attack:
                anim.SetBool("isAttack", true);
                StartCoroutine(ResetAttackAfterDuration(attackDuration));
                Debug.Log("Wizard performing Attack");
                break;
            case State.Death:
                // Set death animation trigger
                anim.SetTrigger("isDeath");
                isDead = true;
                rb.linearVelocity = Vector2.zero;
                
                // Disable collider agar tidak bisa diserang lagi
                Collider2D col = GetComponent<Collider2D>();
                if (col != null)
                {
                    col.enabled = false;
                }
                
                // Disable pathfinding
                if (pathfindingInitialized)
                {
                    if (aiDestinationSetter != null)
                        aiDestinationSetter.enabled = false;
                    if (aiPath != null)
                        aiPath.enabled = false;
                }
                
                Debug.Log("Wizard entered death state - animation should play");
                break;
        }
    }

    void FacePlayerDirection()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        scale.x = player.position.x > transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private IEnumerator ResetAttackAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        if (currentState == State.Attack)
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= attackRange)
                {
                    rb.linearVelocity = Vector2.zero;
                    ChangeState(State.Idle);
                }
                else
                {
                    ChangeState(State.Chase);
                }
            }
            else
            {
                ChangeState(State.Idle);
            }
        }
    }

    public void OnTakeDamage()
    {
        if (!isDead && currentState != State.Death && anim != null)
        {
            anim.SetTrigger("isTakeHit");
            Debug.Log("Wizard take hit animation triggered");
        }
    }

    public void OnDeath()
    {
        ChangeState(State.Death);
    }

    public void BossKnockback(Transform attacker, float force, float duration)
    {
        if (isDead || currentState == State.Death) return;
        
        isKnockedBack = true;
        Vector2 direction = (transform.position - attacker.position).normalized;
        rb.linearVelocity = (direction * force);
        
        StartCoroutine(ResetKnockback(duration));
    }

    private IEnumerator ResetKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);
        isKnockedBack = false;
    }

    private IEnumerator InitializePathfinding()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (AstarPath.active != null && AstarPath.active.data != null && AstarPath.active.data.graphs != null)
        {
            var node = AstarPath.active.GetNearest(transform.position);
            if (node.node != null)
            {
                pathfindingInitialized = true;
                Debug.Log("Wizard pathfinding initialized successfully");
                
                if (aiPath != null)
                {
                    aiPath.maxSpeed = speed;
                }
            }
            else
            {
                Debug.LogWarning("Wizard: No valid pathfinding node found near position " + transform.position + ". Using direct movement.");
                usePathfinding = false;
            }
        }
        else
        {
            Debug.LogWarning("Wizard: Pathfinding graph not ready. Using direct movement.");
            usePathfinding = false;
        }
    }

    public void SnapToNearestNode()
    {
        if (AstarPath.active != null)
        {
            var nearest = AstarPath.active.GetNearest(transform.position);
            if (nearest.node != null)
            {
                transform.position = (Vector3)nearest.position;
                Debug.Log("Wizard snapped to nearest pathfinding node");
            }
        }
    }

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
}