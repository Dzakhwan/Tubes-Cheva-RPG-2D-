using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public GameObject slashEffect;                   // Prefab slash effect
    public Transform attackPoint, attackPointUp, attackPointDown; // Titik serangan
    private Vector2 lastMoveDir = Vector2.right;
    public LayerMask enemyLayer;

    public float slashDuration = 0.5f;               // Waktu efek slash bertahan
    public float slashDelay = 0.1f;                  // Jeda sebelum slash muncul

    public Transform weaponParent;
    public GameObject weaponObject;
    public Animator anim;
    public float attackDuration = 0.3f;

    private bool isAttacking = false;
    private float attackTimer = 0f;

    private int comboStep = 0;
    private float comboResetTime = 1f;
    private float comboTimer = 0f;

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (moveInput != Vector2.zero)
        {
            lastMoveDir = moveInput.normalized;
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            comboStep++;
            comboTimer = comboResetTime;

            if (comboStep > 2)
                comboStep = 1;

            StartAttack(comboStep);
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }

        if (comboStep > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                comboStep = 0;
            }
        }
    }

    void StartAttack(int comboIndex)
    {
        isAttacking = true;
        attackTimer = attackDuration;

        if (anim != null)
            anim.SetTrigger("Attack" + comboIndex);

        Transform selectedAttackPoint = attackPoint;
        if (Mathf.Abs(lastMoveDir.y) > Mathf.Abs(lastMoveDir.x))
        {
            selectedAttackPoint = lastMoveDir.y > 0 ? attackPointUp : attackPointDown;
        }

        // Kirim posisi dan arah saat itu ke coroutine
        StartCoroutine(DelayedSlash(selectedAttackPoint.position, lastMoveDir));
    }

    IEnumerator DelayedSlash(Vector3 slashPosition, Vector2 moveDir)
    {
        yield return new WaitForSeconds(slashDelay);

        GameObject slash = Instantiate(slashEffect, slashPosition, Quaternion.identity);
        Destroy(slash, slashDuration);

        // Rotasi efek berdasarkan arah saat serang
        if (moveDir.x > 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (moveDir.x < 0)
            slash.transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (moveDir.y > 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (moveDir.y < 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, -90);

        // Deteksi musuh di posisi saat itu
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            slashPosition,
            StatsManager.Instance.attackRange,
            enemyLayer
        );

        if (hitEnemies.Length > 0)
        {
            hitEnemies[0].GetComponent<EnemyHealth>().ChangeHealth(-StatsManager.Instance.damageAmount);
            hitEnemies[0].GetComponent<EnemyKnockback>().knockback(
                transform,
                StatsManager.Instance.knockbackForce,
                StatsManager.Instance.stunTime,
                StatsManager.Instance.knockbackTime
            );
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (StatsManager.Instance == null) return;

        Gizmos.color = Color.red;

        Transform selectedAttackPoint = attackPoint;
        if (Mathf.Abs(lastMoveDir.y) > Mathf.Abs(lastMoveDir.x))
        {
            selectedAttackPoint = lastMoveDir.y > 0 ? attackPointUp : attackPointDown;
        }

        if (selectedAttackPoint != null)
            Gizmos.DrawWireSphere(selectedAttackPoint.position, StatsManager.Instance.attackRange);
    }
}
