using UnityEngine;

public class EnemeyCombat : MonoBehaviour
{
    public int damage = 1;
    public Animator anim;
    public Transform AttackPoint;
    public float weaponRange ;
    public LayerMask playerLayer;

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().changeHealth(-damage);

        }

    }
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
        }
        else
        {
            Debug.LogError("Player doesn't have PlayerHealth component!");
        }
    }
}

    
}
