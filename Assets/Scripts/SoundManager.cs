using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        
        [HideInInspector]
        public AudioSource source;
    }

    [Header("Ses Efektleri")]
    public Sound[] sounds;

    [Header("Ses Ayarları")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    public bool isSoundOn = true;

    [Header("Müzik Ayarları")]
    [SerializeField] private Sound[] backgroundMusics;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    private Sound currentMusic;
    private List<int> playedMusicIndices = new List<int>(); // Çalınan müzikleri takip etmek için

    private void Awake()
    {
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

        InitializeSounds();
    }

    private void InitializeSounds()
    {
        // Efekt sesleri için
        foreach (Sound s in sounds)
        {
            SetupAudioSource(s);
        }

        // Müzikler için
        foreach (Sound s in backgroundMusics)
        {
            SetupAudioSource(s);
        }
    }

    private void SetupAudioSource(Sound sound)
    {
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume * (sound.loop ? musicVolume : masterVolume);
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;
    }

    public void PlaySound(string name)
    {
        if (!isSoundOn) return;

        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Ses bulunamadı: {name}");
            return;
        }
        
        s.source.Play();
        Debug.Log($"Çalınan ses: {name}");
    }

    public void StopSound(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Ses bulunamadı: {name}");
            return;
        }
        
        s.source.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        foreach (Sound s in sounds)
        {
            if (s.source != null)
            {
                s.source.volume = s.volume * masterVolume;
            }
        }
    }

    public void PlayMusic(string levelName)
    {
        // Mevcut müziği durdur
        if (currentMusic != null && currentMusic.source.isPlaying)
        {
            StartCoroutine(FadeOutMusic(currentMusic));
        }

        // Yeni müziği başlat
        Sound newMusic = System.Array.Find(backgroundMusics, music => music.name == levelName);
        if (newMusic == null)
        {
            Debug.LogWarning($"Müzik bulunamadı: {levelName}");
            return;
        }

        StartCoroutine(FadeInMusic(newMusic));
        currentMusic = newMusic;
    }

    private IEnumerator FadeOutMusic(Sound music)
    {
        float startVolume = music.source.volume;
        float fadeTime = 1f;
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            music.source.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
            yield return null;
        }

        music.source.Stop();
    }

    private IEnumerator FadeInMusic(Sound music)
    {
        music.source.volume = 0f;
        music.source.Play();

        float targetVolume = music.volume * musicVolume;
        float fadeTime = 1f;
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            music.source.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / fadeTime);
            yield return null;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        foreach (Sound s in backgroundMusics)
        {
            if (s.source != null)
            {
                s.source.volume = s.volume * musicVolume;
            }
        }
    }

    public void ToggleSound(bool state)
    {
        isSoundOn = state;
        if (!state)
        {
            // Efekt seslerini durdur
            foreach (Sound s in sounds)
            {
                if (s.source.isPlaying)
                {
                    s.source.Stop();
                }
            }
            
            // Müzikleri durdur
            foreach (Sound s in backgroundMusics)
            {
                if (s.source.isPlaying)
                {
                    s.source.Stop();
                }
            }
        }
        else if (currentMusic != null)
        {
            // Sesi açınca mevcut müziği tekrar başlat
            currentMusic.source.Play();
        }
    }

    public void PlayRandomMusic()
    {
        if (backgroundMusics.Length == 0) return;

        // Eğer tüm müzikler çalındıysa listeyi sıfırla
        if (playedMusicIndices.Count >= backgroundMusics.Length)
        {
            playedMusicIndices.Clear();
        }

        // Henüz çalınmamış rastgele bir müzik seç
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, backgroundMusics.Length);
        } while (playedMusicIndices.Contains(randomIndex));

        playedMusicIndices.Add(randomIndex);

        // Mevcut müziği durdur
        if (currentMusic != null && currentMusic.source.isPlaying)
        {
            StartCoroutine(FadeOutMusic(currentMusic));
        }

        // Yeni müziği başlat
        Sound newMusic = backgroundMusics[randomIndex];
        StartCoroutine(FadeInMusic(newMusic));
        currentMusic = newMusic;

        Debug.Log($"Çalınan müzik: {newMusic.name}");
    }

    // Müzik bittiğinde yeni müzik başlatmak için
    private void Update()
    {
        if (currentMusic != null && !currentMusic.source.isPlaying)
        {
            PlayRandomMusic();
        }
    }
} 