using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
    [Header("Flash Effect")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;
    
    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void TriggerHitEffect(Vector2 knockbackDirection)
    {
        // Flash effect
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }
        
        // Knockback effect
        if (rb != null && !isKnockedBack)
        {
            StartCoroutine(KnockbackEffect(knockbackDirection));
        }
    }
    
    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;
        
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    
    private IEnumerator KnockbackEffect(Vector2 direction)
    {
        isKnockedBack = true;
        
        // Apply knockback force
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        
        // Wait for knockback duration
        yield return new WaitForSeconds(knockbackDuration);
        
        // Stop knockback (optional: gradually reduce velocity)
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
    
    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
}