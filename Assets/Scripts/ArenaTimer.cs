using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ArenaTimer : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public GameObject entranceGate, exitGate;
    public Text timerText;

    public float arenaDuration = 60f;
    public float spawnInterval = 5f;

    private bool arenaActive = false;
    private float remainingTime;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        if (timerText != null)
        {
            timerText.text = "";
            timerText.enabled = false;
        }

        if (SaveManager.IsArenaTimerComplete())
        {
            gameObject.SetActive(false);
            Debug.Log("Arena Timer já completada.");
        }
        else
        {
            ResetArena(); 
        }
    }


    public void StartArena()
    {
        if (!arenaActive && !SaveManager.IsArenaTimerComplete())
        {
            remainingTime = arenaDuration;
            arenaActive = true;

            entranceGate?.SetActive(true);
            exitGate?.SetActive(true);

            if (timerText != null)
            {
                timerText.enabled = true;
            }

            StartCoroutine(ArenaCountdown());
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator ArenaCountdown()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        EndArena();
    }

    IEnumerator SpawnEnemies()
    {
        while (arenaActive)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        int randomEnemy = Random.Range(0, enemyPrefabs.Length);
        int randomSpawnPoint = Random.Range(0, spawnPoints.Length);

        GameObject enemy = Instantiate(enemyPrefabs[randomEnemy], spawnPoints[randomSpawnPoint].position, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = seconds.ToString();
        }
    }

    void EndArena()
    {
        arenaActive = false;

        entranceGate?.SetActive(false);
        exitGate?.SetActive(false);

        if (timerText != null)
        {
            timerText.enabled = false;
        }

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        activeEnemies.Clear();

        SaveManager.MarkArenaTimerComplete();
        Debug.Log("Arena Timer completada!");
    }

    public void ResetArena()
    {
        StopAllCoroutines();
        arenaActive = false;
        remainingTime = arenaDuration;

        entranceGate?.SetActive(false);
        exitGate?.SetActive(false);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.enabled = false;
        }

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        activeEnemies.Clear();
    }
}
