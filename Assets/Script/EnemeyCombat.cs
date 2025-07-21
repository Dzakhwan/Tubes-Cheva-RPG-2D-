using UnityEngine;

public class EnemeyCombat : MonoBehaviour
{
    public int damage = 1;
    public Animator anim;
    public Transform AttackPoint;
    public float weaponRange ;
    public float StunTime;
    public float knockbackForce;
    public LayerMask playerLayer;

    
    
    public void Attack()
{
    if (AttackPoint == null)
    {
        Debug.LogError("AttackPoint is not assigned!");
        return;
    }
    
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, weaponRange, playerLayer);
        if (hitEnemies.Length > 0)
        {
            PlayerHealth playerHealth = hitEnemies[0].gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.changeHealth(-damage);
                Debug.Log("We hit " + hitEnemies[0].name);
                // Add hit effect and knockback

            }
            else
            {
                Debug.LogError("Player doesn't have PlayerHealth component!");
            }
        hitEnemies[0].GetComponent<PlayerMovement>().knockback(transform, knockbackForce,StunTime);
    }
}

    
}
