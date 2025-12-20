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
    // Add more clips for other scenes as needed
    
    [Header("Audio Settings")]
    public float musicVolume = 0.5f;
    public float fadeDuration = 1f;
    
    private AudioSource musicSource;
    private Dictionary<string, AudioClip> sceneMusicMap;
    
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
        
        // Initialize AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        
        // Setup scene-to-music mapping
        InitializeMusicMap();
        
        // Subscribe to scene events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start()
    {
        // Play music for initial scene
        PlaySceneMusic(SceneManager.GetActiveScene().name);
    }
    
    private void InitializeMusicMap()
    {
        sceneMusicMap = new Dictionary<string, AudioClip>
        {
            { "drrarne", MainGameMusic },
            { "MenuScene", MenuMusic },
            { "Boss_Jor", BossMusic }
            // Add more mappings as needed
        };
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);
    }
    
    public void PlaySceneMusic(string sceneName)
    {
        if (sceneMusicMap.TryGetValue(sceneName, out AudioClip clipToPlay))
        {
            if (clipToPlay != null && musicSource.clip != clipToPlay)
            {
                StartCoroutine(FadeToNewMusic(clipToPlay));
            }
            else if (clipToPlay == null && musicSource.isPlaying)
            {
                // Stop music if scene has no assigned clip
                StartCoroutine(FadeOutMusic());
            }
        }
        else
        {
            Debug.LogWarning($"No music assigned for scene: {sceneName}");
        }
    }
    
    private IEnumerator FadeToNewMusic(AudioClip newClip)
    {
        // Fade out current music if playing
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeMusicVolume(1f, 0f));
        }
        
        // Switch to new clip
        musicSource.clip = newClip;
        musicSource.Play();
        
        // Fade in new music
        yield return StartCoroutine(FadeMusicVolume(0f, 1f));
    }
    
    private IEnumerator FadeOutMusic()
    {
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeMusicVolume(1f, 0f));
            musicSource.Stop();
        }
    }
    
    private IEnumerator FadeMusicVolume(float startVolume, float endVolume)
    {
        float timer = 0f;
        musicSource.volume = startVolume;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            musicSource.volume = Mathf.Lerp(startVolume, endVolume, progress);
            yield return null;
        }
        
        musicSource.volume = endVolume;
    }
    
    // Public methods for manual control
    public void PlayMusic(AudioClip clip, bool fade = true)
    {
        if (clip == null) return;
        
        if (fade)
        {
            StartCoroutine(FadeToNewMusic(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
            musicSource.volume = musicVolume;
        }
    }
    
    public void StopMusic(bool fade = true)
    {
        if (fade)
        {
            StartCoroutine(FadeOutMusic());
        }
        else
        {
            musicSource.Stop();
        }
    }
    
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }
    
    public void AddSceneMusic(string sceneName, AudioClip musicClip)
    {
        if (!sceneMusicMap.ContainsKey(sceneName))
        {
            sceneMusicMap.Add(sceneName, musicClip);
        }
        else
        {
            sceneMusicMap[sceneName] = musicClip;
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}