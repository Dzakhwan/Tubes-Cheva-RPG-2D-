using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    [Header("Loot")]
    public List<LootItem> lootItems = new List<LootItem>(); // List of loot items>
    private SpriteRenderer spriteRenderer;
    // public ExpManager expManager;
    public int expReward = 2;
    public delegate void MonsterDefeated(int expReward);
    public static event MonsterDefeated OnMonsterDefeated;




    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // Trigger hit effect when taking damage

        if (amount < 0)
            StartCoroutine(BlackFlash());
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
        foreach (LootItem item in lootItems)
        {
            if (Random.Range(0, 100) < item.dropChance)
            {
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public IEnumerator BlackFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.1f); // durasi flash
            spriteRenderer.color = Color.white;
        }
    }
}