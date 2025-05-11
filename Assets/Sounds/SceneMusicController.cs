using UnityEngine;

public class SceneMusicController : MonoBehaviour
{
    public AudioClip sceneMusic;
    public float fadeDuration = 1f;

    void Start()
    {
        if (sceneMusic != null && !AudioManager.Instance.musicaBloqueada)
        {
            AudioManager.Instance.PlayMusic(sceneMusic, true, fadeDuration);
        }
    }
}
