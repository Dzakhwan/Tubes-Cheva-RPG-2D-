using UnityEngine;
using UnityEngine.UI; // Tambahkan ini

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] 
    
    public Slider healthSlider; // Tambahkan ini

    void Start()
    {
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;

        // Pastikan slider di-set pada awal
        if (healthSlider != null)
        {
            healthSlider.maxValue = StatsManager.Instance.maxHealth;
            healthSlider.value = StatsManager.Instance.currentHealth;
        }
    }

    public void changeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;

        // Batas atas dan bawah
        StatsManager.Instance.currentHealth = Mathf.Clamp(StatsManager.Instance.currentHealth, 0, StatsManager.Instance.maxHealth);

        // Update slider
        if (healthSlider != null)
        {
            healthSlider.value = StatsManager.Instance.currentHealth;
        }

        if (StatsManager.Instance.currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
