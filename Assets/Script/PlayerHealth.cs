using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    public Slider healthSlider;
    public Animator anim;
    public SpriteRenderer spriteRenderer; // untuk efek flash

    private Color originalColor;

    void Start()
    {
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = StatsManager.Instance.maxHealth;
            healthSlider.value = StatsManager.Instance.currentHealth;
        }

        // Simpan warna asli sprite
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }


    public void changeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(StatsManager.Instance.currentHealth, 0, StatsManager.Instance.maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = StatsManager.Instance.currentHealth;
        }

        // Kalau kena damage (amount < 0), mainkan efek flash
        if (amount < 0)
        {
            SoundEffectManager.Play("Hit");
            StartCoroutine(WhiteFlash());
        }
        // Kalau mati, mainkan animasi death
        if (StatsManager.Instance.currentHealth <= 0)
        {
            anim.SetTrigger("IsDeath");
        }
       
    }
 public void Heal(int amount)
    {
        if (StatsManager.Instance.currentHealth >= StatsManager.Instance.maxHealth)
            return; // Already at full health

        StatsManager.Instance.currentHealth += amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(
            StatsManager.Instance.currentHealth, 
            0, 
            StatsManager.Instance.maxHealth
        );

        if (healthSlider != null)
            healthSlider.value = StatsManager.Instance.currentHealth;

        StartCoroutine(HealFlash()); // Visual feedback
    }
    // Dipanggil dari Animation Event pada akhir animasi death
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    // Coroutine untuk efek flash putih
    private System.Collections.IEnumerator WhiteFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.1f); // durasi flash
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
