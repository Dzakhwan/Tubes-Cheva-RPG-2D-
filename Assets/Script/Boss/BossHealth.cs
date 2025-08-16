using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Tambahin ini biar bisa akses Slider

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("UI")]
    public Slider bossSlider; // Slider health di Canvas

    [Header("Health Phases")]
    [Range(0, 1)] public float phase2HealthThreshold = 0.66f;
    [Range(0, 1)] public float phase3HealthThreshold = 0.33f;
    private int currentPhase = 1;

    [Header("Boss Loot")]
    public List<LootItem> commonLoot = new List<LootItem>();
    public List<LootItem> rareLoot = new List<LootItem>();
    public List<LootItem> guaranteedLoot = new List<LootItem>();

    [Header("Visual Effects")]
    private SpriteRenderer spriteRenderer;
    public GameObject deathEffect;
    private Color originalColor; 

    [Header("Rewards")]
    public int expReward = 50;
    public delegate void BossDefeated(int expReward);
    public static event BossDefeated OnBossDefeated;

    [Header("References")]
    public Animator anim;
    private BossFSM bossFSM;

    [Header("Damage Immunity")]
    public float damageImmunityDuration = 0.2f;
    private bool isDamageImmune = false;
    private bool isDead = false;

    [Header("Phase Change Effects")]
    public GameObject phase2Effect;
    public GameObject phase3Effect;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
       originalColor = Color.white;
        spriteRenderer.color = originalColor;
        bossFSM = GetComponent<BossFSM>();
        currentHealth = maxHealth;

        // Sembunyikan slider di awal
        if (bossSlider != null)
        {
            bossSlider.maxValue = maxHealth;
            bossSlider.value = maxHealth;
            bossSlider.gameObject.SetActive(false);
        }

        if (bossFSM == null)
        {
            Debug.LogWarning("BossFSM component not found on " + gameObject.name);
        }
    }

    public void ChangeHealth(int amount)
    {
        if (isDead) return;

        if (amount < 0)
        {
            if (!isDamageImmune)
            {
                currentHealth += amount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                StartCoroutine(DamageImmunity());
                CheckPhaseChange();
                Debug.Log($"Boss took {-amount} damage. Health: {currentHealth}/{maxHealth}");
            }
            else
            {
                Debug.Log("Boss is damage immune - no damage taken");
            }

            StartCoroutine(HitEffect());

            if (bossFSM != null)
            {
                bossFSM.OnTakeDamage();
            }

            // Tampilkan slider saat pertama kali kena damage
            if (bossSlider != null)
            {
                if (!bossSlider.gameObject.activeSelf)
                {
                    bossSlider.gameObject.SetActive(true);
                }
                bossSlider.value = currentHealth;
            }
        }
        else if (amount > 0)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (bossSlider != null)
            {
                bossSlider.value = currentHealth;
            }
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void CheckPhaseChange()
    {
        float healthPercentage = (float)currentHealth / maxHealth;

        if (currentPhase == 1 && healthPercentage <= phase2HealthThreshold)
        {
            currentPhase = 2;
            OnPhaseChange(2);
        }
        else if (currentPhase == 2 && healthPercentage <= phase3HealthThreshold)
        {
            currentPhase = 3;
            OnPhaseChange(3);
        }
    }

    private void OnPhaseChange(int newPhase)
    {
        Debug.Log("Boss entering Phase " + newPhase);

        GameObject effectToSpawn = null;
        if (newPhase == 2 && phase2Effect != null)
        {
            effectToSpawn = phase2Effect;
        }
        else if (newPhase == 3 && phase3Effect != null)
        {
            effectToSpawn = phase3Effect;
        }

        if (effectToSpawn != null)
        {
            Instantiate(effectToSpawn, transform.position, Quaternion.identity);
        }

        if (bossFSM != null)
        {
            switch (newPhase)
            {
                case 2:
                    bossFSM.attackCooldown *= 0.8f;
                    bossFSM.speed *= 1.2f;
                    break;
                case 3:
                    bossFSM.attackCooldown *= 0.6f;
                    bossFSM.speed *= 1.5f;
                    break;
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Boss " + gameObject.name + " has been defeated!");

        if (bossFSM != null)
        {
            bossFSM.OnDeath();
        }

        if (anim != null)
        {
            anim.SetTrigger("isDeath");
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        OnBossDefeated?.Invoke(expReward);

        // Sembunyikan slider saat boss mati
        if (bossSlider != null)
        {
            bossSlider.gameObject.SetActive(false);
        }

        StartCoroutine(DropLootAfterDelay(2f));
    }

    private IEnumerator DropLootAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DropLoot();

        yield return new WaitForSeconds(1f);
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DropLoot()
    {
        Vector3 dropPosition = transform.position;

        foreach (LootItem item in guaranteedLoot)
        {
            if (item.itemPrefab != null)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                Instantiate(item.itemPrefab, dropPosition + randomOffset, Quaternion.identity);
            }
        }

        foreach (LootItem item in commonLoot)
        {
            if (Random.Range(0, 100) < item.dropChance)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                Instantiate(item.itemPrefab, dropPosition + randomOffset, Quaternion.identity);
            }
        }

        foreach (LootItem item in rareLoot)
        {
            if (Random.Range(0, 100) < item.dropChance)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
                Instantiate(item.itemPrefab, dropPosition + randomOffset, Quaternion.identity);
            }
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public int GetCurrentPhase()
    {
        return currentPhase;
    }

    public IEnumerator HitEffect()
    {
        if (spriteRenderer != null)
        {
            
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator DamageImmunity()
    {
        isDamageImmune = true;
        yield return new WaitForSeconds(damageImmunityDuration);
        isDamageImmune = false;
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (bossSlider != null)
        {
            bossSlider.value = currentHealth;
        }

        Debug.Log("Boss healed for " + amount + " HP. Current: " + currentHealth);
    }
}
