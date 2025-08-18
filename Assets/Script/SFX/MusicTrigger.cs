using UnityEngine;

using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private string musicName; // nama musik di library

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            musicEffectManager.Play(musicName); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            musicEffectManager.Stop(); 
        }
    }
}
