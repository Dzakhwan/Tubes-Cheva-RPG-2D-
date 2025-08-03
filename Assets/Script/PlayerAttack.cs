using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public GameObject slashEffect;
    public Transform attackPoint, attackPointUp, attackPointDown;
    private Vector2 lastMoveDir = Vector2.right;
    public LayerMask enemyLayer;

    public float slashDuration = 0.5f;

    public Transform weaponParent;
    public GameObject weaponObject;
    public Animator anim;
    public float attackDuration = 0.3f;

    private bool isAttacking = false;
    private float attackTimer = 0f;

    private int comboStep = 0;
    private float comboResetTime = 1f;
    private float comboTimer = 0f;

    private Transform currentAttackPoint;
    private Vector2 currentAttackDir;

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

        // Simpan arah dan posisi saat serangan dimulai
        currentAttackDir = lastMoveDir;

        currentAttackPoint = attackPoint;
        if (Mathf.Abs(currentAttackDir.y) > Mathf.Abs(currentAttackDir.x))
        {
            currentAttackPoint = currentAttackDir.y > 0 ? attackPointUp : attackPointDown;
        }
    }

    /// <summary>
    /// Dipanggil oleh Animation Event di animasi serangan
    /// </summary>
    public void TriggerSlash()
    {
        if (currentAttackPoint == null) return;

        GameObject slash = Instantiate(slashEffect, currentAttackPoint.position, Quaternion.identity);
        Destroy(slash, slashDuration);

        if (currentAttackDir.x > 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (currentAttackDir.x < 0)
            slash.transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (currentAttackDir.y > 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (currentAttackDir.y < 0)
            slash.transform.rotation = Quaternion.Euler(0, 0, -90);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            currentAttackPoint.position,
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
