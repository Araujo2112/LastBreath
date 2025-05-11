using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    public ArenaController arenaController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger (ArenaTrigger) ativado com: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou na arena via EntryTrigger.");
            arenaController.AtivarArena();
        }
    }
}
