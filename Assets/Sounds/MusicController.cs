using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public Text musicText;
    private int volumeLevel = 10;

    void Start()
    {
        float savedVol = AudioManager.Instance.GetMusicVolume();
        volumeLevel = Mathf.RoundToInt(savedVol * 10);
        UpdateVolume();
    }

    public void IncreaseVolume()
    {
        if (volumeLevel < 10)
        {
            volumeLevel++;
            UpdateVolume();
        }
    }

    public void DecreaseVolume()
    {
        if (volumeLevel > 0)
        {
            volumeLevel--;
            UpdateVolume();
        }
    }

    void UpdateVolume()
    {
        float vol = volumeLevel / 10f;
        AudioManager.Instance.SetMusicVolume(vol);
        musicText.text =volumeLevel + " Música";
    }
}
