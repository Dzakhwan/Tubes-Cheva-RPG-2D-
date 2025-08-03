using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int coin;
    public TMP_Text coinText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        coin += quantity;
        coinText.text = coin.ToString();
        return;
    }
}
