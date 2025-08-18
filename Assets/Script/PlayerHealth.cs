using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    public Slider healthSlider;
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    
    private Color originalColor;
    public bool isDead = false; // Made public so CheckpointManager can access it
    
    void Start()
    {
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        
        UpdateHealthUI();
        
        // Simpan warna asli sprite
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }
    
    public void changeHealth(int amount)
    {
        if (isDead) return; // Prevent damage while dead
        
        StatsManager.Instance.currentHealth += amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(StatsManager.Instance.currentHealth, 0, StatsManager.Instance.maxHealth);
        
        UpdateHealthUI();
        
        // Kalau kena damage (amount < 0), mainkan efek flash
        if (amount < 0)
        {
            SoundEffectManager.Play("Hit");
            StartCoroutine(WhiteFlash());
        }
        
        // Kalau mati, mainkan animasi death
        if (StatsManager.Instance.currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        anim.SetTrigger("IsDeath");
        
        // Disable player movement
        GetComponent<PlayerMovement>().enabled = false;
    }
    
    public void Heal(int amount)
    {
        if (StatsManager.Instance.currentHealth >= StatsManager.Instance.maxHealth)
            return;
        
        StatsManager.Instance.currentHealth += amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(
            StatsManager.Instance.currentHealth, 
            0, 
            StatsManager.Instance.maxHealth
        );
        
        UpdateHealthUI();
        StartCoroutine(HealFlash());
    }
    
    // Updated method called from Animation Event
    public void OnDeathAnimationEnd()
    {
        // Respawn player immediately after death animation
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.RespawnPlayer();
        }
        else
        {
            // Fallback if no CheckpointManager
            RespawnAtCurrentPosition();
        }
    }
    
    void RespawnAtCurrentPosition()
    {
        isDead = false;
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        UpdateHealthUI();
        
        // Reset animator
        anim.SetBool("IsRunning", false);
        anim.SetFloat("horizontal", 0);
        anim.SetFloat("vertical", 0);
        anim.ResetTrigger("IsDeath");
        
        GetComponent<PlayerMovement>().enabled = true;
    }
    
    public void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = StatsManager.Instance.maxHealth;
            healthSlider.value = StatsManager.Instance.currentHealth;
        }
    }
    
    // Existing flash methods remain the same
    private System.Collections.IEnumerator WhiteFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
    
    private System.Collections.IEnumerator HealFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = originalColor;
        }
    }
}