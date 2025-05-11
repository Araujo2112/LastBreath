using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fontes de Áudio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    private AudioSource dialogueSource;
    private AudioSource tempMusicSource; 
    public bool musicaBloqueada = false;


    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 5f)] public float fadeDuration = 1f;

    private Coroutine musicFadeCoroutine;

void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("AudioManager não é um objeto de raiz. Remove-o do parent.");
        }

        // Garante que as fontes de áudio estão atribuídas
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            Debug.LogWarning("MusicSource não estava atribuído. Foi criado automaticamente.");
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            Debug.LogWarning("SFXSource não estava atribuído. Foi criado automaticamente.");
        }

        dialogueSource = gameObject.AddComponent<AudioSource>();
        dialogueSource.loop = true;
        dialogueSource.playOnAwake = false;

        // Criar a segunda fonte para crossfade
        tempMusicSource = gameObject.AddComponent<AudioSource>();
        tempMusicSource.loop = true;
        tempMusicSource.playOnAwake = false;

        ApplyVolumes();
    }
    else
    {
        Destroy(gameObject);
    }
}



    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
        PlayerPrefs.SetFloat("musicVolume", vol);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public void SetSFXVolume(float vol)
    {
        sfxVolume = vol;
        PlayerPrefs.SetFloat("sfxVolume", vol);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;

    public void PlayMusic(AudioClip newClip, bool loop = true, float fadeDurationOverride = -1f)
    {
        if (newClip == null || musicaBloqueada) return; // <──── BLOQUEIO ABSOLUTO
        
        if (musicSource == null)
        {
            Debug.LogError("MusicSource não está atribuído no AudioManager!");
            return;
        }

        if (musicSource.isPlaying && musicSource.clip == newClip) return;

        float fadeTime = fadeDurationOverride > 0f ? fadeDurationOverride : fadeDuration;

        tempMusicSource.clip = newClip;
        tempMusicSource.loop = loop;
        tempMusicSource.volume = 0f;
        tempMusicSource.Play();

        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(CrossfadeMusic(fadeTime));
    }


    private IEnumerator CrossfadeMusic(float duration)
    {
        float startVolume = musicSource.volume;
        float targetVolume = musicVolume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Fade out da música atual
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t);

            // Fade in da nova música
            tempMusicSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        // Troca as fontes para o novo clip ser a música principal
        AudioSource oldSource = musicSource;
        musicSource = tempMusicSource;
        tempMusicSource = oldSource;
        tempMusicSource.Stop();  // Para a música antiga
        tempMusicSource.clip = null;

        ApplyVolumes();
    }

    public void StopMusic(float fadeOutDuration = 1f)
    {
        if (musicSource == null)
        {
            Debug.LogError("MusicSource não está atribuído no AudioManager!");
            return;
        }

        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        StartCoroutine(FadeOutAndStopMusic(fadeOutDuration));
    }


    private IEnumerator FadeOutAndStopMusic(float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = null;
        musicSource.volume = musicVolume; // Reseta para o volume normal
    }


    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayDialogueSound(AudioClip clip, float pitch = 1f)
    {
        if (clip == null || dialogueSource == null) return;
        dialogueSource.clip = clip;
        dialogueSource.pitch = pitch;
        dialogueSource.volume = sfxVolume;
        dialogueSource.Play();
    }

    public void StopDialogueSound()
    {
        if (dialogueSource != null)
        {
            dialogueSource.Stop();
        }
    }

    private void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (dialogueSource != null) dialogueSource.volume = sfxVolume;
    }
}
