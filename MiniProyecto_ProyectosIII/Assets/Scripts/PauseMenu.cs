using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject playerInterface;
    public GameObject cameras;
    public UnitHealth health;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        cameras.SetActive(true); // Asegúrate de que la cámara esté activa al reanudar
        playerInterface.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause()
    {
        cameras.SetActive(false); // Desactiva la cámara cuando el juego se pausa
        playerInterface.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void FirstLevel()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté corriendo por si venía de una pausa o victoria

        if (cameras != null)
            cameras.SetActive(true); // Activa la cámara por si estaba desactivada

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false); // Oculta el menú de pausa por si estaba visible

        if (playerInterface != null)
            playerInterface.SetActive(true); // Activa la interfaz del jugador

        SceneManager.LoadScene("Level 1"); // Carga el primer nivel
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        cameras.SetActive(true);

        // Encontrar al jugador y reiniciar su salud
        GameManager.gameManager._playerHealth.HealUnit(100);



        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Por si estaba en pausa
                             // Reactivar la cámara antes de volver al menú
        cameras.SetActive(true); // Asegura que la cámara esté activada
        SceneManager.LoadScene("MainMenu"); // Asegúrate de que la escena se llame así
    }
}

