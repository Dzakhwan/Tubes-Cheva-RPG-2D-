using System.Collections;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    public float stunTime = 1f;

    private Rigidbody2D rb;
    private EnemyFSM enemyFSM;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyFSM = GetComponent<EnemyFSM>();

    }



    public void knockback(Transform playerTransform, float knockbackForce,float knockbackTime, float stunTime)
    {
        // Hitung arah knockback
        enemyFSM.ChangeState(EnemyFSM.State.Knockback);
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = direction * knockbackForce;

        // Mulai timer stun
        StartCoroutine(KnockbackCounter(stunTime, knockbackTime));

        Debug.Log($"Knockback applied to {gameObject.name}: direction={direction}, force={knockbackForce}");
    }
    IEnumerator KnockbackCounter(float duration,float knockbackTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        enemyFSM.ChangeState(EnemyFSM.State.Idle);
    }

}