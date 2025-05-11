using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    private bool isActivated = false;
    private bool canActivate = false;
    public float activationDelay = 2f;

    private void Start()
    {
        Invoke("EnableActivation", activationDelay);
    }

    private void EnableActivation()
    {
        canActivate = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated && canActivate)
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // Salva o progresso do jogador incluindo o estado da arena
                ArenaController arenaController = FindObjectOfType<ArenaController>();
                bool arenaCompleta = arenaController != null && arenaController.IsArenaComplete();

                player.SavePlayer(checkpointID, arenaCompleta);
                CheckpointMessage.Instance.ShowMessage("CHECKPOINT SALVO");
                Debug.Log("Checkpoint " + checkpointID + " guardado! Arena completa: " + arenaCompleta);
                
                isActivated = true;
            }
        }
    }
}
