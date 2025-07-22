using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    // public ExpManager expManager;
    public int expReward = 2;
    public delegate void MonsterDefeated(int expReward);
    public static event MonsterDefeated OnMonsterDefeated;
    
    
   
    
    void Start()
    {
        currentHealth = maxHealth;
        
    }
    
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // Trigger hit effect when taking damage


        if (currentHealth <= 0)
        {
            Die();
            OnMonsterDefeated(expReward);
            // expManager.GainExp(2);
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