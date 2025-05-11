using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public Collider2D entryTrigger;
    public GameObject[] portoes;
    public List<GameObject> hordas;
    public GameObject rewardObject;
    public AudioClip arenaMusic;
    public AudioClip ambientMusic;
    public float fadeDuration = 1f;

    private int hordaAtual = 0;
    private bool arenaAtivada = false;
    private bool arenaCompleta = false;

    void Start()
    {
        arenaCompleta = SaveManager.IsArenaWaveComplete();
        if (arenaCompleta)
        {
            Debug.Log("Arena já completada. Desativando objetos.");
            gameObject.SetActive(false);
            return;
        }

        foreach (var portao in portoes)
        {
            portao.SetActive(false);
        }

        foreach (var horda in hordas)
        {
            horda.SetActive(false);
        }

        if (rewardObject != null)
        {
            rewardObject.SetActive(false);
        }

        if (ambientMusic != null)
        {
            AudioManager.Instance?.PlayMusic(ambientMusic);
        }
    }

    public void AtivarArena()
    {
        if (arenaAtivada || arenaCompleta) return;
        arenaAtivada = true;

        foreach (var portao in portoes)
        {
            portao.SetActive(true);
        }

        if (arenaMusic != null)
        {
            AudioManager.Instance?.PlayMusic(arenaMusic);
        }

        StartCoroutine(AtivarHordasPorOrdem());
    }

    IEnumerator AtivarHordasPorOrdem()
    {
        while (hordaAtual < hordas.Count)
        {
            GameObject horda = hordas[hordaAtual];
            horda.SetActive(true);

            while (HordaTemInimigos(horda))
            {
                yield return new WaitForSeconds(0.5f);
            }

            hordaAtual++;
        }

        foreach (var portao in portoes)
        {
            portao.SetActive(false);
        }

        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
        }

        if (ambientMusic != null)
        {
            AudioManager.Instance?.PlayMusic(ambientMusic);
        }

        SaveManager.SaveGame(FindObjectOfType<PlayerController>(), 1, SaveManager.IsArenaTimerComplete(), SaveManager.IsArenaWaveComplete()
);

        Debug.Log("Arena completada e guardada!");
    }

    bool HordaTemInimigos(GameObject horda)
    {
        foreach (Transform child in horda.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsArenaComplete()
    {
        return arenaCompleta;
    }

        public void ResetArena()
    {
        arenaAtivada = false;
        arenaCompleta = false;
        hordaAtual = 0;

        foreach (var portao in portoes)
        {
            portao.SetActive(false);
        }

        foreach (var horda in hordas)
        {
            horda.SetActive(false);
        }

        if (rewardObject != null)
        {
            rewardObject.SetActive(false);
        }

        if (ambientMusic != null)
        {
            AudioManager.Instance?.PlayMusic(ambientMusic);
        }

        Debug.Log("Arena reiniciada.");
    }

}
