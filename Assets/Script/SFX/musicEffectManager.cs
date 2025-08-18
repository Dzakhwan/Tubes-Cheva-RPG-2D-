using UnityEngine;
using System.Collections;

public class musicEffectManager : MonoBehaviour
{
    public static musicEffectManager Instance { get; private set; }

    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float maxVolume = 0.2f;

    private Coroutine fadeCoroutine;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();

            audioSource.loop = true;
            audioSource.volume = maxVolume;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(string musicName)
    {
        AudioClip clip = soundEffectLibrary.GetRandomClip(musicName);
        if (clip == null) return;

        if (Instance.fadeCoroutine != null)
            Instance.StopCoroutine(Instance.fadeCoroutine);

        Instance.fadeCoroutine = Instance.StartCoroutine(Instance.FadeMusic(clip));
    }

    private IEnumerator FadeMusic(AudioClip newMusic)
    {
        float startVolume = audioSource.volume;

        // Fade out musik lama
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();

        // Ganti clip
        audioSource.clip = newMusic;
        audioSource.volume = 0f;
        audioSource.Play();

        // Fade in musik baru
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = maxVolume;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public static void Stop()
    {
        if (Instance.fadeCoroutine != null)
            Instance.StopCoroutine(Instance.fadeCoroutine);

        Instance.InstanceFadeOut();
    }

    private void InstanceFadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();
    }
}
