using UnityEngine;

public class StatsUIController : MonoBehaviour
{
    public GameObject statsUI; // drag & drop StatsUI di inspector
    public StatsUI statsUIController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // tekan tombol
        {
            bool isActive = statsUI.activeSelf;
            statsUI.SetActive(!isActive); // toggle tampil/hilang
            statsUIController.UpdateStats();
        }
    }
}
