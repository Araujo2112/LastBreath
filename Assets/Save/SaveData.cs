using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float positionX;
    public float positionY;
    public int health;
    public int maxHealth;
    public int gameProgress;
    public bool arenaTimerComplete;
    public bool arenaWaveComplete;
    public string currentScene;

    public SaveData(Vector2 position, int health, int maxHealth, int gameProgress, bool arenaTimerComplete, bool arenaWaveComplete, string currentScene)
    {
        this.positionX = position.x;
        this.positionY = position.y;
        this.health = health;
        this.maxHealth = maxHealth;
        this.gameProgress = gameProgress;
        this.arenaTimerComplete = arenaTimerComplete;
        this.arenaWaveComplete = arenaWaveComplete;
        this.currentScene = currentScene;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(positionX, positionY);
    }

    public bool IsArenaTimerComplete()
    {
        return arenaTimerComplete;
    }

    public void SetArenaTimerComplete(bool complete)
    {
        arenaTimerComplete = complete;
    }

    public bool IsArenaWaveComplete()
    {
        return arenaWaveComplete;
    }

    public void SetArenaWaveComplete(bool complete)
    {
        arenaWaveComplete = complete;
    }

    public string GetCurrentScene()
    {
        return currentScene;
    }

    public void SetCurrentScene(string sceneName)
    {
        currentScene = sceneName;
    }
}
