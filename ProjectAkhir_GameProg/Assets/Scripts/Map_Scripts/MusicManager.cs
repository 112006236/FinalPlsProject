using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Scene Music Clips")]
    public AudioClip MainGameMusic;
    public AudioClip MenuMusic;
    public AudioClip BossMusic;

    [Header("End State Music")]
    public AudioClip DeathMusic;
    public AudioClip WinMusic;

    [Header("Audio Settings")]
    public float musicVolume = 0.5f;
    public float fadeDuration = 1f;

    private AudioSource musicSource;
    private Dictionary<string, AudioClip> sceneMusicMap;

    private bool deathMusicPlayed = false;
    private bool winMusicPlayed = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        InitializeMusicMap();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlaySceneMusic(SceneManager.GetActiveScene().name);
    }

    private void InitializeMusicMap()
    {
        sceneMusicMap = new Dictionary<string, AudioClip>
        {
            { "MenuScene", MenuMusic },
            { "drrarne", MainGameMusic },
            { "Boss_Jor", BossMusic }
        };
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        deathMusicPlayed = false;
        winMusicPlayed = false;
        PlaySceneMusic(scene.name);
    }

    // =========================
    // SCENE MUSIC
    // =========================

    public void PlaySceneMusic(string sceneName)
    {
        if (!sceneMusicMap.TryGetValue(sceneName, out AudioClip clipToPlay))
        {
            Debug.LogWarning($"No music assigned for scene: {sceneName}");
            return;
        }

        if (clipToPlay == null)
        {
            StopMusic(true);
            return;
        }

        if (musicSource.clip == clipToPlay && musicSource.isPlaying)
            return;

        StartCoroutine(FadeToNewMusic(clipToPlay));
    }

    private IEnumerator FadeToNewMusic(AudioClip newClip)
    {
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeMusicVolume(musicSource.volume, 0f));
        }

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        yield return StartCoroutine(FadeMusicVolume(0f, musicVolume));
    }

    private IEnumerator FadeOutMusic()
    {
        if (!musicSource.isPlaying)
            yield break;

        yield return StartCoroutine(FadeMusicVolume(musicSource.volume, 0f));
        musicSource.Stop();
    }

    private IEnumerator FadeMusicVolume(float startVolume, float endVolume)
    {
        float timer = 0f;
        musicSource.volume = startVolume;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, endVolume, timer / fadeDuration);
            yield return null;
        }

        musicSource.volume = endVolume;
    }

    // =========================
    // END STATES
    // =========================

    public void PlayDeathMusic()
    {
        if (DeathMusic == null || deathMusicPlayed)
            return;

        deathMusicPlayed = true;
        winMusicPlayed = true; // prevent overlap
        StartCoroutine(FadeToEndMusic(DeathMusic));
    }

    public void PlayWinMusic()
    {
        if (WinMusic == null || winMusicPlayed)
            return;

        winMusicPlayed = true;
        deathMusicPlayed = true; // prevent overlap
        StartCoroutine(FadeToEndMusic(WinMusic));
    }

    private IEnumerator FadeToEndMusic(AudioClip endClip)
    {
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeMusicVolume(musicSource.volume, 0f));
        }

        musicSource.clip = endClip;
        musicSource.loop = false;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    // =========================
    // MANUAL CONTROLS
    // =========================

    public void PlayMusic(AudioClip clip, bool fade = true)
    {
        if (clip == null) return;

        if (fade)
            StartCoroutine(FadeToNewMusic(clip));
        else
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic(bool fade = true)
    {
        if (fade)
            StartCoroutine(FadeOutMusic());
        else
            musicSource.Stop();
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void AddSceneMusic(string sceneName, AudioClip musicClip)
    {
        sceneMusicMap[sceneName] = musicClip;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
