using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para detectar el cambio de escena

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }
    public UnitHealth _playerHealth = new UnitHealth(100, 100);

    public List<GameObject> enemies = new List<GameObject>(); // Lista de enemigos
    public TextMeshProUGUI enemyCountText; // UI para mostrar el contador
    public int totalEnemies; // N�mero total de enemigos


    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya entre escenas
        }
    }

    void Start()
    {

        SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al cambio de escena
        UpdateEnemyList();
        totalEnemies = enemies.Count;
        UpdateEnemyCount();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Nueva escena cargada: " + scene.name);

        // Buscar nuevamente el texto en la nueva escena
        enemyCountText = GameObject.Find("EnemiesLeft")?.GetComponent<TextMeshProUGUI>();

        UpdateEnemyList(); // Actualizar la lista de enemigos
        totalEnemies = enemies.Count;
        UpdateEnemyCount();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse cuando el objeto se destruye
    }

    // Actualiza la lista de enemigos y almacena el n�mero total de enemigos
    public void UpdateEnemyList()
    {
        enemies.Clear();
        GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in foundEnemies)
        {
            if (enemy != null)
                enemies.Add(enemy);
        }

        totalEnemies = enemies.Count;
    }

    // Elimina un enemigo de la lista
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            UpdateEnemyCount();
        }
    }

    // Actualiza el contador de enemigos en la UI
    public void UpdateEnemyCount()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = "Enemies Left: " + enemies.Count + "/" + totalEnemies;
        }
    }
    // Reinicia la escena actual (ideal para usar en bot�n "Reintentar")

}
