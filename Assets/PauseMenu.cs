using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;    // Panel Pause Menu
    public TextMeshProUGUI funnyText; // Text ledekan
    private bool isPaused = false;

    void Update()
    {
        // Tekan Escape untuk toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // stop waktu game
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        funnyText.gameObject.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ExitGame()
    {
        // tampilkan teks konyol lalu pindah scene
        funnyText.text = GetRandomFunnyText();
        funnyText.gameObject.SetActive(true);

        // mulai coroutine untuk delay sebelum load main menu
        StartCoroutine(ExitAfterDelay(2f)); // 2 detik delay
    }

    private IEnumerator ExitAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // pake Realtime biar ga kena Time.timeScale
        Time.timeScale = 1f; // pastikan game jalan lagi sebelum ganti scene
        SceneManager.LoadScene("MainMenu"); // ganti sesuai nama scene Main Menu kamu
    }

    private string GetRandomFunnyText()
    {
        string[] jokes =
        {
            "Mau kabur? Main lagi besok ya 😜",
            "Keluar detected... tapi jangan baper ya 😂",
            "Cupu mode ON: keluar dari game 🤭",
            "Oke deh, musuhnya juga butuh istirahat 😴",
            "Bye bye... jangan lupa makan sayur! 🥦"
        };

        int rand = Random.Range(0, jokes.Length);
        return jokes[rand];
    }
}
