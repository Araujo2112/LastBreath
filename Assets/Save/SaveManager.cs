using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    private static string savePath = Application.persistentDataPath + "/save.json";
    private static SaveData currentSave;

    public static void SaveGame(PlayerController player, int gameProgress, bool arenaTimerComplete, bool arenaWaveComplete)
    {
        currentSave = new SaveData(
            player.transform.position,
            player.currentHealth,
            player.maxHealth,
            gameProgress,
            arenaTimerComplete,
            arenaWaveComplete,
            SceneManager.GetActiveScene().name
        );

        string json = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Jogo guardado em: " + savePath);
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSave = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Jogo carregado de: " + savePath);
            return currentSave;
        }
        else
        {
            Debug.LogWarning("Nenhum ficheiro de save encontrado.");
            currentSave = new SaveData(Vector2.zero, 5, 5, 0, false, false, SceneManager.GetActiveScene().name);
            return currentSave;
        }
    }

    public static bool IsArenaTimerComplete()
    {
        return currentSave != null && currentSave.IsArenaTimerComplete();
    }

    public static void MarkArenaTimerComplete()
    {
        if (currentSave != null)
        {
            currentSave.SetArenaTimerComplete(true);
            SaveCurrentState();
        }
    }

    public static bool IsArenaWaveComplete()
    {
        return currentSave != null && currentSave.IsArenaWaveComplete();
    }

    public static void MarkArenaWaveComplete()
    {
        if (currentSave != null)
        {
            currentSave.SetArenaWaveComplete(true);
            SaveCurrentState();
        }
    }

    private static void SaveCurrentState()
    {
        if (currentSave != null)
        {
            string json = JsonUtility.ToJson(currentSave, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Estado atual guardado em: " + savePath);
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save apagado.");
        }
    }
}
