using UnityEngine;

public class DoorTriggerChangeLevel : MonoBehaviour
{
    public GameObject door; // Referencia a la puerta
    public float speed = 2f; // Velocidad de apertura y cierre
    private Vector3 initialPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private bool isClosing = false;

    void Start()
    {
        if (door != null)
        {
            initialPosition = door.transform.position;
            openPosition = initialPosition + new Vector3(4f, 0f, 0f); // Mover 4 unidades a la derecha
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && door != null && GameManager.gameManager.enemies.Count == 0)
        {
            isOpening = true;
            isClosing = false; // Detiene el cierre si el jugador vuelve a entrar
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && door != null)
        {
            isClosing = true;
            isOpening = false; // Detiene la apertura si el jugador sale
        }
    }

    void Update()
    {
        if (isOpening && door != null)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, openPosition, Time.deltaTime * speed);
        }
        else if (isClosing && door != null)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, initialPosition, Time.deltaTime * speed);
        }
    }
}