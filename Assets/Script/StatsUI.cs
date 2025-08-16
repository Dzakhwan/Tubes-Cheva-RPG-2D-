using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] stats;
    public GameObject statsPanel;
      

     private void Start()
    {
        // Menyembunyikan semua UI di awal game
        SetStatsVisibility(false);
    }

    
  private void SetStatsVisibility(bool visible)
{
    gameObject.SetActive(visible); // ini akan menyembunyikan parent + semua child
}


    public void UpdateDamage()
    {
        stats[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatsManager.Instance.damageAmount;
    }
    public void UpdateHealth()
    {
        stats[1].GetComponentInChildren<TMP_Text>().text = "Health: " + StatsManager.Instance.maxHealth;
    }
     public void UpdateKnocback()
    {
        stats[2].GetComponentInChildren<TMP_Text>().text = "Knockback: " + StatsManager.Instance.knockbackForce;
    }
     public void UpdateRange()
    {
        stats[3].GetComponentInChildren<TMP_Text>().text = "Range: " + StatsManager.Instance.attackRange;
    }
     public void UpdateSpeed()
    {
        stats[4].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }
     public void UpdateStun()
    {
        stats[5].GetComponentInChildren<TMP_Text>().text = "Stun: " + StatsManager.Instance.stunTime;
    }
    public void CurrentHealth()
    {
        stats[6].GetComponentInChildren<TMP_Text>().text = "Health: " + StatsManager.Instance.currentHealth;
    }
    public void UpdateStats()
    {
        UpdateDamage();
        UpdateHealth();
        UpdateKnocback();
        UpdateRange();
        UpdateSpeed();
        UpdateStun();
        CurrentHealth();
    }
}
