using Unity.VisualScripting;
using UnityEngine;
using System;

public class Loot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;
    public int quantity;
    public static event Action<ItemSO,int> OnItemLooted;

    private void OnValidate()
    {
        if (itemSO == null)
            return;

        sr.sprite = itemSO.itemIcon;
        this.name = itemSO.itemName;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnItemLooted?.Invoke(itemSO, quantity);
            SoundEffectManager.Play("Coin");
            Destroy(gameObject);
        }
    }
}
