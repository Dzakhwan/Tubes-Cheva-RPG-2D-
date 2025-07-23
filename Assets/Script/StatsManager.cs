using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    [Header("Combat Stats")]
    public float knockbackForce;
    public float knockbackTime;
    public float attackRange;
    public int damageAmount;
    public float stunTime;

    [Header("Movement Stats")]
    public float speed;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
