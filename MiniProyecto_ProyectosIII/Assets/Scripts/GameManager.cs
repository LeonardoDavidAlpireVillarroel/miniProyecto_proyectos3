using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }
    public UnitHealth _playerHealth = new UnitHealth(100, 100);

    public List<GameObject> enemies = new List<GameObject>(); // Lista de enemigos
    public TextMeshProUGUI enemyCountText; // Asumiendo que tienes un texto en la UI para mostrar el contador
    public int totalEnemies; // N�mero total de enemigos

    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(this);
        }
        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject); // Esto evita que el GameManager se destruya entre escenas
        }
    }

    void Start()
    {
        // Aseg�rate de actualizar la lista y el total al inicio
        if (totalEnemies == 0)  // Asigna el total solo una vez
        {
            UpdateEnemyList(); // Llenar la lista de enemigos
            totalEnemies = enemies.Count; // Asignar el total de enemigos
            Debug.Log("Total Enemies: " + totalEnemies); // Verifica cu�ntos enemigos hay
        }
        UpdateEnemyCount(); // Aseg�rate de actualizar el contador en la UI
    }

    // Actualiza la lista de enemigos y almacena el n�mero total de enemigos
    public void UpdateEnemyList()
    {
        enemies.Clear();
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    // Elimina un enemigo de la lista
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy); // Remover el enemigo de la lista
            UpdateEnemyCount(); // Actualizar el contador de enemigos
        }
    }

    // Actualiza el contador de enemigos en la UI
    public void UpdateEnemyCount()
    {
        if (enemyCountText != null)
        {
            // Muestra el n�mero de enemigos restantes sobre el total de enemigos
            enemyCountText.text = "Enemies Left: " + enemies.Count + "/" + totalEnemies;
        }
    }
}
