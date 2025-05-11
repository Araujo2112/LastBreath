using UnityEngine;

public class ArenaTimerTrigger : MonoBehaviour
{
    public ArenaTimer arenaTimer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            arenaTimer.StartArena();

            if (SaveManager.IsArenaTimerComplete())
            {
                gameObject.SetActive(false);
            }
        }
    }
}
