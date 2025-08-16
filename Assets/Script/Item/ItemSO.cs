using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    // Definisikan enum langsung di dalam class
    public enum ItemType
    {
        Coin,
        HealthPotion
        // Tambahkan tipe lain jika diperlukan
    }

    // Data item
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;  // Menggunakan enum yang sudah didefinisikan di sini
    
    [Header("Coin Properties")]
    public int coinValue = 1;
    
    [Header("Health Potion Properties")]
    public int healAmount = 20;
}