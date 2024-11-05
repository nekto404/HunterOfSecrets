using UnityEngine;
using UnityEngine.Audio;


public class AudioController : MonoBehaviour
{
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    public static AudioController Instance { get; private set; }

    [SerializeField] 
    private AudioMixer audioMixer;

    [SerializeField] 
    private AudioSource musicAudioSource;
    [SerializeField]
    private AudioSource sfxAudioSource;
    [SerializeField]
    private float musicVolume;
    [SerializeField]
    private float sfxVolume;

    [SerializeField] 
    private AudioClip clickSound;

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
        }

    }

    private void Start()
    {

        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 100f);
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 100f);

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0f, 100f);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        audioMixer.SetFloat(MusicVolumeKey, musicVolume > 0 ? Mathf.Log10(musicVolume / 100) * 20 : -80f);

    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 100f); 
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume); 
        audioMixer.SetFloat(SFXVolumeKey, sfxVolume > 0 ? Mathf.Log10(sfxVolume / 100) * 20 : -80f);
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void ClickSound()
    {
        sfxAudioSource.clip = clickSound;
        sfxAudioSource.Play();
    }
}
