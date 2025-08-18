using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PrologueManager : MonoBehaviour
{
    
    public float duration = 5f;

    void Start()
    {
        
        StartCoroutine(BackToMenu());
    }

    private System.Collections.IEnumerator BackToMenu()
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // pastikan MainMenu ada di Build Settings
    }
}
