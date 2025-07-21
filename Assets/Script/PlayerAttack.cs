using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject slashEffect;                   // Prefab slash effect
    public Transform attackPoint, attackPointUp, attackPointDown; // Titik serangan
    private Vector2 lastMoveDir = Vector2.right;
    public LayerMask enemyLayer;
    public float attackRange = 1.5f;
    public int damageAmount = 3;

    public float slashDuration = 0.5f;

    public Transform weaponParent;                   // Parent senjata
    public GameObject weaponObject;                  // Senjata (hand_sword_0)
    public Animator anim;                            // Animator player
    public float attackDuration = 0.3f;

    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (moveInput != Vector2.zero)
        {
            lastMoveDir = moveInput.normalized;
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartAttack();
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        if (anim != null)
            anim.SetTrigger("Attack");

        Transform selectedAttackPoint = attackPoint;

        if (Mathf.Abs(lastMoveDir.y) > Mathf.Abs(lastMoveDir.x))
        {
            if (lastMoveDir.y > 0)
                selectedAttackPoint = attackPointUp;
            else
                selectedAttackPoint = attackPointDown;
        }

        GameObject slash = Instantiate(slashEffect, selectedAttackPoint.position, Quaternion.identity);
        Destroy(slash, slashDuration);

        // Rotasi
        if (lastMoveDir.x > 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (lastMoveDir.x < 0)
            slash.transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (lastMoveDir.y > 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (lastMoveDir.y < 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, -90);

        // Serang musuh di area
        Debug.Log("Attacking at position: " + selectedAttackPoint.position + " with range: " + attackRange);
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(selectedAttackPoint.position, attackRange, enemyLayer);
        Debug.Log("Found " + hitEnemies.Length + " enemies in range");
        
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name);
            
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                Debug.Log("Dealing damage to: " + enemy.name);
                health.ChangeHealth(-damageAmount);
                
                // Gunakan HitEffect system untuk knockback dan flash
                HitEffect hitEffect = enemy.GetComponent<HitEffect>();
                if (hitEffect != null)
                {
                    Debug.Log("Triggering hit effect on: " + enemy.name);
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    hitEffect.TriggerHitEffect(knockbackDirection);
                }
                else
                {
                    Debug.LogError("No HitEffect component found on: " + enemy.name);
                }
            }
            else
            {
                Debug.LogError("No EnemyHealth component found on: " + enemy.name);
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }
}