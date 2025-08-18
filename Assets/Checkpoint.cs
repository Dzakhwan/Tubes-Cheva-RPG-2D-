using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public bool isActivated = false;
    
    [Header("Visual Effects")]
    public SpriteRenderer checkpointSprite;
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.yellow;
    public GameObject activationEffect; // Optional particle effect
    
    [Header("Audio")]
    public string activationSoundName = "Checkpoint";
    
    void Start()
    {
        UpdateVisuals();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }
    
    void ActivateCheckpoint()
    {
        isActivated = true;
        
        // Save this checkpoint position
        CheckpointManager.Instance.SetCheckpoint(transform.position);
        
        // Update visuals
        UpdateVisuals();
        
        // Play activation effect
        if (activationEffect != null)
        {
            GameObject effect = Instantiate(activationEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Clean up after 2 seconds
        }
        
        // Play sound
        if (SoundEffectManager.Instance != null)
            SoundEffectManager.Play(activationSoundName);
            
        Debug.Log($"Checkpoint activated at: {transform.position}");
    }
    
    void UpdateVisuals()
    {
        if (checkpointSprite != null)
        {
            checkpointSprite.color = isActivated ? activeColor : inactiveColor;
        }
    }
    
    // Method to deactivate other checkpoints (optional)
    public void Deactivate()
    {
        isActivated = false;
        UpdateVisuals();
    }
}