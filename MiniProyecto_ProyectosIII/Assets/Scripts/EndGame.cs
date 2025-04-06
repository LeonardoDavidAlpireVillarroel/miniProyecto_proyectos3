using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject camerasToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f; // Pausa el juego

            if (endGamePanel != null)
                endGamePanel.SetActive(true); // Activa panel o UI

            if (camerasToDisable != null)
                camerasToDisable.SetActive(false); // Desactiva cámaras
        }
    }
}
