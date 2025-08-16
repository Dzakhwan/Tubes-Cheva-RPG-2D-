using UnityEngine;

public class DungeonStepTrigger : MonoBehaviour
{
    public string newFootstepSound = "DungeonStep";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().SetFootstepSound(newFootstepSound);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().SetFootstepSound("Footstep"); // balik ke default
        }
    }
}
