using UnityEngine;

public class SwordEventRelay : MonoBehaviour
{
    public PlayerAttack playerAttack;

    private void Start()
    {
        if (playerAttack == null)
        {
            playerAttack = GetComponentInParent<PlayerAttack>();
        }
    }

    // Fungsi ini dipanggil dari Animation Event di animasi Slash
    public void SlashEffectTrigger()
    {
        if (playerAttack != null)
        {
            playerAttack.TriggerSlash(); // fungsi yang ingin kamu panggil di PlayerAttack
        }
    }
}
