using UnityEngine;
using UnityEngine.Audio; // Required if using AudioMixer later

// Manages background music and sound effects.
public class AudioManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    private static AudioManager _instance;
    public static AudioManager Instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<AudioManager>();
            if (_instance == null) {
                GameObject go = new GameObject("AudioManager");
                 _instance = go.AddComponent<AudioManager>();
            }
            return _instance;
        }
    }

    // --- Audio Sources ---
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    // Optional: [SerializeField] private AudioMixer mainMixer;

    // --- Default Volume ---
    private float masterVolume = 1.0f; // Example overall volume

    // --- Unity Lifecycle ---
    void Awake() {
         // --- Enforce Singleton ---
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
         Debug.Log("AudioManager Awake and Persisting.");

        // --- Find/Create Audio Sources if not assigned ---
        if (musicSource == null) {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // Music usually loops
            musicSource.playOnAwake = false;
             Debug.Log("Music Source created.");
        }
         if (sfxSource == null) {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false; // SFX usually don't loop
            sfxSource.playOnAwake = false;
            Debug.Log("SFX Source created.");
        }

        // --- Load Volume Settings (from PlayerPrefs maybe?) ---
        masterVolume = PlayerPrefs.GetFloat(PREFS_PREFIX + "MasterVolume", 0.5f); // Load saved volume, default 0.5
        SetMasterVolume(masterVolume); // Apply loaded/default volume
    }

     // --- PlayerPrefs Key ---
     private const string PREFS_PREFIX = "AstronautGame_"; // Consistent prefix

    // --- Public Methods ---

    public void PlayMusic(AudioClip musicClip) {
        if (musicSource.clip == musicClip && musicSource.isPlaying) {
            return; // Don't restart if already playing the same clip
        }
        if (musicClip != null) {
            musicSource.clip = musicClip;
            musicSource.Play();
            Debug.Log($"Playing Music: {musicClip.name}");
        } else {
             Debug.LogWarning("PlayMusic called with null clip.");
        }
    }

    public void StopMusic() {
        musicSource.Stop();
         Debug.Log("Music Stopped.");
    }

    // Play a sound effect one time
    public void PlaySFX(AudioClip sfxClip) {
         if (sfxClip != null) {
            // PlayOneShot allows multiple SFX to overlap without cutting each other off
            sfxSource.PlayOneShot(sfxClip, sfxSource.volume); // Use sfxSource's current volume
             Debug.Log($"Playing SFX: {sfxClip.name}");
        } else {
             Debug.LogWarning("PlaySFX called with null clip.");
        }
    }

    // --- Volume Controls ---
    // Basic volume control directly on sources. Using AudioMixer is better for advanced control.
    public void SetMasterVolume(float volume) {
        masterVolume = Mathf.Clamp01(volume); // Ensure volume is between 0 and 1
        // Apply master volume to both sources (or use mixer groups later)
        musicSource.volume = masterVolume; // Simplistic: master affects music directly
        sfxSource.volume = masterVolume;   // Simplistic: master affects SFX directly
         Debug.Log($"Master Volume set to: {masterVolume}");

         // Save volume setting
         PlayerPrefs.SetFloat(PREFS_PREFIX + "MasterVolume", masterVolume);
         PlayerPrefs.Save();
    }

    // Add separate Music/SFX volume later if needed, requires AudioMixer setup ideally.
    /*
    public void SetMusicVolume(float volume) {
        // Requires setting up an exposed parameter on an AudioMixer group
        // mainMixer.SetFloat("MusicVolumeParam", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetSFXVolume(float volume) {
        // mainMixer.SetFloat("SFXVolumeParam", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }
    */

     public float GetMasterVolume() {
        return masterVolume;
     }
}