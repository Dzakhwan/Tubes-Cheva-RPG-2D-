using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void changeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
