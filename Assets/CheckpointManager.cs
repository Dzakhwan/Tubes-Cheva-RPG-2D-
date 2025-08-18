using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    
    [Header("Spawn Settings")]
    public Vector3 initialSpawnPoint = Vector3.zero;
    private Vector3 currentCheckpoint;
    private bool hasCheckpoint = false;
    
    [Header("Respawn Settings")]
    public float respawnDelay = 0.5f;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Set initial spawn point
            currentCheckpoint = initialSpawnPoint;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        currentCheckpoint = checkpointPosition;
        hasCheckpoint = true;
        
        Debug.Log($"Checkpoint saved at: {checkpointPosition}");
        
        // Play checkpoint sound effect
        if (SoundEffectManager.Instance != null)
            SoundEffectManager.Play("Checkpoint");
    }
    
    public Vector3 GetCurrentSpawnPoint()
    {
        return hasCheckpoint ? currentCheckpoint : initialSpawnPoint;
    }
    
    public void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            StartCoroutine(RespawnCoroutine(player));
        }
    }
    
    private System.Collections.IEnumerator RespawnCoroutine(GameObject player)
    {
        // Get components
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Animator anim = player.GetComponent<Animator>();
        
        // Wait for respawn delay
        yield return new WaitForSeconds(respawnDelay);
        
        // Reset player position
        player.transform.position = GetCurrentSpawnPoint();
        
        // Reset player health and UI
        if (playerHealth != null)
        {
            StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
            playerHealth.UpdateHealthUI();
            playerHealth.isDead = false; // Reset death flag
        }
        
        // Reset rigidbody velocity
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        // Reset animator to idle state
        if (anim != null)
        {
            anim.SetBool("IsRunning", false);
            anim.SetFloat("horizontal", 0);
            anim.SetFloat("vertical", 0);
            anim.ResetTrigger("IsDeath");
        }
        
        // Re-enable player controls
        if (playerMovement != null)
            playerMovement.enabled = true;
            
        Debug.Log($"Player respawned at: {GetCurrentSpawnPoint()}");
    }
}