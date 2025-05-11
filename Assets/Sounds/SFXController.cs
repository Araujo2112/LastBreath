using UnityEngine;
using UnityEngine.UI;

public class SFXController : MonoBehaviour
{
    public Text sfxText;
    private int sfxLevel = 5;

    void Start()
    {
        float savedVol = AudioManager.Instance.GetSFXVolume();
        sfxLevel = Mathf.RoundToInt(savedVol * 10);
        UpdateSFX();
    }

    public void IncreaseSFX()
    {
        if (sfxLevel < 10)
        {
            sfxLevel++;
            UpdateSFX();
            PlaySample();
        }
    }

    public void DecreaseSFX()
    {
        if (sfxLevel > 0)
        {
            sfxLevel--;
            UpdateSFX();
            PlaySample();
        }
    }

    void UpdateSFX()
    {
        float vol = sfxLevel / 10f;
        AudioManager.Instance.SetSFXVolume(vol);
        sfxText.text = sfxLevel + " Efeito sonoro";

        // Toca o som de exemplo diretamente pelo AudioManager
        if (AudioManager.Instance.sfxSource != null)
        {
            AudioManager.Instance.sfxSource.volume = vol;
        }
    }


    void PlaySample()
    {
        if (AudioManager.Instance != null)
        {
            AudioClip sample = AudioManager.Instance.sfxSource.clip;
            if (sample != null)
            {
                AudioManager.Instance.PlaySFX(sample);
            }
        }
    }

}
