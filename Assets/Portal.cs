using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Portal : MonoBehaviour
{
    public bool loadPrologueScene = true; // kalau true -> load scene
   
   
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (loadPrologueScene)
            {
                // Pindah ke scene Prologue
                SceneManager.LoadScene("Prologue");
             StartCoroutine(BackToMenu(duration));
            }

        }
    }

    private System.Collections.IEnumerator BackToMenu(float duration)
    {

        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

        
    }
}
