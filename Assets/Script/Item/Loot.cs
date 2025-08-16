using UnityEngine;
using System;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;
    public int quantity = 1; // Default quantity
    
    public static event Action<ItemSO, int> OnItemLooted;

    private void OnValidate()
    {
        if (itemSO == null)
            return;

        sr.sprite = itemSO.itemIcon;
        this.name = "Loot: " + itemSO.itemName;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnItemLooted?.Invoke(itemSO, quantity);
            
            // Play different sound based on item type
            PlayPickupSound();
            
            Destroy(gameObject);
        }
    }
    
    private void PlayPickupSound()
    {
        switch (itemSO.itemType)
        {
            case ItemSO.ItemType.Coin:
                SoundEffectManager.Play("Coin");
                break;
            case ItemSO.ItemType.HealthPotion:
                SoundEffectManager.Play("HealthPotion");
                break;
        }
    }
}