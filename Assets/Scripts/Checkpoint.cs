using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Carro"))
        {
            LapManager.instance.CarroPassouCheckpoint(other.gameObject, transform);
        }
    }
}
