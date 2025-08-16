using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int coins;
    public TMP_Text coinText;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSO itemSO, int quantity)
    {
        switch (itemSO.itemType)
        {
            case ItemSO.ItemType.Coin:
                coins += quantity * itemSO.coinValue;
                coinText.text = coins.ToString();
                SoundEffectManager.Play("Coin");
                break;

            case ItemSO.ItemType.HealthPotion:
                if (playerHealth != null)
                {
                    playerHealth.Heal(itemSO.healAmount);
                    SoundEffectManager.Play("HealthPotion");
                    Debug.Log($"Healed by {itemSO.healAmount} HP!");
                }
                break;
        }
    }
}