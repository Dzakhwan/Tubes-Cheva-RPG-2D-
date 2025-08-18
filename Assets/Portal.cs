using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Portal : MonoBehaviour
{
    public bool loadPrologueScene = true; // kalau true -> load scene
    public string prologueSceneName = "Prologue"; // nama scene prologue
    public TextMeshProUGUI congratsText; // teks selamat
    public float textDuration = 3f; // durasi teks tampil

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (loadPrologueScene)
            {
                // Pindah ke scene Prologue
                SceneManager.LoadScene(prologueSceneName);
            }
            else
            {
                // Tampilkan teks selamat
                StartCoroutine(ShowCongratsText());
            }
        }
    }

    private System.Collections.IEnumerator ShowCongratsText()
    {
        congratsText.gameObject.SetActive(true);
        congratsText.text = "🎉 Selamat! Kamu berhasil mengalahkan Boss! 🎉";

        yield return new WaitForSecondsRealtime(textDuration);

        congratsText.gameObject.SetActive(false);
    }
}
