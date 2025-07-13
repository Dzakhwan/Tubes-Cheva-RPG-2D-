using UnityEngine;
using UnityEngine.UI; // Tambahkan ini

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] 
    private int maxHealth = 10;
    private int currentHealth;
    public Slider healthSlider; // Tambahkan ini

    void Start()
    {
        currentHealth = maxHealth;

        // Pastikan slider di-set pada awal
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void changeHealth(int amount)
    {
        currentHealth += amount;

        // Batas atas dan bawah
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
