using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    
    [Header("Components")]
    private HitEffect hitEffect;
    
    void Start()
    {
        currentHealth = maxHealth;
        hitEffect = GetComponent<HitEffect>();
    }
    
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        
        // Trigger hit effect when taking damage
        if (amount < 0 && hitEffect != null)
        {
            // Get knockback direction from player (you might need to adjust this)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;
                hitEffect.TriggerHitEffect(knockbackDirection);
            }
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // Add death logic here
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}