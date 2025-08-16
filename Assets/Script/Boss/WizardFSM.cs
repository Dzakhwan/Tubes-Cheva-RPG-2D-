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
    public float playerDetectionRange = 8f; // Boss has larger detection range
    public LayerMask playerLayer;
    public float speed = 1.5f; // Boss moves slower but hits harder
    
    [Header("Attack Settings")]
    public float attackRange = 3f; // Boss has longer attack range
    private float attackTimer = 0;
    public float attackCooldown = 3f; // Longer cooldown between attacks
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

        // Get BossHealth component
        bossHealth = GetComponent<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogWarning("BossHealth component not found on " + gameObject.name);
        }

        // Get AIPath component
        aiPath = GetComponent<AIPath>();
        if (aiPath != null)
        {
            aiPath.enabled = false; // Start disabled
        }

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Player found and assigned automatically for Boss");
            }
        }

        // Initialize pathfinding after a short delay to ensure graph is ready
        StartCoroutine(InitializePathfinding());
    }

    void Update()
    {
        // Don't do anything if dead
        if (isDead) return;
        
        if (currentState != State.Knockback && currentState != State.Death)
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
                    rb.linearVelocity = Vector2.zero;
                    break;
                case State.Chase:
                    ChasePlayer();
                    break;
                case State.Attack:
                    rb.linearVelocity = Vector2.zero;
                    // Check if player moved out of range during attack
                    if (Vector2.Distance(transform.position, player.position) > attackRange * 1.5f)
                    {
                        ChangeState(State.Chase);
                    }
                    break;
            }
        }
        
        // Keep boss in position if it has a parent
        if (transform.parent != null)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    void ChasePlayer()
    {
        if (player == null || rb == null) return;

        // Use pathfinding if available and initialized
        if (usePathfinding && pathfindingInitialized && aiPath != null && aiPath.enabled)
        {
            // Let AIPath handle the movement
            return;
        }
        else
        {
            // Fallback to direct movement
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }

        // Face player direction
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
            
            // Enable pathfinding if initialized and available
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

            Debug.Log("Boss detected player! Distance: " + distanceToPlayer + ", Current State: " + currentState);

            // Don't change state if currently attacking and still in cooldown
            if (currentState == State.Attack && attackTimer > 0)
            {
                return;
            }

            // Check attack range
            if (distanceToPlayer <= attackRange && attackTimer <= 0)
            {
                attackTimer = attackCooldown;
                ChangeState(State.Attack);
            }
            // If outside attack range, chase
            else if (distanceToPlayer > attackRange && currentState != State.Attack)
            {
                ChangeState(State.Chase);
            }
            // If in attack range but still cooling down and NOT currently attacking
            else if (distanceToPlayer <= attackRange && attackTimer > 0 && currentState != State.Attack)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            // Disable pathfinding when no player detected
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
            
            // Return to idle if no player detected
            if (currentState == State.Chase)
            {
                ChangeState(State.Idle);
            }
        }
    }

    public void ChangeState(State newState)
    {
        if (isDead) return;
        
        Debug.Log("Boss changing state from " + currentState + " to " + newState);

        // Exit current state
        ExitCurrentState();

        // Change to new state
        currentState = newState;

        // Enter new state
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
                Debug.Log("Boss performing Attack");
                break;
            case State.Death:
                anim.SetTrigger("isDeath");
                isDead = true;
                rb.linearVelocity = Vector2.zero;
                // Disable pathfinding
                if (pathfindingInitialized)
                {
                    if (aiDestinationSetter != null)
                        aiDestinationSetter.enabled = false;
                    if (aiPath != null)
                        aiPath.enabled = false;
                }
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
            // Check if player is still in range
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= attackRange)
                {
                    rb.linearVelocity = Vector2.zero; // Wait for next attack
                    ChangeState(State.Idle); // Return to idle, will detect player again and potentially attack
                }
                else
                {
                    ChangeState(State.Chase); // Chase if player moved away
                }
            }
            else
            {
                ChangeState(State.Idle);
            }
        }
    }

    // Called from BossHealth when taking damage
    public void OnTakeDamage()
    {
        if (!isDead && currentState != State.Death && anim != null)
        {
            // Trigger hit animation without changing state
            anim.SetTrigger("isTakeHit");
            Debug.Log("Boss take hit animation triggered");
        }
    }

    // Called from BossHealth when dying
    public void OnDeath()
    {
        ChangeState(State.Death);
    }

    public void BossKnockback(Transform attacker, float force, float duration)
    {
        if (isDead) return;
        
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
        // Wait for pathfinding graph to be ready
        yield return new WaitForSeconds(0.5f);
        
        // Check if pathfinding graph exists and has nodes
        if (AstarPath.active != null && AstarPath.active.data != null && AstarPath.active.data.graphs != null)
        {
            // Try to find the closest node to current position
            var node = AstarPath.active.GetNearest(transform.position);
            if (node.node != null)
            {
                pathfindingInitialized = true;
                Debug.Log("Boss pathfinding initialized successfully");
                
                // Enable pathfinding components if they exist
                if (aiPath != null)
                {
                    aiPath.maxSpeed = speed;
                }
            }
            else
            {
                Debug.LogWarning("Boss: No valid pathfinding node found near position " + transform.position + ". Using direct movement.");
                usePathfinding = false;
            }
        }
        else
        {
            Debug.LogWarning("Boss: Pathfinding graph not ready. Using direct movement.");
            usePathfinding = false;
        }
    }

    // Method to manually snap boss to nearest valid pathfinding node
    public void SnapToNearestNode()
    {
        if (AstarPath.active != null)
        {
            var nearest = AstarPath.active.GetNearest(transform.position);
            if (nearest.node != null)
            {
                transform.position = (Vector3)nearest.position;
                Debug.Log("Boss snapped to nearest pathfinding node");
            }
        }
    }

    // For debugging
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